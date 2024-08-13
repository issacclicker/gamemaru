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
        string joinCode = joinCodeInputField.text;

        if (!string.IsNullOrEmpty(joinCode))
        {
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

                joinLobbyPanel.SetActive(false);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to join lobby: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning("Join code cannot be empty.");
        }
    }
}
