using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_fbx : MonoBehaviour
{
    private PlayerMovement player; // PlayerMovement ������Ʈ ����
    private AnimalTransform animal;

    public GameObject fox_model; // ���� ��
    public GameObject tiger_model; // ȣ���� �� �߰�

    private GameObject currentModel; // ���� Ȱ��ȭ�� ��
    private GameObject originalModel; // ���� ��� ��

    private void Start()
    {
        // ���� ������Ʈ�� �⺻ �𵨷� ����
        currentModel = gameObject;
        originalModel = gameObject;

        player = GetComponent<PlayerMovement>();
        animal = GetComponent<AnimalTransform>();
    }

    private void Update()
    {
        if (!animal.isAnimal)
        {
            ChangeModel();
        }
    }

    private void ChangeModel()
    {
        // player�� animal�� null���� Ȯ��
        if (player == null)
        {
            Debug.LogError("PlayerMovement ������Ʈ�� ã�� �� �����ϴ�.");
            return;
        }
        if (animal == null)
        {
            Debug.LogError("AnimalTransform ������Ʈ�� ã�� �� �����ϴ�.");
            return;
        }

        // ���� �� ����
        if (currentModel != null && currentModel != originalModel)
        {
            Destroy(currentModel);
        }

        // ���ο� �� ���� �� ����
        switch (player.playerState)
        {
            case "Fox":
                ChangeToModel(fox_model, "���� �𵨷� ����");
                break;

            case "Tiger":
                ChangeToModel(tiger_model, "ȣ���� �𵨷� ����");
                break;

            default:
                Debug.Log("�� ���� ����");
                break;
        }
    }

    private void ChangeToModel(GameObject newModelPrefab, string logMessage)
    {
        GameObject newModel = Instantiate(newModelPrefab, transform.position, transform.rotation);

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
        Debug.Log(logMessage);
    }
}
