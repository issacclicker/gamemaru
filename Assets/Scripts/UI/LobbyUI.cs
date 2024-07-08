using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    
    [SerializeField] private Button quickJoinButton;

    private void Awake(){
        mainMenuButton.onClick.AddListener(() => { 
            Loader.Load(Loader.Scene.LobbyScene);
        });
        createLobbyButton.onClick.AddListener(() => { 
            lLobby.Instance.CreateLobby("LobbyName",false);
        });
        mainMenuButton.onClick.AddListener(() => { 
            lLobby.Instance.QuickJoin();
        });
    }

}
