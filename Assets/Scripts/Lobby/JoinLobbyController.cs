using UnityEngine;
using UnityEngine.UI;

public class joinLobbyController : MonoBehaviour
{
    public Button joinLobbyButton; 
    public JoinLobbyManager joinLobbyManager; 

    private void Start()
    {
        joinLobbyButton.onClick.AddListener(() => joinLobbyManager.ShowJoinLobbyPanel());
    }
}
