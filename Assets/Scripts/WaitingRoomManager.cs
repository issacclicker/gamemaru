using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class WaitingRoomManager : MonoBehaviour
{
    public Button startGameButton;

    private void Start()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            startGameButton.gameObject.SetActive(true);
            startGameButton.onClick.AddListener(OnStartGame);
        }
        else
        {
            startGameButton.gameObject.SetActive(false);
        }
    }

    public void OnStartGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            CheckReadyToStartGame();
        }
    }

    private void CheckReadyToStartGame()
    {
        if (NetworkManager.Singleton.ConnectedClients.Count >= 2) 
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (!client.PlayerObject.GetComponent<LobbyPlayer>().isReady.Value)
                {
                    Debug.Log("모든 플레이어가 준비되지 않았습니다.");
                    return;
                }
            }

            NetworkManager.Singleton.SceneManager.LoadScene("multiplayer test", LoadSceneMode.Single);
        }
    }
}
