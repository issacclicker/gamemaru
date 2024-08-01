using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Item : NetworkBehaviour
{
    public enum Type { Bead };
    public Type type;
    public int value;

    private MeshRenderer meshRenderer;

    public NetworkVariable<bool> isActive = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void OnDestroy()
    {
        isActive.Value = false;
    }

    void Update()
    {
        if(!IsOwner)
        {
            return;
        }

        if (!isActive.Value)
        {
            // 클라이언트에서 직접 값을 변경하는 대신 서버에 요청
            UpdateIsActiveServerRpc(false);
        }
    }

    [ServerRpc]
    void UpdateIsActiveServerRpc(bool newValue)
    {
        isActive.Value = newValue;
        Debug.Log("비활성화");
    }
}
