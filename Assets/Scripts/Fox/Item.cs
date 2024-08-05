using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Item : NetworkBehaviour
{

    public static Item Instance;

    public GameObject nearObject;

    [SerializeField] private GameObject Bead;

    [SerializeField] private Transform[] beadTransForm;


    private void Awake()
    {
        Instance = this;
        
    }

    void Start()
    {
        

        if(!IsHost || !IsOwner)
        {
            return;
        }

        MakeBeadServerRpc();
    }

    [ServerRpc]
    private void MakeBeadServerRpc()
    {
        for(int i=0;i<beadTransForm.Length;i++)
        {
            GameObject bd = Instantiate(Bead, beadTransForm[i].position, beadTransForm[i].rotation);
            bd.GetComponent<NetworkObject>().Spawn();
        }

        // GameObject bd = Instantiate(Bead, beadTransForm.position, beadTransForm.rotation);
        // bd.GetComponent<NetworkObject>().Spawn();
    }

    
    [ServerRpc(RequireOwnership = false)]
    public void DestroyBeadServerRpc(NetworkObjectReference target)
    {
        //nearObject.GetComponent<NetworkObject>().Despawn();
        if (target.TryGet(out var obj))
            obj.Despawn();
    }

}
