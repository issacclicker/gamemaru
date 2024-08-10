using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class CatAnimation : NetworkBehaviour
{
    private enum CatState { Idle, Moving, Eating, Dying }
    private CatState currentState = CatState.Idle;

    Animator anim;

    public float maxSpeed = 1.25f; // 최대 속도
    public float minDirectionChangeInterval = 2.5f;
    public float maxDirectionChangeInterval = 10f;
    private Vector3 randomDirection;
    private float speed;
    private Renderer rendererComponent;

    void Start()
    {
        rendererComponent = GetComponent<Renderer>();
        anim = GetComponent<Animator>();
        if (rendererComponent == null)
        {
            Debug.LogError("Renderer component is missing on " + gameObject.name);
        }
        StartCoroutine(ActivityRoutine());
    }

    void Update()
    {
        if (currentState == CatState.Moving)
        {
            transform.Translate(randomDirection * speed * Time.deltaTime, Space.World);
        }
    }

    IEnumerator ActivityRoutine()
    {
        while (true)
        {
            // 랜덤한 시간 동안 이동
            yield return StartCoroutine(MoveRoutine());

            // 랜덤한 시간 동안 쉼
            yield return StartCoroutine(WaitRoutine());

            // 랜덤한 시간 동안 먹이 먹기
            yield return StartCoroutine(EatRoutine());
        }
    }

    IEnumerator MoveRoutine()
    {
        float moveDuration = Random.Range(1f, 10f);
        speed = Random.Range(0.5f, maxSpeed);
        currentState = CatState.Moving;
        anim.SetBool("isWalk", true); // 걷는 애니메이션 재생

        // 방향 변경
        StartCoroutine(ChangeDirectionRoutine());

        // 이동 지속 시간 동안 대기
        yield return new WaitForSeconds(moveDuration);

        anim.SetBool("isWalk", false); // 애니메이션 정지
        currentState = CatState.Idle;
    }

    IEnumerator WaitRoutine()
    {
        float waitDuration = Random.Range(1f, 10f);
        currentState = CatState.Idle;

        // 쉼 지속 시간 동안 대기
        yield return new WaitForSeconds(waitDuration);
    }

    IEnumerator EatRoutine()
    {
        float eatDuration = Random.Range(2f, 10f);
        currentState = CatState.Eating;
        anim.SetBool("isEat", true); // 먹이 먹는 애니메이션 재생

        yield return new WaitForSeconds(eatDuration);

        anim.SetBool("isEat", false); // 애니메이션 정지
        currentState = CatState.Idle; // 행동이 끝나면 Idle 상태로 변경
    }

    IEnumerator ChangeDirectionRoutine()
    {
        while (currentState == CatState.Moving)
        {
            // 랜덤 방향
            randomDirection = new Vector3(Random.Range(-1f, 1f), 0.0f, Random.Range(-1f, 1f)).normalized;

            // 방향 변경 간격
            float interval = Random.Range(minDirectionChangeInterval, maxDirectionChangeInterval);
            yield return new WaitForSeconds(interval);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cube") || collision.gameObject.CompareTag("Player"))
        {
            // 충돌 시 멈춤
            currentState = CatState.Dying;

            // 죽는 애니메이션 재생
            anim.SetTrigger("doDie");
        }
    }
}

