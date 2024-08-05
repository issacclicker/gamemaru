using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Item : NetworkBehaviour
{

    public static Item Instance;

    public GameObject nearObject;

    [SerializeField] private GameObject Bead;

    [SerializeField] private Transform beadTransForm;


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
        GameObject bd = Instantiate(Bead, beadTransForm.position, beadTransForm.rotation);
        bd.GetComponent<NetworkObject>().Spawn();
    }

    
    [ServerRpc(RequireOwnership = false)]
    public void DestroyBeadServerRpc()
    {
        nearObject.GetComponent<NetworkObject>().Despawn();
    }

}
