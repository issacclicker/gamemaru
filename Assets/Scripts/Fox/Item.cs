using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;

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

    // 여의주 스폰 범위
    [SerializeField] private float spawnRangeX = 100.0f; // x축 범위
    [SerializeField] private float spawnRangeZ = 150.0f; // z축 범위

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
            Vector3 randomPosition = GetRandomPosition();
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

    private Vector3 GetRandomPosition()
    {
        Vector3 randomPosition = Vector3.zero; // 초기값 설정
        bool validPosition = false;
        int maxAttempts = 10; // 최대 시도 횟수
        int attempts = 0;

        while (!validPosition && attempts < maxAttempts)
        {
            // 설정한 범위 내에서 랜덤한 x와 z 좌표 생성
            float randomX = Random.Range(-spawnRangeX, spawnRangeX);
            float randomZ = Random.Range(-spawnRangeZ, spawnRangeZ);

            // y 값을 고정
            float fixedY = -8.3f;

            // 최종 랜덤 위치 설정
            randomPosition = new Vector3(randomX, fixedY, randomZ);

            // Collider가 있는지 확인 (반경 1.0f로 설정)
            Collider[] colliders = Physics.OverlapSphere(randomPosition, 1.0f);
            if (colliders.Length == 0)
            {
                validPosition = true; // 겹치지 않으면 유효한 위치
            }

            attempts++;
        }

        if (!validPosition)
        {
            Debug.LogWarning("Could not find a valid spawn position after multiple attempts.");
            return Vector3.zero; // 유효한 위치를 찾지 못한 경우 기본 위치 반환
        }

        return randomPosition; // 유효한 위치 반환
    }


    [ServerRpc]
    private void MakeBeadServerRpc()
    {
        for (int i = 0; i < beadTransForm.Length; i++)
        {
            Vector3 randomPosition = GetRandomPosition();
            GameObject bd = Instantiate(Bead, randomPosition, Quaternion.identity);
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
