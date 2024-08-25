using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class StartGame : NetworkBehaviour
{

    ulong lastClientId;

    public bool IsGameStarted=false;

    [SerializeField]private GameObject[] LobbyUI;

    public int Tiger_Client_ID;

    //게임 시작
    public void _StartGame_()
    {
        if(!IsServer)
        {
            Debug.Log("서버 아님");
            return;
        }
        GetLastID();
        int lastClientIdINT = (int)lastClientId;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Tiger_Client_ID = Random.Range(0,lastClientIdINT+1);
        DisableLobbyUIsClientRpc();
    }

    public void GetLastID()
    {
        // ConnectedClientsList를 사용하여 모든 클라이언트 ID를 가져오기
        List<ulong> clientIds = new List<ulong>();
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            clientIds.Add(client.ClientId);
            lastClientId = client.ClientId;
        }   
    }



    private void DisableLobbyUIs()
    {
        foreach(var obj in LobbyUI)
        {
            obj.SetActive(false);
        }
    }

    [ClientRpc]
    private void DisableLobbyUIsClientRpc()
    {
        DisableLobbyUIs();
        IsGameStarted = true;
    }

}
