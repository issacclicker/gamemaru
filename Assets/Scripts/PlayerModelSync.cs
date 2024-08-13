using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerModelSync : NetworkBehaviour
{
    public static PlayerModelSync Instance;

    [SerializeField] private GameObject[] PlayerModels;

    private void Awake()
    {
        Instance = this;
    }

    [ServerRpc]
    public void DestroyModelServerRpc(NetworkObjectReference model)
    {

        Debug.Log("함수 실행");
        if(model.TryGet(out var md))
        {
            Destroy(md.gameObject);
            Debug.Log("모델 파괴");
        }
    }
}
