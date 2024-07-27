using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalTransform : MonoBehaviour
{
    //���� �𵨿��� �̹����� �����ϴ� ������� ��������
    //xŰ�� ���� �̹����� ���� -> ���� �̹����� ���ƿ���
    //ü�� ������ ���� �𵨷� ���ư�. ü���� 0 ���� Ŭ ���� �а� ����. - testHealthBar ��ũ��Ʈ�� ����Ǿ� ����
    //����� 2���� �̹����� ����Ǿ� ����
    //�ִϸ��̼��� ���� �Ѿ�°� ��� ���� �� ��
    //Assets>AnimalTransform ������ �̹��� �־��
    //*AnimalTransform script ���� �� Animal Models�� �̹��� �־�ξ�� ����� �۵���
    //*PlayerTestThirdPerson�� �����ؼ� �׷��� ������ �̹����� ���ư��� �� ����� �Բ� ť�갡 ����� �̹����� ��

    public GameObject[] animalModels; // ���� �� �迭
    private GameObject currentModel; // ���� Ȱ��ȭ�� ��
    private GameObject originalModel; // ���� ��� ��
    private bool isAnimal = false; // ���� ���� �������� ����

    private void Start()
    {
        // ���� ������Ʈ�� �⺻ �𵨷� ����
        currentModel = gameObject;
        originalModel = gameObject;
    }

    private void Update()
    {
        // XŰ�� ������ �� ���� - ���� Ű�� ����
        if (Input.GetKeyDown(KeyCode.X))
        {
            ChangeModel();
        }
    }

    private void ChangeModel()
    {
        // ���� �𵨷� ����
        if (isAnimal)
        {
            Destroy(currentModel);

            if (originalModel != null)
            {
                Renderer[] renderers = originalModel.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    renderer.enabled = true;
                }
            }

            isAnimal = false;
            Debug.Log("���� �𵨷� ����");
        }
        // ���� �𵨷� ����
        else
        {
            // �а��� ������ ������ ��(ü���� 0���� Ŭ ��)�� ���� �𵨷� ����
            testHealthBar healthBar = GetComponent<testHealthBar>();
            if (healthBar != null && healthBar.currentHealth > 0)
            {
                if (currentModel != null && currentModel != originalModel)
                {
                    Destroy(currentModel);
                }

                // �������� ���� ��
                int randomIndex = Random.Range(0, animalModels.Length);
                GameObject newModel = Instantiate(animalModels[randomIndex], transform.position, transform.rotation);

                // ���� �� ��Ȱ��ȭ
                if (originalModel != null)
                {
                    Renderer[] renderers = originalModel.GetComponentsInChildren<Renderer>();
                    foreach (var renderer in renderers)
                    {
                        renderer.enabled = false;
                    }
                }

                newModel.transform.SetParent(transform);
                currentModel = newModel;
                isAnimal = true;
                Debug.Log("���� �𵨷� ����");
            }
            else
            {
                Debug.Log("ü�� ���� ���� �𵨷� ���� �Ұ�");
            }
        }
    }

    // ���� �ð��� ������� �� ���� �𵨷� ���ư���
    public void RevertToOriginalModel()
    {
        if (isAnimal)
        {
            Destroy(currentModel);

            if (originalModel != null)
            {
                Renderer[] renderers = originalModel.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    renderer.enabled = true;
                }
            }

            isAnimal = false;
            Debug.Log("���� �𵨷� ����");
        }
    }
}
