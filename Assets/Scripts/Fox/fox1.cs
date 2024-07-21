using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//playerMovement 로 통합

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

    //��ü �̵� ���� ������ ����
    Vector3 moveVec;

    Rigidbody rigid;
    
    //Ʈ���� �� �������� ������ ����
    GameObject nearObject;

    //ȹ���� ������ ��
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

    //�Է¹޴� �Լ�
    void GetInput()
    {
        //�̵��� ���� �Է� �ޱ�
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        //���� �Է� �ޱ�
        jDown = Input.GetButtonDown("Jump");

        //���� �Ա�
        iDown = Input.GetButtonDown("Interaction");
    }

    //�̵���Ű��
    void Move()
    {
        //�밢�� ���⿡���� 1�� �̵��ϵ���
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        //�̵� �ӵ� ���� ���� �̵�
        transform.position += moveVec * speed * Time.deltaTime;
    }

    //ȸ����Ű��
    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    //�����ϱ�
    void Jump()
    {
        if (jDown)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
        }
    }

    void Interaction()
    {
        //������ �Ա�
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
        //���� ������ ����
        if (other.tag == "Bead")
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        //�����ְ� �ִ� ������ ����� ������ ���ֱ�
        if (other.tag == "Bead")
            nearObject = null;
    }

    // �ؽ�Ʈ ������Ʈ
    private void UpdateBeadCountText()
    {
        beadCountText.text = "Number: " + beadCount;
    }
}
