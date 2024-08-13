using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Item : NetworkBehaviour
{

    public static Item Instance;

    public GameObject nearObject;

    //여의주 모델 오브젝트
    [SerializeField] private GameObject Bead;

    //여의주가 소환될 위치
    [SerializeField] private Transform[] beadTransForm;

    //음식 모델 오브젝트
    [SerializeField] private GameObject Food;

    //음식이 소환될 위치
    [SerializeField] private Transform[] foodTransForm;


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

        for(int i=0;i<foodTransForm.Length;i++)
        {
            GameObject fd = Instantiate(Food, foodTransForm[i].position, foodTransForm[i].rotation);
            fd.GetComponent<NetworkObject>().Spawn();
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
