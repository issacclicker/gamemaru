using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class WaitingRoomManager : MonoBehaviour
{
    public Button startGameButton;
    public Text joinCodeText;  // Join Code를 화면에 표시하기 위한 Text

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

        // 대기실에 조인 코드를 표시합니다.
        joinCodeText.text = "Join Code: " + LobbyInfo.JoinCode;
    }

    public void OnStartGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            CheckReadyToStartGame();
        }
    }

    public void CheckReadyToStartGame()
    {
        // 최소 플레이어 수 확인
        if (NetworkManager.Singleton.ConnectedClients.Count >= 1)
        {
            bool allPlayersReady = true;

            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                var lobbyPlayer = client.PlayerObject.GetComponent<LobbyPlayer>();
                if (lobbyPlayer == null || !lobbyPlayer.isReady.Value)
                {
                    allPlayersReady = false;
                    break;
                }
            }

            if (allPlayersReady)
            {
                NetworkManager.Singleton.SceneManager.LoadScene("multiplayer test", UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
            else
            {
                Debug.Log("모든 플레이어가 준비되지 않았습니다.");
            }
        }
        else
        {
            Debug.Log("플레이어 수가 부족합니다.");
        }
    }
}
