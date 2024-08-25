using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class PlayerCounterNetwork : NetworkBehaviour
{
    private Text _text;

    public GameObject _UIManager;

    private NetworkVariable<int> playerCount = new NetworkVariable<int>(0);

    int local_playerCount=0;

    void Start()
    {
        _text = GetComponent<Text>();
    }

    [ServerRpc(RequireOwnership=false)]
    public void ChangePlayerCountTextServerRpc()
    {
        playerCount.Value += 1;
        ChangePlayerCountTextClientRpc();
    }

    [ClientRpc]
    public void ChangePlayerCountTextClientRpc()
    {

        
        local_playerCount = playerCount.Value+1;
        if(IsHost)
        {
            local_playerCount--;
        }
        
        _text.text = $"PC : {local_playerCount}/5";

        if(IsHost && playerCount.Value>1)
        {
            _UIManager.GetComponent<UIManager>().GameStartButton.SetActive(true);
        }
    }
}
