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
    private HalfScreen halfScr; //from HalfScreen.cs

    //Tiger UI(from Tiger_Controller.cs)
    // public GameObject Heart1, Heart2,Heart3;
    //체력 디버깅용
    [SerializeField] 
    public static int health;

    //Fox features(from fox1.cs)
    private int beadCount = 0;
    // public GameObject[] Bead;
    // public bool[] hasBeads; 
    // public Text beadCountText;
    bool iDown;
    GameObject nearObject;

    // void Awake()
    // {
    //     UpdateBeadCountText();
    // }



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

        // playerState = "Fox"; //테스트를 위한 플레이어 상태 설정
        

        if(playerState == "Tiger"){
            // halfScr = FindObjectOfType<HalfScreen>();
            health = 3;
        }
        


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
            // UpdateHeartUI();
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
                if (halfScr != null)
                {
                    halfScr.ActivateHalfScreenPenalty();
                }
                break;
        }
    }


    public void HuntFailure()
    {
        if (health > 0)
        {
            health = health - 1;
            Debug.Log("Hunt Fail!");
        }

        // UpdateHeartUI();

        if (health == 0)
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
        if (halfScr != null)
        {
            halfScr.DeactivateHalfScreenPenalty();
        }
        Debug.Log("Penalty removed.");
    }

    //하트 UI 표시
    // void UpdateHeartUI()
    // {
    //     Heart1.SetActive(health >= 1);
    //     Heart2.SetActive(health >= 2);
    //     Heart3.SetActive(health >= 3);
    // }
    IEnumerator ResetHealthAfterDelay()
    {
        yield return new WaitForSeconds(5);
        health = 3;
        // UpdateHeartUI();
    }


    //Fox Interactions
    // private void UpdateBeadCountText()
    // {
    //     beadCountText.text = "Number: " + beadCount;
    // }
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

                beadCount++;
                // UpdateBeadCountText();

                // Destroy(nearObject);
                nearObject.SetActive(false);
            }
        }
    }
    void OnTriggerStay(Collider other)
    {
        //닿은 여의주 저장
        if (other.tag == "Bead")
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        //여의주가 있는 영역을 벗어나면 여의주 없애기
        if (other.tag == "Bead")
            nearObject = null;
    }

}