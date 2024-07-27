using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float health = 100; // �ʱ� ü��
    public Image healthpoints; // ü�¹� �̹���
    // private bool canEatFood = false; // ���̸� ���� �� �ִ��� ����

    public bool IsGameStarted = false;

    private void Update()
    {
        // // E Ű�� ������ �� ���� �Ա�
        // if (canEatFood && Input.GetKeyDown(KeyCode.E))
        // {
        //     EatFood(5f);
        // }

        // ���̸� ���� �ʾ��� �� ü�� ����
        if(IsGameStarted)
            PoisonZone(2f * Time.deltaTime);
        
    }

    //���̿��� �Ÿ��� ����� ��
    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Food"))
    //     {
    //         canEatFood = true;
    //     }
    // }

    // //���̿��� �Ÿ��� �� ��
    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Food"))
    //     {
    //         canEatFood = false;
    //     }
    // }

    // E Ű�� ������ �� ü�� ȸ��
    public void EatFood(float heal)
    {
        if (health < 100)
        {
            health += heal;
            health = Mathf.Min(health, 100);
            healthpoints.fillAmount = health / 100f;
        }
    }

    // �� �����Ӹ��� ����
    public void PoisonZone(float damage)
    {
        if (health > 0)
        {
            health -= damage;
            health = Mathf.Max(health, 0);
            healthpoints.fillAmount = health / 100f;
        }
    }
}
