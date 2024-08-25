using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Item : NetworkBehaviour
{
    public static Item Instance;

    public GameObject nearObject;

    // 여의주 모델 오브젝트
    [SerializeField] private GameObject Bead;

    // 여의주가 소환될 위치
    [SerializeField] private Transform[] beadTransForm;

    // 음식 모델 오브젝트
    [SerializeField] private GameObject Food;

    // 음식이 소환될 위치
    [SerializeField] private Transform[] foodTransForm;

    // 개구멍
    [SerializeField] private GameObject DogHole;

    // 개구멍이 소환될 위치
    [SerializeField] private Transform[] dogHoleTransForm;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (!IsHost || !IsOwner)
        {
            return;
        }

        InitializeBeadTransforms();

        MakeBeadServerRpc();
    }

    private void InitializeBeadTransforms()
    {
        for (int i = 0; i < beadTransForm.Length; i++)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-10f, 10f), Random.Range(1f, 5f), Random.Range(-10f, 10f));
            beadTransForm[i].position = randomPosition;

            Debug.Log($"beadTransForm[{i}] new position: {beadTransForm[i].position}");
        }
    }

    private Vector3 GetGroundPosition(Vector3 originalPosition)
    {
        RaycastHit hit;
        if (Physics.Raycast(originalPosition + Vector3.up * 50f, Vector3.down, out hit, Mathf.Infinity))
        {
            Debug.Log($"Raycast hit at: {hit.point}");
            return hit.point;
        }
        else{
            return originalPosition; 
        }
    }

    [ServerRpc]
    private void MakeBeadServerRpc()
    {
        for (int i = 0; i < beadTransForm.Length; i++)
        {
            Vector3 groundPosition = GetGroundPosition(beadTransForm[i].position);
            GameObject bd = Instantiate(Bead, groundPosition, beadTransForm[i].rotation);
            bd.GetComponent<NetworkObject>().Spawn();
        }

        for (int i = 0; i < foodTransForm.Length; i++)
        {
            Vector3 groundPosition = GetGroundPosition(foodTransForm[i].position);
            GameObject fd = Instantiate(Food, groundPosition, foodTransForm[i].rotation);
            fd.GetComponent<NetworkObject>().Spawn();
        }

        for (int i = 0; i < dogHoleTransForm.Length; i++)
        {
            Vector3 groundPosition = GetGroundPosition(dogHoleTransForm[i].position);
            GameObject dh = Instantiate(DogHole, groundPosition, dogHoleTransForm[i].rotation);
            dh.GetComponent<NetworkObject>().Spawn();
        }
    }
    

    [ServerRpc(RequireOwnership = false)]
    public void DestroyBeadServerRpc(NetworkObjectReference target)
    {
        if (target.TryGet(out var obj))
            obj.Despawn();
    }
}
