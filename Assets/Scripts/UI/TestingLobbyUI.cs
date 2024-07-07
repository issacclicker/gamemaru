using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingLobbyUI : MonoBehaviour
{
    [SerializeField] private Button createGameBtn;
    [SerializeField] private Button joinGameBtn;

    private void Awake(){
        createGameBtn.onClick.AddListener(() =>{
            Debug.Log("HOST");
            NetworkManager.Singleton.StartHost();
            Loader.LoadNetwork(Loader.Scene.netcode_test2);
        });

        joinGameBtn.onClick.AddListener(()=>{
            Debug.Log("CLIENT");
            NetworkManager.Singleton.StartClient();
            
        });
    }
}
