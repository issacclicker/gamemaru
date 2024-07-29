using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float health = 100; 
    public Image healthpoints; 
    public bool IsGameStarted = false;

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
        if (health > 0)
        {
            health -= damage;
            health = Mathf.Max(health, 0);
            healthpoints.fillAmount = health / 100f;
        }
    }
}
