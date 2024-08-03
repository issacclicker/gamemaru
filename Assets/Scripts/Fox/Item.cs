using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Item : NetworkBehaviour
{
    [SerializeField] private GameObject Bead;

    [SerializeField] private Transform beadTransForm;

    void Start()
    {
        if(!IsOwner)
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
}
