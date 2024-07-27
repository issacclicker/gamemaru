using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testHealthBar : MonoBehaviour
{
    //Food �±׸� ������ �ִ� ������Ʈ�� ����� �� Ȱ��ȭ
    //Food �±׸� ������ �ִ� ������Ʈ�� is Trigger�� Ȱ��ȭ ���Ѿ� ����� ���ư�
    //���̿� ����� ���¿��� EŰ�� ������ �� ���� �Ա� ����
    //ü�� ������ ���� �𵨷� ���ư� - AnimalTransform ��ũ��Ʈ�� ����Ǿ� ����
    //���̸� �Դ� ���ȿ��� ü���� �������� ���� - max���� ȸ���Ǵ� ���� �Ұ���. ������ �߻��Ѵٸ� �̸� ���߿� �����ؾ� �� �� ����
    //*����� healthbar �̹����� ��� �ܼ�â�� ���� ������� ������
    //*�а� ��ų Ǯ���� ���� �׽�Ʈ �ϱ� ���� ü���� 20���� �����ص� - ���� ����

    public float maxHealth = 20f; // �ִ� ü�� 20
    public float currentHealth; // ���� ü��
    private bool canEatFood = false; // ���̸� ���� �� �ִ��� ����
    private AnimalTransform animalTransform; // AnimalTransform ��ũ��Ʈ�� ���� ����

    private void Start()
    {
        currentHealth = maxHealth;
        animalTransform = GetComponent<AnimalTransform>();
    }

    private void Update()
    {
        // E Ű�� ������ �� ���� �Ա�
        if (canEatFood && Input.GetKeyDown(KeyCode.E))
        {
            EatFood(5f);
        }

        // �ð��� ��� �� ü�� ����
        if (currentHealth > 0)
        {
            PoisonZone(2f * Time.deltaTime);
        }

        // �� �����Ӹ��� ���� ü���� �ֿܼ� ���
        Debug.Log("���� ü��: " + currentHealth);
    }

    // ���̿��� �Ÿ��� ����� �� - ���̸� ���� �� �ִ� ����
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            canEatFood = true;
        }
    }

    // ���̿��� �Ÿ��� �� �� - ���̸� ���� �� ���� ����
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            canEatFood = false;
        }
    }

    // E Ű�� ������ �� ü�� ȸ��
    public void EatFood(float heal)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += heal;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            Debug.Log("ü�� ȸ��: " + heal + ", ���� ü��: " + currentHealth);
        }
    }

    // �� �����Ӹ��� ü�� ���� - �ð� ��� �� ü�� ����
    public void PoisonZone(float damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Max(currentHealth, 0);
            Debug.Log("����: " + damage + ", ���� ü��: " + currentHealth);

            // ü���� 0 ������ �� üũ
            if (currentHealth <= 0)
            {
                OnHealthDepleted();
            }
        }
    }

    //ü�� ���� �� ���� �𵨷� ���ư���
    private void OnHealthDepleted()
    {
        Debug.Log("ü���� �����Ǿ����ϴ�.");
        if (animalTransform != null)
        {
            animalTransform.RevertToOriginalModel();
        }
    }
}
