using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

//Every Player move Script Unite here;
//모든 플레이어 움직임 관련코드 이걸로 통일
//주석처리된 코드는 전부 주석 해제 해야함

public class PlayerMovement : NetworkBehaviour
{

    //ThirdPerson Movement
    Animator _animator;
    Camera _camera;
    [SerializeField]
    GameObject MainCamera;
    CharacterController _controller;

    public float speed = 5f;
    public float runspeed = 8f;
    public float finalSpeed;
    public bool toggleCameraRotation;
    public bool run;
    public float smoothness = 10f;

    //Player State
    public string playerState = "";
    //Tiger features(from Tiger_Controller.cs)
    private int huntFailures = 0;
    private bool isPenaltyActive = false;
    private int penaltyType = 0;
    

    //Fox features(from fox1.cs)
    // public GameObject[] Bead;
    // public bool[] hasBeads; 
    bool iDown;
    GameObject nearObject;

    GameObject UIManagerObject;

    UIManager _uiManager; //UI관리
    HealthBar _healthBar; //여우 체력바




    // // Start is called before the first frame update
    void Start()
    {
        _animator = this.GetComponent<Animator>();
        _camera = Camera.main;
        _controller = this.GetComponent<CharacterController>();
        

        if(!IsOwner){
            MainCamera.gameObject.SetActive(false);
        }else{
            MainCamera.gameObject.SetActive(true);
        }
        
        UIManagerObject = GameObject.Find("UIManager");
        _uiManager = UIManagerObject.GetComponent<UIManager>(); // UIManager 스크립트 가져오기
        _healthBar = UIManagerObject.GetComponent<HealthBar>(); // HealthBar 스크립트 가져오기

        UIManagerObject.GetComponent<UIManager>().playerState = playerState;
        _uiManager.UIEnable();
        _healthBar.IsGameStarted = true;

    }

    // Update is called once per frame
    void Update()
    {
        // //For Network Owner
        if(!IsOwner){
            return;
        }

        // PlayerState Separate
        if(playerState=="Fox")
        {
            iDown = Input.GetButtonDown("Interaction");
            Interaction();
        }
        else if(playerState=="Tiger")
        {
            //Tiger Penalty(from Tiger_Controller.cs)
            if (isPenaltyActive)
            {
                ApplyPenalty();
            }

            //Tiger UI update
            _uiManager.UpdateHeartUI();
        }
        else
        {

        }



        //Thrid Person Movement
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            toggleCameraRotation = true; // �ѷ����� Ȱ��ȭ
        }
        else
        {
            toggleCameraRotation = false; // �ѷ����� ��Ȱ��ȭ
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            run = true;
        }
        else
        {
            run = false;
        }
        InputMovement();
        
    }

    void LateUpdate()
    {

        if(!IsOwner){
            return;
        }

        if (toggleCameraRotation != true)
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);

        }
    }

    void InputMovement()
    {
        finalSpeed = (run) ? runspeed : speed;

        Vector3 forword = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        Vector3 moveDirection = forword * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");

        _controller.Move(moveDirection.normalized * finalSpeed * Time.deltaTime);

        float percent = ((run) ? 1 : 0.5f) * moveDirection.magnitude;
        _animator.SetFloat("Blend",  percent,0.1f,Time.deltaTime);
    }


    //Tiger Penalty
    void ApplyPenalty()
    {
        switch (penaltyType)
        {
            case 1:
                // 속도 감소
                break;
            case 2:
                // 방향 반전
                break;
            case 3:
                if (_uiManager.halfScr != null)
                {
                    _uiManager.halfScr.ActivateHalfScreenPenalty();
                }
                break;
        }
    }


    public void HuntFailure()
    {
        if (_uiManager.health_tiger > 0)
        {
            _uiManager.health_tiger = _uiManager.health_tiger - 1;
            Debug.Log("Hunt Fail!");
        }

        _uiManager.UpdateHeartUI();

        if (_uiManager.health_tiger == 0)
        {
            ApplyRandomPenalty();
            StartCoroutine(ResetHealthAfterDelay());
        }
    }

    void ApplyRandomPenalty()
    {
        penaltyType = Random.Range(1, 4);
        isPenaltyActive = true;

        switch (penaltyType)
        {
            case 1:
                Debug.Log("Penalty applied: Speed reduced.");
                break;
            case 2:
                Debug.Log("Penalty applied: Controls reversed.");
                break;
            case 3:
                Debug.Log("Penalty applied: Half screen visibility.");
                break;
        }

        StartCoroutine(PenaltyTimer());
    }

    IEnumerator PenaltyTimer()
    {
        yield return new WaitForSeconds(5);
        RemovePenalty();
    }

    void RemovePenalty()
    {
        isPenaltyActive = false;
        penaltyType = 0;
        if (_uiManager.halfScr != null)
        {
            _uiManager.halfScr.DeactivateHalfScreenPenalty();
        }
        Debug.Log("Penalty removed.");
    }

    
    IEnumerator ResetHealthAfterDelay()
    {
        yield return new WaitForSeconds(5);
        _uiManager.health_tiger = 3;
        _uiManager.UpdateHeartUI();
    }


    //Fox Interactions
    
    void Interaction()
    {
        //여의주 먹기
        if(iDown && nearObject != null) //jdown 조건에서 잠깐 뺐음
        {
            if(nearObject.tag == "Bead")
            {
                // Item item = nearObject.GetComponent<Item>();
                // int beadIndex = item.value;
                // hasBeads[beadIndex] = true;

                _uiManager.beadCount++;
                _uiManager.UpdateBeadCountText();

                // Destroy(nearObject); //destroy 보다 이게 나을거 같아서 바꿈
                nearObject.GetComponent<BoxCollider>().enabled = false;
                nearObject.GetComponent<MeshRenderer>().enabled = false;
            }
            else if(nearObject.tag == "Food") //From HealthBar.cs
            {
                _healthBar.EatFood(5f);
                nearObject.GetComponent<BoxCollider>().enabled = false;
                nearObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
    void OnTriggerStay(Collider other)
    {
        //닿은 여의주 저장
        if (other.tag == "Bead" || other.tag == "Food")
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        //여의주가 있는 영역을 벗어나면 여의주 없애기
        if (other.tag == "Bead")
            nearObject = null;
    }

}