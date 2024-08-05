using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_fbx : MonoBehaviour
{
    private PlayerMovement player; // PlayerMovement 컴포넌트 참조
    private AnimalTransform animal;

    public GameObject fox_model; // 여우 모델
    public GameObject tiger_model; // 호랑이 모델 추가

    private GameObject currentModel; // 현재 활성화된 모델
    private GameObject originalModel; // 원래 사람 모델

    private void Start()
    {
        // 현재 오브젝트를 기본 모델로 설정
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
        // player와 animal이 null인지 확인
        if (player == null)
        {
            Debug.LogError("PlayerMovement 컴포넌트를 찾을 수 없습니다.");
            return;
        }
        if (animal == null)
        {
            Debug.LogError("AnimalTransform 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        // 기존 모델 제거
        if (currentModel != null && currentModel != originalModel)
        {
            Destroy(currentModel);
        }

        // 새로운 모델 선택 및 변경
        switch (player.playerState)
        {
            case "Fox":
                ChangeToModel(fox_model, "여우 모델로 변경");
                break;

            case "Tiger":
                ChangeToModel(tiger_model, "호랑이 모델로 변경");
                break;

            default:
                Debug.Log("모델 변경 없음");
                break;
        }
    }

    private void ChangeToModel(GameObject newModelPrefab, string logMessage)
    {
        GameObject newModel = Instantiate(newModelPrefab, transform.position, transform.rotation);

        // 원래 모델 비활성화
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
