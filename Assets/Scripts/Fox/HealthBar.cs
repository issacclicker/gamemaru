using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float health = 100; //원래 100
    public Image healthpoints; 
    public bool IsGameStarted = false;
    private bool isGameEnded = false;//게임 종료

    public bool isAnimal = false; // 플레이어가 변신 상태인지 아닌지

    private void Update()
    {
        
        if(IsGameStarted)
            PoisonZone(2f * Time.deltaTime);
        
    }

    public void EatFood(float heal)
    {
        if (health < 100)
        {
            health += heal;
            health = Mathf.Min(health, 100);
            healthpoints.fillAmount = health / 100f;
        }
    }

    
    public void PoisonZone(float damage)
    {
        if (health > 0 && isAnimal)
        {
            health -= damage;
            health = Mathf.Max(health, 0);
            healthpoints.fillAmount = health / 100f;
        }
    }
}
