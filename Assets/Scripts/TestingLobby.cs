using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TestingLobby : MonoBehaviour
{

    private Lobby hostLobby;
    private float heartbeatTimer;
    private async void Start(){
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }


    private void Update(){
        // HandleLobbyHeartbeat();
    }
    private async void HandleLobbyHeartbeat(){ // 로비 lifespan 안 끝나게 해주는 코드
        if(hostLobby!=null){
            heartbeatTimer -= Time.deltaTime;
            if(heartbeatTimer<0f){
                float heartbeatTimerMax = 15;
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    public async void CreateLobby(){ //로비 생성
        try{
            string lobbyName = "MyLobby";
            int maxPlayers = 2;

            CreateLobbyOptions createLobbyOptions= new CreateLobbyOptions{
                IsPrivate = true;//여기서 부터 이어서 하기
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
            
            hostLobby = lobby;
            Debug.Log("Created Lobby ! " + lobby.Name + " " + lobby.MaxPlayers);
        }
        catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }


    public async void ListLobbies(){ //로비 리스트 가져오는 코드

        try{
            QueryLobbiesOptions queryLobbiesOptions= new QueryLobbiesOptions { //플레이어 수 꽉차면 로비 안보이게함
                Count = 25,
                Filters = new List<QueryFilter>{
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots,"0",QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>{
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            
            Debug.Log("Lobbies found : " + queryResponse.Results.Count);
            foreach(Lobby lobby in queryResponse.Results){
            Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            } 
        }catch (LobbyServiceException e){
            Debug.Log(e);
        }
       
    }

    public async void JoinLobby(){ //로비 참가 하는 코드
        
        try{
            
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            
            await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
            Debug.Log("참가 성공");
        }catch (LobbyServiceException e){
            Debug.Log(e);
        }

        
    }
}
