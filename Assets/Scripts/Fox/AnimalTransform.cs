using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalTransform : MonoBehaviour
{
    //현재 모델에서 이미지를 변경하는 방식으로 구현했음
    //x키로 동물 이미지로 변경 -> 원래 이미지로 돌아오기
    //체력 소진시 원래 모델로 돌아감. 체력이 0 보다 클 때만 둔갑 가능. - testHealthBar 스크립트와 연결되어 있음
    //현재는 2개의 이미지만 적용되어 있음
    //애니메이션은 아직 넘어온게 없어서 적용 못 함
    //Assets>AnimalTransform 폴더에 이미지 넣어둠
    //*AnimalTransform script 적용 후 Animal Models에 이미지 넣어두어야 제대로 작동함
    //*PlayerTestThirdPerson에 적용해서 그런지 원래의 이미지로 돌아갔을 때 사람과 함께 큐브가 적용된 이미지가 뜸

    public GameObject[] animalModels; // 동물 모델 배열
    private GameObject currentModel; // 현재 활성화된 모델
    private GameObject originalModel; // 원래 사람 모델
    private bool isAnimal = false; // 현재 모델이 동물인지 여부

    private void Start()
    {
        // 현재 오브젝트를 기본 모델로 설정
        currentModel = gameObject;
        originalModel = gameObject;
    }

    private void Update()
    {
        // X키를 누르면 모델 변경 - 추후 키는 변경
        if (Input.GetKeyDown(KeyCode.X))
        {
            ChangeModel();
        }
    }

    private void ChangeModel()
    {
        // 원래 모델로 변경
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
            Debug.Log("원래 모델로 변경");
        }
        // 동물 모델로 변경
        else
        {
            // 둔갑이 가능한 상태일 때(체력이 0보다 클 때)만 동물 모델로 변경
            testHealthBar healthBar = GetComponent<testHealthBar>();
            if (healthBar != null && healthBar.currentHealth > 0)
            {
                if (currentModel != null && currentModel != originalModel)
                {
                    Destroy(currentModel);
                }

                // 랜덤으로 동물 모델
                int randomIndex = Random.Range(0, animalModels.Length);
                GameObject newModel = Instantiate(animalModels[randomIndex], transform.position, transform.rotation);

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
                isAnimal = true;
                Debug.Log("동물 모델로 변경");
            }
            else
            {
                Debug.Log("체력 부족 동물 모델로 변경 불가");
            }
        }
    }

    // 일정 시간이 경과했을 때 원래 모델로 돌아가기
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
            Debug.Log("원래 모델로 변경");
        }
    }
}
