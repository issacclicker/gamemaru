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
    // private GameObject currentModel; // ���� Ȱ��ȭ�� ��
    // private GameObject originalModel; // ���� ��� ��
    private bool isAnimal = false; // ���� ���� �������� ����

    GameObject UIManagerObject; //UI����
    HealthBar _healthBar; //ü�¹�
    PlayerMovement _playerMovement;
    

    private void Start()
    {
        UIManagerObject = GameObject.Find("UIManager");
        _healthBar = UIManagerObject.GetComponent<HealthBar>();

        _playerMovement = GetComponent<PlayerMovement>();

        // ���� ������Ʈ�� �⺻ �𵨷� ����
        _playerMovement.currentModel = gameObject;
        _playerMovement.originalModel = gameObject;
    }

    private void Update()
    {


        if (_healthBar.health <0){
            RevertToOriginalModel();
        }
        // XŰ�� ������ �� ���� - ���� Ű�� ����
        if (Input.GetKeyDown(KeyCode.X) && !_playerMovement.isAwaken)
        {
            ChangeModel();
        }
        
    }

    private void ChangeModel()
    {
        // ���� �𵨷� ����
        if (isAnimal)
        {
            Destroy(_playerMovement.currentModel);

            if (_playerMovement.originalModel != null)
            {
                Renderer[] renderers = _playerMovement.originalModel.GetComponentsInChildren<Renderer>();
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
            

            if (_healthBar != null && _healthBar.health > 0)
            {
                if (_playerMovement.currentModel != null && _playerMovement.currentModel != _playerMovement.originalModel)
                {
                    Destroy(_playerMovement.currentModel);
                }

                // �������� ���� ��
                int randomIndex = Random.Range(0, animalModels.Length);
                GameObject newModel = Instantiate(animalModels[randomIndex], transform.position, transform.rotation);

                // ���� �� ��Ȱ��ȭ
                if (_playerMovement.originalModel != null)
                {
                    Renderer[] renderers = _playerMovement.originalModel.GetComponentsInChildren<Renderer>();
                    foreach (var renderer in renderers)
                    {
                        renderer.enabled = false;
                    }
                }

                newModel.transform.SetParent(transform);
                _playerMovement.currentModel = newModel;
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
        if (isAnimal && !_playerMovement.isAwaken)
        {
            Destroy(_playerMovement.currentModel);

            if (_playerMovement.originalModel != null)
            {
                Renderer[] renderers = _playerMovement.originalModel.GetComponentsInChildren<Renderer>();
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
