using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;

public class TestRelay : MonoBehaviour
{
    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private string playerName;
    private float updateLobbyStatusTimer;

    // UI 요소
    public GameObject waitingRoomPanel;
    public Button startGameButton;
    public Text lobbyCodeText;
    public InputField lobbyCodeInputField; // 로비 코드 입력 필드
    public Button joinButton; // Join 버튼
    public Button createButton; // Create 버튼
    public Button listLobbiesButton; // 로비 목록 버튼
    public Text playerCountText;
    public GameObject lobbyListPanel; // 로비 목록
    private bool isCreatingLobby = false;
    [SerializeField] private GameObject lobbyListItemPrefab;
    [SerializeField] private Transform lobbyListContent;

    
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        playerName = "Player" + UnityEngine.Random.Range(10, 99);
        Debug.Log("플레이어 : " + playerName);

        if (createButton != null)
    {
        createButton.onClick.RemoveAllListeners();
        createButton.onClick.AddListener(CreateLobby);
    }
        if (joinButton != null)
        {
            joinButton.onClick.AddListener(OnJoinButtonClick);
        }
        if (listLobbiesButton != null)
        {
            listLobbiesButton.onClick.AddListener(ListLobbies);
        }
        if (lobbyListItemPrefab == null)
    {
        Debug.LogError("lobbyListItemPrefab is not assigned in the Inspector.");
    }

    if (lobbyListContent == null)
    {
        Debug.LogError("lobbyListContent is not assigned in the Inspector.");
    }
    }
    private void Update()
    {
        HandleLobbyHeartbeat();
        updateLobbyStatusTimer -= Time.deltaTime;
        if (joinedLobby != null && updateLobbyStatusTimer <= 0f)
        {
            updateLobbyStatusTimer = 5f; 
            UpdateLobbyStatusAsync(); 
        }
        
    }
    private async void UpdateLobbyStatusAsync()
    {
        try
        {
            joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

            if (joinedLobby.Data.ContainsKey("GameStarted") && joinedLobby.Data["GameStarted"].Value == "true")
            {
                SceneManager.LoadScene("Multiplayer Test 1");
            }

            UpdatePlayerCount();   
            UpdateStartGameButton(); 
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private void UpdatePlayerCount()
    {
        if (joinedLobby != null && playerCountText != null)
        {
            int currentPlayers = joinedLobby.Players.Count;
            int maxPlayers = joinedLobby.MaxPlayers;
            playerCountText.text = $"{currentPlayers}/{maxPlayers}";
        }
    }

    private async void HandleLobbyHeartbeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                heartbeatTimer = 30f;
                try
                {
                    await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
            }
        }
    }

   public async void CreateLobby()
    {
        if (isCreatingLobby) return;

        isCreatingLobby = true;
        Debug.Log("CreateLobby called");

        if (LobbyService.Instance == null || AuthenticationService.Instance == null)
        {
            Debug.LogError("LobbyService or AuthenticationService is not initialized.");
            return;
        }

        try
        {
            string lobbyName = "MyLobby";
            int maxPlayers = 4;  //일단 최대 인원 4명으로 설정

            var player = GetPlayer();
            if (player == null)
            {
                Debug.LogError("Player is null.");
                return;
            }

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = player,
                Data = new Dictionary<string, DataObject>
                {
                    { "Gamemode", new DataObject(DataObject.VisibilityOptions.Public, "TigerAndFox") }, 
                    { "GameStarted", new DataObject(DataObject.VisibilityOptions.Public, "false") }
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
            if (lobby == null)
            {
                Debug.LogError("Lobby creation failed.");
                return;
            }

            hostLobby = lobby;
            joinedLobby = hostLobby;

            Debug.Log("Created Lobby! " + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);
            if (lobbyCodeText != null)
            {
                lobbyCodeText.text = "Join Code: " + lobby.LobbyCode;
            }
            else
            {
                Debug.LogError("lobbyCodeText is not assigned.");
            }

            if (waitingRoomPanel != null)
            {
                waitingRoomPanel.SetActive(true);
            }
            else
            {
                Debug.LogError("waitingRoomPanel is not assigned.");
            }

            UpdatePlayerCount();

            UpdateStartGameButton();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Failed to create lobby: " + e);
        }
        finally
        {
            isCreatingLobby = false; 
        }
    }

   public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            joinedLobby = lobby;

            Debug.Log("Joined Lobby Code: " + lobbyCode);
            waitingRoomPanel.SetActive(true); 
            lobbyCodeText.text = "Join Code: " + lobbyCode;

            UpdatePlayerCount();

            UpdateStartGameButton();
        }
        catch (LobbyServiceException e)
        {   
            Debug.Log(e);
        }
    }

public async void ListLobbies()
{
    if (lobbyListPanel == null || lobbyListContent == null || lobbyListItemPrefab == null)
    {
        Debug.LogError("Lobby UI elements are not assigned.");
        return;
    }

    // 기존 목록을 지우기
    foreach (Transform child in lobbyListContent)
    {
        Destroy(child.gameObject);
    }

    try
    {
        QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
        {
            Count = 25,
            Filters = new List<QueryFilter>
            {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            },
            Order = new List<QueryOrder>
            {
                new QueryOrder(false, QueryOrder.FieldOptions.Created)
            }
        };

        // 로비를 가져올 때 재시도 로직 적용
        QueryResponse queryResponse = await ExecuteWithRetryAsync(
            () => Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions)
        );

        Debug.Log("Lobbies found: " + queryResponse.Results.Count);

        // 로비 리스트를 UI에 표시
        foreach (Lobby lobby in queryResponse.Results)
        {
            Debug.Log("Lobby Code: " + lobby.LobbyCode + " " + lobby.MaxPlayers + " " + (lobby.Data.ContainsKey("Gamemode") ? lobby.Data["Gamemode"].Value : "No Gamemode"));

            // Prefab을 인스턴스화
            GameObject item = Instantiate(lobbyListItemPrefab, lobbyListContent);
            
            // 텍스트와 버튼 컴포넌트를 찾기
            Text[] texts = item.GetComponentsInChildren<Text>();
            Button joinButton = item.GetComponentInChildren<Button>();

            if (texts.Length > 0)
            {
                // 로비 코드를 텍스트로 설정
                texts[0].text = lobby.LobbyCode; 
                if (texts.Length > 1)
                {
                    // 플레이어 수 설정
                    texts[1].text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
                }
            }

            if (joinButton != null)
            {
                string lobbyCode = lobby.LobbyCode;
                joinButton.onClick.RemoveAllListeners(); // 기존 리스너 제거
                joinButton.onClick.AddListener(() => JoinLobbyByCode(lobbyCode));
            }
        }

        lobbyListPanel.SetActive(true);
    }
    catch (LobbyServiceException e)
    {
        Debug.LogError("Error querying lobbies: " + e);
    }
}
private async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> func, int retryCount = 3)
    {
        for (int i = 0; i < retryCount; i++)
        {
            try
            {
                return await func();
            }
            catch (LobbyServiceException e)
            {
                if (i == retryCount - 1) // 마지막 재시도에서 실패 시 에러를 던짐
                {
                    throw;
                }
                Debug.LogWarning($"Retry {i + 1} failed. Exception: {e.Message}");
                await Task.Delay(2000); // 재시도 간의 대기 시간 (2초)
            }
        }
        return default; // 반환값이 있는 경우 기본값 반환
    }
    public async void QuickJoinLobby()
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();
            Debug.Log("Joined Lobby");

            waitingRoomPanel.SetActive(true); 
            UpdateStartGameButton();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
            }
        };
    }

    public void PrintPlayers()
    {
        PrintPlayers(joinedLobby);
    }

    public void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Players in Lobby " + lobby.Name + " " + lobby.Data["Gamemode"].Value);
        foreach (Player player in lobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }

    public async void UpdateLobbyGameMode(string gameMode)
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "Gamemode", new DataObject(DataObject.VisibilityOptions.Public, gameMode) }
                }
            });
            joinedLobby = hostLobby;
            UpdateStartGameButton(); 
            PrintPlayers(hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public GameObject startGameButtonObject; 

    private void UpdateStartGameButton()
    {
        if (startGameButton == null)
        {
            Debug.LogError("Start Game Button is not assigned.");
            return;
        }

        if (hostLobby == null || joinedLobby == null)
        {
            Debug.LogError("hostLobby or joinedLobby is not assigned.");
            startGameButton.gameObject.SetActive(false);
            return;
        }

        bool isHost = AuthenticationService.Instance.PlayerId == hostLobby.HostId;
        bool enoughPlayers = joinedLobby.Players.Count >= 2; //2명 이상일 때 start game 버튼 나옴

        startGameButton.gameObject.SetActive(isHost && enoughPlayers);
    }



    public async void StartGame()
    {
        if (hostLobby != null)
        {
            try
            {
                await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { "GameStarted", new DataObject(DataObject.VisibilityOptions.Public, "true") }
                    }
                });

                SceneManager.LoadScene("Multiplayer Test 1");

                Debug.Log("게임이 시작되었습니다.");
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public void OnJoinButtonClick()
    {
        string lobbyCode = lobbyCodeInputField.text; 
        if (!string.IsNullOrEmpty(lobbyCode))
        {
            JoinLobbyByCode(lobbyCode);
        }
        else
        {
            Debug.LogError("Lobby code is empty.");
        }
    }
}