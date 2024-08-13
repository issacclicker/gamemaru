using Unity.Netcode;
using UnityEngine;

public class LobbyPlayer : NetworkBehaviour
{
    public NetworkVariable<bool> isReady = new NetworkVariable<bool>(false);

    public void SetReadyStatus(bool ready)
    {
        if (IsClient && IsOwner)
        {
            isReady.Value = ready;
            CmdCheckReadyToStartGameServerRpc(); 
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void CmdCheckReadyToStartGameServerRpc()
    {
        if (NetworkManager.Singleton.ConnectedClients.Count >= 2)
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (!client.PlayerObject.GetComponent<LobbyPlayer>().isReady.Value)
                {
                    return; 
                }
            }

            NetworkManager.Singleton.SceneManager.LoadScene("multiplayer test 1", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
