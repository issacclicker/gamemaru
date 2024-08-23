using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine.SceneManagement;
//역할 할당 및 scene 전환

public class GameStateManager : NetworkBehaviour
{
    public static GameStateManager Instance;

    public NetworkVariable<FixedString128Bytes> playerRole = new NetworkVariable<FixedString128Bytes>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerRole(string role)
    {
        playerRole.Value = new FixedString128Bytes(role);
    }

    public string GetPlayerRole()
    {
        return playerRole.Value.ToString();
    }

    public void AssignRolesAndStartGame()
    {
        if (IsHost)
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                var playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(client.ClientId);
                if (playerObject != null)
                {
                    string assignedRole = (UnityEngine.Random.value > 0.5f) ? "Fox" : "Tiger";
                    playerObject.GetComponent<Player>().SetPlayerRoleClientRpc(assignedRole);
                }
            }

            NetworkManager.Singleton.SceneManager.LoadScene("gamescene", LoadSceneMode.Single);
        }
    }

    [ClientRpc]
    public void SetPlayerRoleClientRpc(string role)
    {
        SetPlayerRole(role);
    }
}
