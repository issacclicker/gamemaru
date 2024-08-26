using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance;

    [SerializeField] private string ChangingSceneName;

    void Awake()
    {
        Instance = this;
    }

    public void OnChangingBtnClick()
    {
        SceneManager.LoadScene(ChangingSceneName);
    }

    
}
