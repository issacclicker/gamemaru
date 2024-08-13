using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerController : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}