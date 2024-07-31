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

    public NetworkVariable<bool> isActive = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {

        if(!IsOwner)
        {
            return;
        }

        if(!isActive.Value && meshRenderer.enabled)
        {
            meshRenderer.enabled = false;
            Debug.Log("비활성화");
        }

        
    }

    
}
