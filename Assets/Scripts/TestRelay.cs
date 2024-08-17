using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public Transform lobbyListContent; // 로비 목록
    public GameObject lobbyListItemPrefab; // 로비 목록
    private bool isCreatingLobby = false;

    
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

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            Debug.Log("Lobbies found: " + queryResponse.Results.Count);

            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers + " " + (lobby.Data.ContainsKey("Gamemode") ? lobby.Data["Gamemode"].Value : "No Gamemode"));

                GameObject item = Instantiate(lobbyListItemPrefab, lobbyListContent);
                Text[] texts = item.GetComponentsInChildren<Text>();
                if (texts.Length > 0)
                {
                    texts[0].text = lobby.Name; 
                    if (texts.Length > 1)
                    {
                        texts[1].text = $"{lobby.Players.Count}/{lobby.MaxPlayers}"; 
                    }
                }

                Button joinButton = item.GetComponentInChildren<Button>();
                if (joinButton != null)
                {
                    string lobbyCode = lobby.LobbyCode;
                    joinButton.onClick.AddListener(() => JoinLobbyByCode(lobbyCode));
                }
            }

        lobbyListPanel.SetActive(true); 
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
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