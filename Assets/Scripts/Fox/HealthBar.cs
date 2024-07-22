using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float health = 100; // 초기 체력
    public Image healthpoints; // 체력바 이미지
    private bool canEatFood = false; // 먹이를 먹을 수 있는지 여부

    private void Update()
    {
        // E 키를 눌렀을 때 먹이 먹기
        if (canEatFood && Input.GetKeyDown(KeyCode.E))
        {
            EatFood(5f);
        }

        // 먹이를 먹지 않았을 때 체력 감소
        if (!canEatFood)
        {
            PoisonZone(2f * Time.deltaTime);
        }
    }

    //먹이와의 거리가 가까울 때
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            canEatFood = true;
        }
    }

    //먹이와의 거리가 멀 때
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
        if (health < 100)
        {
            health += heal;
            health = Mathf.Min(health, 100);
            healthpoints.fillAmount = health / 100f;
        }
    }

    // 매 프레임마다 피해
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
