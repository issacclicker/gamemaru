using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class npc_animation: NetworkBehaviour
{
    private enum CatState { Idle, Moving, Eating, Dying }
    private CatState currentState = CatState.Idle;

    Animator anim;

    public float maxSpeed = 1.25f; // �ִ� �ӵ�
    public float minDirectionChangeInterval = 2.5f;
    public float maxDirectionChangeInterval = 10f;
    private Vector3 randomDirection;
    private float speed;
    private Renderer rendererComponent;

    public GameObject renderingObject;

    void Start()
    {
        rendererComponent = renderingObject.GetComponent<Renderer>();
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

            // ȸ��
            if (randomDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(randomDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            // �ִϸ��̼� ���� ����
            if (speed > 0.75f) // �ӵ��� ���� �Ǵ�
            {
                anim.SetBool("isRun", true);
                anim.SetBool("isWalk", false);
            }
            else
            {
                anim.SetBool("isRun", false);
                anim.SetBool("isWalk", true);
            }
        }
    }

    IEnumerator MoveRoutine()
    {
        float moveDuration = Random.Range(1f, 10f);
        speed = Random.Range(0.5f, maxSpeed);
        currentState = CatState.Moving;

        // ���� ����
        StartCoroutine(ChangeDirectionRoutine());

        // �̵� ���� �ð� ���� ���
        yield return new WaitForSeconds(moveDuration);

        anim.SetBool("isWalk", false);
        anim.SetBool("isRun", false);
        currentState = CatState.Idle;
    }

    IEnumerator ActivityRoutine()
    {
        while (true)
        {
            // ������ �ð� ���� �̵�
            yield return StartCoroutine(MoveRoutine());

            // ������ �ð� ���� ��
            yield return StartCoroutine(WaitRoutine());

            // ������ �ð� ���� ���� �Ա�
            yield return StartCoroutine(EatRoutine());
        }
    }

    IEnumerator WaitRoutine()
    {
        float waitDuration = Random.Range(1f, 10f);
        currentState = CatState.Idle;

        // �� ���� �ð� ���� ���
        yield return new WaitForSeconds(waitDuration);
    }

    IEnumerator EatRoutine()
    {
        float eatDuration = Random.Range(2f, 10f);
        currentState = CatState.Eating;
        anim.SetBool("isEat", true); // ���� �Դ� �ִϸ��̼� ���

        yield return new WaitForSeconds(eatDuration);

        anim.SetBool("isEat", false); // �ִϸ��̼� ����
        currentState = CatState.Idle; // �ൿ�� ������ Idle ���·� ����
    }

    IEnumerator ChangeDirectionRoutine()
    {
        while (currentState == CatState.Moving)
        {
            // ���� ����
            randomDirection = new Vector3(Random.Range(-1f, 1f), 0.0f, Random.Range(-1f, 1f)).normalized;

            // ���� ���� ����
            float interval = Random.Range(minDirectionChangeInterval, maxDirectionChangeInterval);
            yield return new WaitForSeconds(interval);
        }
    }

    // void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Cube") || collision.gameObject.CompareTag("Player"))
    //     {
    //         // �浹 �� ����
    //         currentState = CatState.Dying;

    //         // �״� �ִϸ��̼� ���
    //         anim.SetTrigger("doDie");
    //     }
    // }
}

