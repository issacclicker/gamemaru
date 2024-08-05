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

    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    private Vector3 velocity;
    private bool isGrounded;

    //Player State
    [SerializeField]
    public string playerState;
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

    public GameObject animalModel; //이미호
    public GameObject currentModel;
    public GameObject originalModel;
    public bool isAwaken = false; //이미호 인지 아닌지

    private ScoreManager scoreManager; //점수 관리

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    if (_animator == null)
    {     
        Debug.LogError("Animator component is not found on the GameObject.");
    }
    else
    {
        Debug.Log("Animator component is found.");
    }
        _camera = Camera.main;
        _controller = GetComponent<CharacterController>();

        //디버깅 용
    if (string.IsNullOrEmpty(playerState))
    {
        if (IsHost)
        {
            playerState = "Tiger";
        }
        else
        {
            playerState = "Fox";
        }
    }


        if (!IsOwner)
        {
            MainCamera.gameObject.SetActive(false);
            return;
        }
        else
        {
            MainCamera.gameObject.SetActive(true);
        }

        UIManagerObject = GameObject.Find("UIManager");
        _uiManager = UIManagerObject.GetComponent<UIManager>(); // UIManager 스크립트 가져오기
        _healthBar = UIManagerObject.GetComponent<HealthBar>(); // HealthBar 스크립트 가져오기

        UIManagerObject.GetComponent<UIManager>().playerState = playerState;
        _uiManager.UIEnable();
        _healthBar.IsGameStarted = true;

        scoreManager = GameObject.FindObjectOfType<ScoreManager>();

        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager instance not found in the scene.");
        }
    }

    void Update()
    {
        // For Network Owner
        if (!IsOwner)
        {
            return;
        }

        // PlayerState Separate
        if (playerState == "Fox")
        {
            iDown = Input.GetButtonDown("Interaction");
            if (!isAwaken)
            {
                Interaction();
            }
        }
        else if (playerState == "Tiger")
        {
            // Tiger Penalty(from Tiger_Controller.cs)
            if (isPenaltyActive)
            {
                ApplyPenalty();
            }

            // Tiger UI update
            _uiManager.UpdateHeartUI();
        }

        // Third Person Movement
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            toggleCameraRotation = true; // 자유 시점 활성화
        }
        else
        {
            toggleCameraRotation = false; // 자유 시점 비활성화
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            run = true;
        }
        else
        {
            run = false;
        }

        isGrounded = _controller.isGrounded;

        // 현재 플레이어가 지면에 닿아있지 않는 문제 확인
        //Debug.Log($"isGrounded: {isGrounded}");

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump();
            }
        }

        InputMovement();

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        _controller.Move(velocity * Time.deltaTime);
    }

    void LateUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        if (!toggleCameraRotation)
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
        }
    }

    void InputMovement()
    {
        finalSpeed = run ? runspeed : speed;

        if (isPenaltyActive && penaltyType == 1)
        {
            finalSpeed /= 2;
        }

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (isPenaltyActive && penaltyType == 2)
        {
            verticalInput = -verticalInput;
            horizontalInput = -horizontalInput;
        }

        Vector3 moveDirection = forward * verticalInput + right * horizontalInput;

        _controller.Move(moveDirection.normalized * finalSpeed * Time.deltaTime);

        float percent = (run ? 1 : 0.5f) * moveDirection.magnitude;
        _animator.SetFloat("Blend", percent, 0.1f, Time.deltaTime);
    }

    void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            _animator.SetTrigger("Jump");
        }
    }


    // Tiger Penalty
    void ApplyPenalty()
    {
        switch (penaltyType)
        {
            case 1:
                finalSpeed = run ? runspeed / 2 : speed / 2;
                break;
            case 2:
                // Reverse controls handled in InputMovement
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
                nearObject = null;
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

    //이미호로 바뀌는 함수
    private void ChangeModel()
    {
        if (_uiManager.beadCount >= 2 && !isAwaken && IsOwner)
        {
            if (currentModel != originalModel)
            {
                Destroy(currentModel);
            }

            GameObject newModel = Instantiate(animalModel, transform.position, transform.rotation);

            newModel.transform.SetParent(transform);
            currentModel = newModel;
            isAwaken = true;

            if (originalModel != null)
            {
                Renderer[] renderers = originalModel.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    renderer.enabled = false;
                }
            }

            Renderer[] newModelRenderers = newModel.GetComponentsInChildren<Renderer>();
            foreach (var renderer in newModelRenderers)
            {
                renderer.enabled = true;  
                Debug.Log(renderer.name + " is now enabled: " + renderer.enabled);
            }
        }
    }

    //생명의 샘에 닿으면 모습을 바꿈
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Lake")
        {
            ChangeModel();
        }

        if (other.gameObject.name == "Girl" && playerState == "Fox" && isAwaken && IsOwner)
        {
            AddScoreServerRpc("Fox", 1); // 여우가 소녀한테 닿으면 점수 1점 얻음
            EndGame(); 
        }
    }

    //게임 종료
    private void EndGame()
    {
        if (scoreManager != null)
        {
            scoreManager.EndGame();
        }
    }

    [ServerRpc]
    private void AddScoreServerRpc(string playerType, int points)
    {
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager is not initialized.");
            return;
        }

        if (playerType == "Fox")
        {
            scoreManager.AddFoxScore(points);
        }
        else if (playerType == "Tiger")
        {
            scoreManager.AddTigerScore(points);
        }
    }

    //모델 바꾸는 함수
    public void RevertToOriginalModel()
    {
        if (isAwaken && IsOwner)
        {
            Destroy(currentModel);

            if (originalModel != null)
            {
                Renderer[] renderers = originalModel.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    renderer.enabled = true;
                }
            }

            isAwaken = false;
        }
    }

}