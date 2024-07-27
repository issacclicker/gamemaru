using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testHealthBar : MonoBehaviour
{
    //Food 태그를 가지고 있는 오브젝트에 닿았을 때 활성화
    //Food 태그를 가지고 있는 오브젝트의 is Trigger를 활성화 시켜야 제대로 돌아감
    //먹이와 가까운 상태에서 E키를 눌렀을 때 먹이 먹기 가능
    //체력 소진시 원래 모델로 돌아감 - AnimalTransform 스크립트와 연결되어 있음
    //먹이를 먹는 동안에만 체력이 감소하지 않음 - max까지 회복되는 것이 불가능. 문제가 발생한다면 이를 나중에 수정해야 할 것 같음
    //*현재는 healthbar 이미지가 없어서 콘솔창에 띄우는 방식으로 구현함
    //*둔갑 스킬 풀리는 것을 테스트 하기 위해 체력을 20으로 설정해둠 - 추후 변경

    public float maxHealth = 20f; // 최대 체력 20
    public float currentHealth; // 현재 체력
    private bool canEatFood = false; // 먹이를 먹을 수 있는지 여부
    private AnimalTransform animalTransform; // AnimalTransform 스크립트에 대한 참조

    private void Start()
    {
        currentHealth = maxHealth;
        animalTransform = GetComponent<AnimalTransform>();
    }

    private void Update()
    {
        // E 키를 눌렀을 때 먹이 먹기
        if (canEatFood && Input.GetKeyDown(KeyCode.E))
        {
            EatFood(5f);
        }

        // 시간이 경과 시 체력 감소
        if (currentHealth > 0)
        {
            PoisonZone(2f * Time.deltaTime);
        }

        // 매 프레임마다 현재 체력을 콘솔에 출력
        Debug.Log("현재 체력: " + currentHealth);
    }

    // 먹이와의 거리가 가까울 때 - 먹이를 먹을 수 있는 상태
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            canEatFood = true;
        }
    }

    // 먹이와의 거리가 멀 때 - 먹이를 먹을 수 없는 상태
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            canEatFood = false;
        }
    }

    // E 키를 눌렀을 때 체력 회복
    public void EatFood(float heal)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += heal;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            Debug.Log("체력 회복: " + heal + ", 현재 체력: " + currentHealth);
        }
    }

    // 매 프레임마다 체력 감소 - 시간 경과 시 체력 감소
    public void PoisonZone(float damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Max(currentHealth, 0);
            Debug.Log("피해: " + damage + ", 현재 체력: " + currentHealth);

            // 체력이 0 이하일 때 체크
            if (currentHealth <= 0)
            {
                OnHealthDepleted();
            }
        }
    }

    //체력 소진 시 원래 모델로 돌아가기
    private void OnHealthDepleted()
    {
        Debug.Log("체력이 소진되었습니다.");
        if (animalTransform != null)
        {
            animalTransform.RevertToOriginalModel();
        }
    }
}
