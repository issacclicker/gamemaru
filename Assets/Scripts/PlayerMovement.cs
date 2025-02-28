using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
// using Unity.Collections.dll;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

//Every Player move Script Unite here;
//모든 플레이어 움직임 관련코드 이걸로 통일

public class PlayerMovement : NetworkBehaviour
{

    public static PlayerMovement Instance;

    //ThirdPerson Movement
    Animator _animator;
    [SerializeField]Camera _camera;
    [SerializeField]
    GameObject MainCamera;
    CharacterController _controller;

    public float speed = 5f;
    public float runspeed = 8f;
    public float finalSpeed;
    public bool toggleCameraRotation;
    public bool run;
    public float smoothness = 10f;


    public Tiger_roaring _tiger_roaring;

    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    private Vector3 velocity;
    private bool isGrounded;



    [SerializeField]private bool isGhost=false;


    //Player State
    [SerializeField] public string playerState;

    [SerializeField] public NetworkVariable<FixedString128Bytes> playerStateSync = new NetworkVariable<FixedString128Bytes>(); //네트워크 동기화용

    [SerializeField] private Material newSkyboxMaterial;//하늘
    [SerializeField] private GameObject spotLight;//빛
    //Tiger features(from Tiger_Controller.cs)
    private int huntFailures = 0;
    private bool isPenaltyActive = false;
    private int penaltyType = 0;
    

    //Fox features(from fox1.cs)
    // public GameObject[] Bead;
    // public bool[] hasBeads; 
    bool iDown;
    [SerializeField]GameObject nearObject;

    GameObject UIManagerObject;

    public UIManager _uiManager; //UI관리
    HealthBar _healthBar; //여우 체력바

    public GameObject animalModel;//이미호
    public NetworkVariable<NetworkObjectReference> currentModel; 
    public GameObject originalModel;

    //player_animation
    public float BlendValue { get; private set; }
    private player_animation playerAnimation;

    //Tiger_animation
    private Tiger_animation tigerAnimation;
    
    public NetworkVariable<bool> isAwaken = new NetworkVariable<bool>();

    private ScoreManager scoreManager;

    //자기 자신 히트박스
    [SerializeField]private Collider MyHitBox;


    void Awake()
    {
        Instance = this;
    }



    // // Start is called before the first frame update
    void Start()
    {
        // _animator = this.GetComponent<Animator>();
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {     
            Debug.LogError("Animator component is not found on the GameObject.");
        }
        else
        {
            Debug.Log("Animator component is found.");
        }


        if(IsOwner)
        {
            MyHitBox.enabled = false;
        }
        

        _camera = Camera.main;
        // _controller = this.GetComponent<CharacterController>();
         _controller = GetComponent<CharacterController>();

        // //디버깅 용
        if(IsOwner)
        {
            if(IsHost){
                playerState = "Tiger"; //ServerRpc로 바꿔야함
                Set_playerStateSyncServerRpc("Tiger");
                
            }else{
                playerState = "Fox"; //ServerRpc로 바꿔야함
                Set_playerStateSyncServerRpc("Fox");
            }
                
            // }
            // playerState = "Tiger";  
            // Set_playerStateSyncServerRpc("Tiger");
            // playerState = "Fox";                //기본은 여우로 하고 한 클라이언트만 호랑이로 바꾸기
            // Set_playerStateSyncServerRpc("Fox");
        }

        // if(string.IsNullOrEmpty(playerState))
        // {
        //     playerState = "Fox";
        // }
        //////////////////////////

        isAwaken.Value = false;


        if(!IsOwner){
            MainCamera.gameObject.SetActive(false);
            return;
        }else{
            MainCamera.gameObject.SetActive(true);
        }
        
        UIManagerObject = GameObject.Find("UIManager");
        _uiManager = UIManagerObject.GetComponent<UIManager>(); // UIManager 스크립트 가져오기
        _healthBar = UIManagerObject.GetComponent<HealthBar>(); // HealthBar 스크립트 가져오기

        UIManagerObject.GetComponent<UIManager>().playerState = playerState;
        _uiManager.UIEnable();
        _healthBar.IsGameStarted = true;


        //UI Manager
        if(IsOwner)
        {
            _uiManager.PlayerCounterText.GetComponent<PlayerCounterNetwork>().ChangePlayerCountTextServerRpc();
            Debug.Log("시정요청");
        }


        scoreManager = GameObject.FindObjectOfType<ScoreManager>();

        if(scoreManager == null)
        {
            Debug.LogError("ScoreManager instance not found in the scene.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // //For Network Owner
        if(!IsOwner||!_uiManager.__StartGame__.GetComponent<StartGame>().IsGameStarted){
            return;
        }

        // PlayerState Separate
        if(playerState=="Fox")
        {
            iDown = Input.GetButtonDown("Interaction");
            if(!isAwaken.Value)
            {
                Interaction();
            }
            
            
        }
        else if(playerState=="Tiger")
        {
            //Tiger Penalty(from Tiger_Controller.cs)
            if (isPenaltyActive)
            {
                ApplyPenalty();
            }

            if (Input.GetKey(KeyCode.R)&&!_tiger_roaring.SkillCoolingTime) 
            {
                _tiger_roaring.Roar();
            }


            //Tiger UI update
            _uiManager.UpdateHeartUI();
        }
        else
        {

        }


        //유령 테스트
        // if(Input.GetKey(KeyCode.G)&&!isGhost)
        // {
        //     isGhost = true;
        //     Debug.Log("현재상태(클라) : " + isGhost);
        //     PlayerDieServerRpc(this.gameObject);
        // }



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
            _animator.SetBool("isRun", true);
        }
        else
        {
            run = false;
            _animator.SetBool("isRun", false);
        }

        isGrounded = _controller.isGrounded;

        // 현재 플레이어가 지면에 닿아있지 않는 문제 확인
        // Debug.Log($"isGrounded: {isGrounded}");  

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
        velocity.y += gravity * Time.deltaTime;
        _controller.Move(velocity * Time.deltaTime);
    }

    void LateUpdate()
    {

        if(!IsOwner){
            return;
        }

        if (!toggleCameraRotation && enabled)
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);

        }
    }

    void InputMovement()
    {
        finalSpeed = run ? runspeed : speed;

        // Vector3 forword = transform.TransformDirection(Vector3.forward);
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
        // Vector3 moveDirection = forword * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");

        _controller.Move(moveDirection.normalized * finalSpeed * Time.deltaTime);

        float percent = (run ? 1 : 0.5f) * moveDirection.magnitude;
        _animator.SetFloat("Blend",  percent,0.1f,Time.deltaTime);

        //player_animation
        BlendValue = percent;
    }

    void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            _animator.SetTrigger("Jump");

            if (playerState == "Fox")
            {
                //player_animation
                player_animation playerAnimation = FindObjectOfType<player_animation>();
                if (playerAnimation != null)
                {
                    playerAnimation.PlayJumpAnimation();
                }
            }
            else if (playerState == "Tiger")
            {
                //Tiger_animation
                Tiger_animation tigerAnimation = FindObjectOfType<Tiger_animation>();
                if (tigerAnimation != null)
                {
                    tigerAnimation.PlayJumpAnimation();
                }
            }
        }
    }


    //Tiger Penalty
    void ApplyPenalty()
    {
        switch (penaltyType)
        {
            case 1:
                //속도감소
                finalSpeed = run ? runspeed / 2 : speed / 2;
                break;
            case 2:
                // 방향 반전
                // Reverse controls handled in InputMovement
                break;
            case 3:
                //한쪽 가림
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

        if(!IsOwner)
        {
            return;
        }


        //여의주 먹기
        if(iDown && nearObject != null) //jdown 조건에서 잠깐 뺐음
        {
            if(nearObject.tag == "Bead")
            {

                PlayerNetworkStats.Instance.IncreaseBeadCountServerRpc(OwnerClientId);

                //Item.Instance.nearObject = nearObject;
                Item.Instance.DestroyBeadServerRpc(nearObject.GetComponent<NetworkObject>());
                
                // DestroyBeadServerRpc();
                Debug.Log("IsClient"+!IsHost);
                Debug.Log(nearObject);


                Debug.Log("여의주 먹음!");
                nearObject = null;
                
            }
            else if(nearObject.tag == "Food") //From HealthBar.cs
            {
                _healthBar.EatFood(5f);
                Item.Instance.DestroyBeadServerRpc(nearObject.GetComponent<NetworkObject>());

                //player_animation
                player_animation playerAnimation = FindObjectOfType<player_animation>();
                if (playerAnimation != null)
                {
                    playerAnimation.PlayEatAnimation();
                }
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

        if(!IsOwner)
        {
            return;
        }

        //여의주가 있는 영역을 벗어나면 여의주 없애기
        if (other.tag == "Bead")
            nearObject = null;
    }

    //이미호로 바뀌는 함수
private void ChangeModel()
{
    if (PlayerNetworkStats.Instance.BeadCount >= 2 && !isAwaken.Value && IsOwner)
    {
        Set_isAwakenServerRpc(true);
        Set_isAwakenClientRpc(true);

        _uiManager.isAwaken = true;

        GetComponent<AnimalTransform>().ChangeModelToSecFox();
        ChangeAllPlayerModelToSecFoxServerRpc();

        Debug.Log("Transformed to Fox");
        ToggleLights(false); 
        ActivatePlayerSpotLight(true); 
        ChangeSkybox(); 

        SetPlayerLightActiveServerRpc(true); 
        ActivateFarthestDogHoleServerRpc();
    }
}
private void ChangeSkybox()
{
    if (IsServer)
    {
        ChangeSkyboxOnServerRpc();
    }
    else
    {
        ChangeSkyboxOnServerRpc();
    }
}

[ServerRpc]
private void ChangeSkyboxOnServerRpc()
{
    if (newSkyboxMaterial != null)
    {
        RenderSettings.skybox = newSkyboxMaterial;
        DynamicGI.UpdateEnvironment();
        ChangeSkyboxClientRpc();
    }
    else
    {
        Debug.LogWarning("skybox 없음");
    }
}
[ClientRpc]
private void ChangeSkyboxClientRpc()
{
    if (newSkyboxMaterial != null)
    {
        RenderSettings.skybox = newSkyboxMaterial;
        DynamicGI.UpdateEnvironment();
    }
    else
    {
        Debug.LogWarning("skybox 없음");
    }
}
    //생명의 샘에 닿으면 모습을 바꿈
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Lake")
        {
            ChangeModel();
        }

        if (other.gameObject.name == "Girl" && playerState == "Fox" && isAwaken.Value && IsOwner)
        {
            AddScoreServerRpc("Fox", 1); // 여우가 소녀한테 닿으면 점수 1점 얻음
            EndGame(); 
        }
    }
private void ToggleLights(bool activatePlayerLight)
{
    Light[] allLights = FindObjectsOfType<Light>();

    if(!IsOwner)
    {
        return;
    }
    
    foreach (Light light in allLights)
    {
        // 플레이어의 Spot Light만 켜고 나머지 모든 빛은 꺼짐
        // if(IsOwner)
        // {
        //     bool isPlayerSpotLight = light.gameObject == spotLight;
        //     light.enabled = isPlayerSpotLight && activatePlayerLight;
        // }
        bool isPlayerSpotLight = light.gameObject == spotLight;
        light.enabled = isPlayerSpotLight && activatePlayerLight;
    }
}

private void ActivatePlayerSpotLight(bool isActive)
{
    if (spotLight != null)
    {
        spotLight.SetActive(isActive);
    }
}

[ClientRpc]
private void SyncLightStateClientRpc(bool activatePlayerLight)
{
    ToggleLights(activatePlayerLight);
}

[ServerRpc]
private void SetPlayerLightActiveServerRpc(bool isActive)
{
    SyncLightStateClientRpc(isActive);
}
[ServerRpc]
private void ActivateFarthestDogHoleServerRpc()
{
    if (!IsServer) return;

    Vector3 tigerPosition = Vector3.zero;
    foreach (var client in NetworkManager.Singleton.ConnectedClients)
    {
        var player = client.Value.PlayerObject.GetComponent<PlayerMovement>();
        if (player.playerStateSync.Value == "Tiger")
        {
            tigerPosition = player.transform.position;
            break;
        }
    }

    DogHole farthestHole = null;
    float maxDistance = float.MinValue;

    foreach (var hole in DogHole.allHoles)
    {
        float distance = Vector3.Distance(hole.transform.position, tigerPosition);
        if (distance > maxDistance)
        {
            maxDistance = distance;
            farthestHole = hole;
        }
    }

    if (farthestHole != null)
    {
        farthestHole.EnableHole();
        ActivateDogHoleClientRpc(farthestHole.gameObject);
    }
}

[ClientRpc]
private void ActivateDogHoleClientRpc(NetworkObjectReference holeRef)
{
    if (holeRef.TryGet(out NetworkObject holeObject))
    {
        holeObject.GetComponent<DogHole>().EnableHole();
    }
}

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

    [ServerRpc(RequireOwnership = false)]
    private void Set_isAwakenServerRpc(bool value)
    {
        isAwaken.Value = value;
    }

    [ClientRpc]
    private void Set_isAwakenClientRpc(bool value)
    {
        Set_isAwakenServerRpc(value);
    }

    [ServerRpc]
    public void Set_playerStateSyncServerRpc(FixedString128Bytes value)
    {
        playerStateSync.Value = value;
    }

    
    // [ClientRpc]
    // private void ChangeAllPlayerModelToSecFoxClientRpc()
    // {
    //     GetComponent<AnimalTransform>().ChangeModelToSecFox();
    //     Debug.Log("클라알피시");
    // }

    [ServerRpc]
    private void ChangeAllPlayerModelToSecFoxServerRpc()
    {
        //각 플레이어 정보 가져오기
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            NetworkObject playerNetworkObject = client.Value.PlayerObject;
            if (playerNetworkObject != null)
            {
                GameObject playerGameObject = playerNetworkObject.gameObject;

                //모델 바꾸는 명령어 실행

                if(playerGameObject.GetComponent<PlayerMovement>().playerStateSync.Value == "Fox" && !IsHost)
                {
                    playerGameObject.GetComponent<AnimalTransform>().ChangeModelToSecFox();
                }

                
            }
        }
    }


    //플레이어 죽음처리
    //현재 전부다 같이 죽는 버그가 있음 -> ServerRpc로 죽은 사람만 실행하게 변경.
    [ClientRpc]
    public void PlayerDieClientRpc()
    {
        Debug.Log($"Player die! : {OwnerClientId}, {name}");

        //player_animation
        player_animation playerAnimation = FindObjectOfType<player_animation>();
        if (playerAnimation != null)
        {
            playerAnimation.PlayDieAnimation();
        }

        // if(IsOwner)
        // {
        //     _uiManager.__EndGame__.GetComponent<EndGame>().GameOver();
        // }

        

        if(IsOwner)
        {
            _camera.enabled = false;
            _camera.gameObject.GetComponent<AudioListener>().enabled = false; 
        }
        // _camera.enabled = false;
        // _camera.gameObject.GetComponent<AudioListener>().enabled = false; 

        
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayerDieServerRpc(NetworkObjectReference player)
    {

        if(player.TryGet(out var p) == false)
        {
            return;
        }

        ProcessDieOnServer(p.GetComponent<PlayerMovement>());
    }

    public static void ProcessDieOnServer(PlayerMovement player)
    {
        player.PlayerDieClientRpc();
        
        if(player.playerStateSync.Value == "Fox")
        {
            Debug.Log("여우 지움");
            player.GetComponent<AnimalTransform>().PlayerNewModelDespawnServerRpc(player.gameObject,0);
        }
        else if(player.playerStateSync.Value == "Tiger")
        {
            Debug.Log("호랑이 지움");
            player.GetComponent<AnimalTransform>().PlayerNewModelDespawnServerRpc(player.gameObject,1);
        }

        player.GetComponent<AnimalTransform>().PlayerNewModelSpawnServerRpc(player.gameObject,2);
        Debug.Log("소환");

    }

    
}