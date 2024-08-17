using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

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
    public Text playerCountText;
    
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

        // 버튼 클릭 이벤트 설정
        if (createButton != null)
        {
            createButton.onClick.AddListener(CreateLobby);
        }
        if (joinButton != null)
        {
            joinButton.onClick.AddListener(OnJoinButtonClick);
        }
    }
    private void Update()
    {
        HandleLobbyHeartbeat();

        // 5초마다 로비 상태를 갱신하도록 설정
        updateLobbyStatusTimer -= Time.deltaTime;
        if (joinedLobby != null && updateLobbyStatusTimer <= 0f)
        {
            updateLobbyStatusTimer = 5f; // 5초로 조정
            UpdateLobbyStatusAsync(); // 비동기적으로 상태 갱신
        }
    }
    private async void UpdateLobbyStatusAsync()
{
    try
    {
        // 로비 상태를 비동기적으로 가져와 인원수 갱신
        joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
        
        UpdatePlayerCount();   // 인원 수 업데이트
        UpdateStartGameButton();  // 버튼 업데이트

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
                heartbeatTimer = 15f;
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
    Debug.Log("CreateLobby called");

    if (LobbyService.Instance == null || AuthenticationService.Instance == null)
    {
        Debug.LogError("LobbyService or AuthenticationService is not initialized.");
        return;
    }

    try
    {
        string lobbyName = "MyLobby";
        int maxPlayers = 4;  // 총 인원 수를 4명으로 설정

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

        // 인원수 업데이트
        UpdatePlayerCount();

        // 호스트일 때만 버튼 활성화
        UpdateStartGameButton();
    }
    catch (LobbyServiceException e)
    {
        Debug.LogError("Failed to create lobby: " + e);
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
        waitingRoomPanel.SetActive(true); // 대기실 UI 활성화
        lobbyCodeText.text = "Join Code: " + lobbyCode;

        // 인원수 업데이트
        UpdatePlayerCount();

        // 버튼 상태 업데이트
        UpdateStartGameButton();
    }
    catch (LobbyServiceException e)
    {
        Debug.Log(e);
    }
}




    public async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                    new QueryFilter(QueryFilter.FieldOptions.S1, "TigerAndFox", QueryFilter.OpOptions.EQ)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            Debug.Log("Lobbies found: " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Data["Gamemode"].Value);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void QuickJoinLobby()
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();
            Debug.Log("Joined Lobby");

            waitingRoomPanel.SetActive(true); // 대기실 UI 활성화
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
            UpdateStartGameButton(); // 버튼 상태 업데이트
            PrintPlayers(hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public GameObject startGameButtonObject; // 버튼이 포함된 GameObject를 드래그하여 할당

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

    // 현재 플레이어가 호스트인지 확인
    bool isHost = AuthenticationService.Instance.PlayerId == hostLobby.HostId;
    bool enoughPlayers = joinedLobby.Players.Count >= 2; // 2명 이상일 때 버튼 활성화

    // 호스트일 때만 버튼 활성화
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

                // 게임 시작 처리를 위한 추가 로직 필요
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
        // 로비 코드 입력 필드의 텍스트를 가져와서 로비에 참가합니다.
        string lobbyCode = lobbyCodeInputField.text; // 입력 필드의 텍스트를 사용합니다.
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