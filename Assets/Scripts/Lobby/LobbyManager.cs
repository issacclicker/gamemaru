using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public Button createLobbyButton;
    public Button joinLobbyButton;
    public Button quickJoinButton;
    private bool isCreatingLobby = false;

    private async void Start()
    {
        // Relay 초기화
        await Unity.Services.Core.UnityServices.InitializeAsync();

        createLobbyButton.onClick.AddListener(CreateLobby);
        joinLobbyButton.onClick.AddListener(JoinLobby);
        quickJoinButton.onClick.AddListener(QuickJoin);
    }

    public async void CreateLobby()
    {
        if (isCreatingLobby) return; 
        isCreatingLobby = true;

        Debug.Log("CreateLobby 호출됨");

        int maxConnections = 4; // 최대 4명의 플레이어가 접속 가능
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        // 조인 코드를 전역 변수에 저장
        LobbyInfo.JoinCode = joinCode;

        // 호스트 시작
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData
        );
        NetworkManager.Singleton.StartHost();

        Debug.Log("Lobby created with join code: " + joinCode);
        SceneManager.LoadScene("WaitingRoomScene", LoadSceneMode.Single);
    }

    public async void JoinLobby()
{
    string joinCode = ""; // Join Code를 UI에서 받아오도록 설정

    if (string.IsNullOrWhiteSpace(joinCode))
    {
        Debug.LogError("Join Code cannot be null or empty.");
        return;
    }

    try
    {
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(
            joinAllocation.RelayServer.IpV4,
            (ushort)joinAllocation.RelayServer.Port,  
            joinAllocation.AllocationIdBytes,
            joinAllocation.Key,
            joinAllocation.ConnectionData,
            joinAllocation.HostConnectionData
        );
        NetworkManager.Singleton.StartClient();

        Debug.Log("Joined lobby with join code: " + joinCode);

        // 대기실 씬으로 전환
        SceneManager.LoadScene("WaitingRoomScene", LoadSceneMode.Single);
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Failed to join lobby: {e.Message}");
    }
}

    public void QuickJoin()
    {
        Debug.Log("Quick Join is not implemented yet.");
    }
}
