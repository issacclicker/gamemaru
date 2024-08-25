using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    public GameObject[] EndGameUI;


    public void GameOver()
    {
        foreach (var obj in EndGameUI)
        {
            obj.SetActive(true);
        }
    }
}
