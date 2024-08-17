using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class JoinLobbyManager : MonoBehaviour
{
    public GameObject joinLobbyPanel;
    public InputField joinCodeInputField; // 코드 입력
    public Button submitCodeButton;

    private void Start()
    {
        submitCodeButton.onClick.AddListener(OnJoinButtonClicked);

        joinLobbyPanel.SetActive(false);
    }

    public void ShowJoinLobbyPanel()
    {
        joinLobbyPanel.SetActive(true);
    }

     private async void OnJoinButtonClicked()
    {
        string joinCode = joinCodeInputField.text.Trim(); // 입력 필드에서 조인 코드 가져오기

        if (string.IsNullOrEmpty(joinCode))
        {
            Debug.LogError("Join Code cannot be null or empty.");
            return;
        }

        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            if (transport == null)
            {
                Debug.LogError("UnityTransport component not found on NetworkManager.");
                return;
            }

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
            UnityEngine.SceneManagement.SceneManager.LoadScene("WaitingRoomScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to join lobby: {e.Message}");
        }
    }
}
