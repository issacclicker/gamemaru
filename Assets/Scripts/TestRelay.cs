using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class TestRelay : MonoBehaviour
{
    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float heartbeatTimer = 30f;
    private string playerName;
    private float updateLobbyStatusTimer = 5f;

    // UI 요소
    public GameObject waitingRoomPanel;
    public Button startGameButton;
    public Text lobbyCodeText;
    public InputField lobbyCodeInputField;
    public Button joinButton;
    public Button createButton;
    public Button listLobbiesButton;
    public Text playerCountText;
    public GameObject lobbyListPanel;
    private bool isCreatingLobby = false;
    [SerializeField] private GameObject lobbyListItemPrefab;
    [SerializeField] private Transform lobbyListContent;

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        playerName = "Player" + UnityEngine.Random.Range(10, 99);
        Debug.Log("플레이어 : " + playerName);

        if (createButton != null)
        {
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
        if (lobbyListItemPrefab == null || lobbyListContent == null)
        {
            Debug.LogError("Lobby UI elements are not assigned.");
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
            if (heartbeatTimer <= 0f)
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

    // 로비 만들기
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
            int maxPlayers = 4;  // 최대 4명으로 일단 설정해놓음

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

            if (waitingRoomPanel != null)
            {
                waitingRoomPanel.SetActive(true);
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

    //코드 입력해서 들어가기
    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };
            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            joinedLobby = lobby;

            Debug.Log("Joined Lobby Code: " + lobbyCode);
            waitingRoomPanel.SetActive(true); 
            lobbyCodeText.text = "Join Code: " + lobbyCode;

            UpdatePlayerCount();
            UpdateStartGameButton();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Failed to join lobby: " + e);
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

            QueryResponse queryResponse = await ExecuteWithRetryAsync(
                () => LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions)
            );

            Debug.Log("Lobbies found: " + queryResponse.Results.Count);

            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log("Lobby Code: " + lobby.LobbyCode + " " + lobby.MaxPlayers + " " + (lobby.Data.ContainsKey("Gamemode") ? lobby.Data["Gamemode"].Value : "No Gamemode"));

                GameObject item = Instantiate(lobbyListItemPrefab, lobbyListContent);
                
                Text[] texts = item.GetComponentsInChildren<Text>();
                Button joinButton = item.GetComponentInChildren<Button>();

                if (texts.Length > 0)
                {
                    texts[0].text = lobby.LobbyCode; 
                    if (texts.Length > 1)
                    {
                        texts[1].text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
                    }
                }

                if (joinButton != null)
                {
                    string lobbyCode = lobby.LobbyCode;
                    joinButton.onClick.RemoveAllListeners(); 
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
                if (i == retryCount - 1) 
                {
                    throw;
                }
                Debug.LogWarning($"Retry {i + 1} failed. Exception: {e.Message}");
                await Task.Delay(2000); 
            }
        }
        return default; 
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
            Debug.LogError("Failed to quick join lobby: " + e);
        }
    }

    private Unity.Services.Lobbies.Models.Player GetPlayer()
    {
        return new Unity.Services.Lobbies.Models.Player
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
        if (lobby == null) return;

        Debug.Log("Players in Lobby " + lobby.Name + " " + (lobby.Data.ContainsKey("Gamemode") ? lobby.Data["Gamemode"].Value : "No Gamemode"));
        foreach (Unity.Services.Lobbies.Models.Player player in lobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }

    public async void UpdateLobbyGameMode(string gameMode)
    {
        if (hostLobby == null) return;

        try
        {
            hostLobby = await LobbyService.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
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
            Debug.LogError("Failed to update lobby game mode: " + e);
        }
    }

    public void UpdateStartGameButton()
    {
        if (startGameButton == null)
        {
            Debug.LogError("Start Game Button is not assigned.");
            return;
        }

        if (hostLobby == null || joinedLobby == null)
        {
            startGameButton.gameObject.SetActive(false);
            return;
        }

        bool isHost = AuthenticationService.Instance.PlayerId == hostLobby.HostId;
        bool enoughPlayers = joinedLobby.Players.Count >= 2; 

        startGameButton.gameObject.SetActive(isHost && enoughPlayers);
    }

public async void StartGame()
{
    if (hostLobby != null)
    {
        try
        {
            await LobbyService.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "GameStarted", new DataObject(DataObject.VisibilityOptions.Public, "true") }
                }
            });

            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.AssignRolesAndStartGame();
            }
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
