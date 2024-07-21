
using System.Collections;
using UnityEngine;

//안 쓸 예정

public class TigerController : MonoBehaviour
{
    [SerializeField] private Transform characterBody;
    [SerializeField] private Transform cameraArm;
    public float speed = 5f;
    public float reverseSpeed = 5f;

    private int huntFailures = 0;
    private bool isPenaltyActive = false;
    private int penaltyType = 0;
    private HalfScreen halfScr;
    private Animator animator;

    public GameObject Heart1, Heart2, Heart3;
    public static int health;

    void Start()
    {
        animator = characterBody.GetComponent<Animator>();
        halfScr = FindObjectOfType<HalfScreen>();

        health = 3;
        UpdateHeartUI();
    }

    void Update()
    {
        LookAround();

        if (isPenaltyActive)
        {
            ApplyPenalty();
        }

        Move();

        UpdateHeartUI();
    }

    private void Move()
    {
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMove = moveInput.magnitude != 0;
        animator.SetBool("isMove", isMove);

        if (isMove)
        {
            Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
            Vector3 movedir = lookForward * moveInput.y + lookRight * moveInput.x;

            characterBody.forward = movedir;

            // 속도 감소
            float currentSpeed = isPenaltyActive && penaltyType == 1 ? speed / 2 : speed;

            // 방향 반전
            if (isPenaltyActive && penaltyType == 2)
            {
                transform.position += -movedir * Time.deltaTime * reverseSpeed;
            }
            else
            {
                transform.position += movedir * Time.deltaTime * currentSpeed;
            }
        }

        Debug.DrawRay(cameraArm.position, new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized, Color.red);
    }

    private void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraArm.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }
        cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }

    void ApplyPenalty()
    {
        switch (penaltyType)
        {
            case 1:
                // 속도 감소
                break;
            case 2:
                // 방향 반전
                break;
            case 3:
                if (halfScr != null)
                {
                    halfScr.ActivateHalfScreenPenalty();
                }
                break;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            CubeController cubeController = FindObjectOfType<CubeController>();
            if (cubeController != null && cubeController.IsActive)
            {
                HuntFailure();
            }
        }
    }

    public void HuntFailure()
    {
        if (health > 0)
        {
            health = health - 1;
        }

        UpdateHeartUI();

        if (health == 0)
        {
            ApplyRandomPenalty();
            StartCoroutine(ResetHealthAfterDelay());
        }
    }

    void ApplyRandomPenalty()
    {
        penaltyType = Random.Range(1, 4);
        isPenaltyActive = true;

        switch (penaltyType)
        {
            case 1:
                Debug.Log("Penalty applied: Speed reduced.");
                break;
            case 2:
                Debug.Log("Penalty applied: Controls reversed.");
                break;
            case 3:
                Debug.Log("Penalty applied: Half screen visibility.");
                break;
        }

        StartCoroutine(PenaltyTimer());
    }

    IEnumerator PenaltyTimer()
    {
        yield return new WaitForSeconds(5);
        RemovePenalty();
    }

    void RemovePenalty()
    {
        isPenaltyActive = false;
        penaltyType = 0;
        if (halfScr != null)
        {
            halfScr.DeactivateHalfScreenPenalty();
        }
        Debug.Log("Penalty removed.");
    }

    void UpdateHeartUI()
    {
        Heart1.SetActive(health >= 1);
        Heart2.SetActive(health >= 2);
        Heart3.SetActive(health >= 3);
    }

    IEnumerator ResetHealthAfterDelay()
    {
        yield return new WaitForSeconds(5);
        health = 3;
        UpdateHeartUI();
    }
}