using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerStateSync : NetworkBehaviour
{
    public static PlayerStateSync Instance;
    void Awake()
    {
        Instance = this;
    }

    [ServerRpc(RequireOwnership=false)]
    public void SetTrueIsAwakenServerRpc()
    {
        PlayerMovement.Instance.isAwaken.Value = true;
        Debug.Log("작동");
    }
}