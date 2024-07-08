using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class lLobby : MonoBehaviour
{
    public static lLobby Instance {get; private set;}

    private Lobby joinedLobby;

    private void Awake(){
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        InitializeUnityAuthentication();
    }

    private async void InitializeUnityAuthentication(){
        if(UnityServices.State != ServicesInitializationState.Initialized){
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(Random.Range(0,1000).ToString());
            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

    }

    public async void CreateLobby(string lobbyName,bool isPrivate){
        try{
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName,4,new CreateLobbyOptions{
            IsPrivate = isPrivate,
            });

            Debug.Log("HOST");
            NetworkManager.Singleton.StartHost();
            Loader.LoadNetwork(Loader.Scene.netcode_test2);

        } catch(LobbyServiceException e){
            Debug.Log(e);
        }
        
        
    }

    public async void QuickJoin(){
        try{
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            
            Debug.Log("CLIENT");
            NetworkManager.Singleton.StartClient();
            
        
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
        
    }
}
