using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class PlayerCounterNetwork : NetworkBehaviour
{
    private Text _text;

    private NetworkVariable<int> playerCount = new NetworkVariable<int>(0);

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
        _text.text = $"PC : {playerCount.Value}/5";
    }
}
