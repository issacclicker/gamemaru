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
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private string playerName;
    private async void Start(){
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        playerName = "Issac" + UnityEngine.Random.Range(10, 99);
        Debug.Log("플레이어 : " + playerName);
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
                IsPrivate = false, //공개/비공개 로비 여부
                Player = GetPlayer(), //플레이어
                Data = new Dictionary<string, DataObject>{
                    { "Gamemode", new DataObject(DataObject.VisibilityOptions.Public, "TigerAndFox")} //게임 모드
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers,createLobbyOptions);
            
            hostLobby = lobby;
            joinedLobby = hostLobby;

            Debug.Log("Created Lobby ! " + lobby.Name + " " + lobby.MaxPlayers +" "+ lobby.Id + " " + lobby.LobbyCode);
        
            PrintPlayers(hostLobby);
        }
        catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }


    public async void ListLobbies(){ //로비 리스트 가져오는 코드

        try{
            QueryLobbiesOptions queryLobbiesOptions= new QueryLobbiesOptions { //플레이어에게 보이는 로비 조건
                Count = 25,
                Filters = new List<QueryFilter>{
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots,"0",QueryFilter.OpOptions.GT), //플레이어 수 꽉차면 로비 안보이게함
                    new QueryFilter(QueryFilter.FieldOptions.S1,"TigerAndFox",QueryFilter.OpOptions.EQ) //TigerAndFox 게임모드만 보이게 함
                },
                Order = new List<QueryOrder>{
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            
            Debug.Log("Lobbies found : " + queryResponse.Results.Count);
            foreach(Lobby lobby in queryResponse.Results){
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Data["Gamemode"].Value);
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

    public async void JoinLobbyByCode(string lobbyCode){//코드로 로비 참가
        try{
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions{
                Player = GetPlayer()
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            joinedLobby = lobby;

            Debug.Log("Joined Lobby Code : " + lobbyCode);

            PrintPlayers(lobby);
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    public async void QuickJoinLobby(){ //빠른 참가

        try{
            await LobbyService.Instance.QuickJoinLobbyAsync();

            Debug.Log("Joined Lobby Code : ");
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }


    private Player GetPlayer(){
        return new Player{
            Data = new Dictionary<string, PlayerDataObject>{
                {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
            }
        };
    }


    public void PrintPlayers(){
        PrintPlayers(joinedLobby);
    }
    public void PrintPlayers(Lobby lobby){
        Debug.Log("Players in Lobby " + lobby.Name + " " + lobby.Data["Gamemode"].Value);
        foreach(Player player in lobby.Players){
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }


    public async void UpdateLobbyGameMode(string gameMode){ //게임모드 바꾸는 함수

        try{
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions{
                Data = new Dictionary<string, DataObject>{
                    {"GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode)}
                }
            });
            joinedLobby = hostLobby;

            PrintPlayers(hostLobby);
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
        
    }
}
