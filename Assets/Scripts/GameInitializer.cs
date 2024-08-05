using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public GameObject scoreManagerPrefab;

    void Start()
    {
        if (FindObjectOfType<ScoreManager>() == null)
        {
            Instantiate(scoreManagerPrefab);
        }
    }
}