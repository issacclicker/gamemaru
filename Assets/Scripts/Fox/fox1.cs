using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fox1 : MonoBehaviour
{
    public float speed;
    public GameObject[] Bead;
    public bool[] hasBeads; 
    public Text beadCountText;

    float hAxis;
    float vAxis;
    bool jDown;
    bool iDown;

    //전체 이동 방향 저장할 변수
    Vector3 moveVec;

    Rigidbody rigid;
    
    //트리거 된 아이템을 저장할 변수
    GameObject nearObject;

    //획득한 여의주 수
    private int beadCount = 0;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        UpdateBeadCountText();
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Interaction();
    }

    //입력받는 함수
    void GetInput()
    {
        //이동할 방향 입력 받기
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        //점프 입력 받기
        jDown = Input.GetButtonDown("Jump");

        //구슬 먹기
        iDown = Input.GetButtonDown("Interaction");
    }

    //이동시키기
    void Move()
    {
        //대각선 방향에서도 1만 이동하도록
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        //이동 속도 값에 따라 이동
        transform.position += moveVec * speed * Time.deltaTime;
    }

    //회전시키기
    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    //점프하기
    void Jump()
    {
        if (jDown)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
        }
    }

    void Interaction()
    {
        //여의주 먹기
        if(iDown && nearObject != null && !jDown)
        {
            if(nearObject.tag == "Bead")
            {
                Item item = nearObject.GetComponent<Item>();
                int beadIndex = item.value;
                hasBeads[beadIndex] = true;

                beadCount++;
                UpdateBeadCountText();

                Destroy(nearObject);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        //닿은 여의주 저장
        if (other.tag == "Bead")
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        //여의주가 있는 영역을 벗어나면 여의주 없애기
        if (other.tag == "Bead")
            nearObject = null;
    }

    // 텍스트 업데이트
    private void UpdateBeadCountText()
    {
        beadCountText.text = "Number: " + beadCount;
    }
}
