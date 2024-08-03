using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class NPCController : NetworkBehaviour
{
    public float maxSpeed = 1.25f; // 호랑이 속도의 4분의 1
    public float minDirectionChangeInterval = 2.5f; 
    public float maxDirectionChangeInterval = 10f; 
    private Vector3 randomDirection;
    private float speed;
    private bool isMoving = true;
    private Renderer rendererComponent; 

    void Start()
    {
        rendererComponent = GetComponent<Renderer>();
        if (rendererComponent == null)
        {
            Debug.LogError("Renderer component is missing on " + gameObject.name);
        }
        StartCoroutine(MoveRoutine());
    }

    void Update()
    {
        if (isMoving)
        {
            // Move in the direction
            transform.Translate(randomDirection * speed * Time.deltaTime, Space.World);

            // Rotate to face the direction of movement
            if (randomDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(randomDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 5f);
            }
        }
    }

    IEnumerator MoveRoutine()
    {
        while (true)
        {
            // 랜덤한 이동 시간 설정
            float moveDuration = Random.Range(1f, 10f);
            speed = Random.Range(0.5f, maxSpeed);
            isMoving = true;
            // 방향 변경
            StartCoroutine(ChangeDirectionRoutine()); 

            // 랜덤한 시간 동안 움직임
            yield return new WaitForSeconds(moveDuration);

            isMoving = false;

            // 랜덤한 시간 동안 쉼
            yield return new WaitForSeconds(Random.Range(1f, 10f)); 
        }
    }

    IEnumerator ChangeDirectionRoutine()
    {
        while (isMoving)
        {
            // 랜덤 방향
            randomDirection = new Vector3(Random.Range(-1f, 1f), 0.0f, Random.Range(-1f, 1f)).normalized;

            // 랜덤 방향 변경 
            float interval = Random.Range(minDirectionChangeInterval, maxDirectionChangeInterval);
            yield return new WaitForSeconds(interval);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cube") || collision.gameObject.CompareTag("Player"))
        {
            // 충돌 시 멈춤
            isMoving = false;

            // 색상을 빨간색으로 변경
            if (rendererComponent != null)
            {
                rendererComponent.material.color = Color.red; // 원본 material 수정
            }
        }
    }
}