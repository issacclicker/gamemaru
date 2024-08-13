using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameLobby : NetworkBehaviour
{
    public void StartGame()
    {
        if (IsServer)
        {
            NetworkManager.SceneManager.LoadScene("multiplayer test", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
