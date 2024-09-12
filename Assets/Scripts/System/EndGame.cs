using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EndGame : MonoBehaviour
{
    public GameObject[] EndGameUI;

    public GameObject SceneManager;

    public void GameOver()
    {
        foreach (var obj in EndGameUI)
        {
            obj.SetActive(true);
        }
        StartCoroutine(WaitForSeconds());
    }

    IEnumerator WaitForSeconds()
    {
        Debug.Log("코루틴 시작: " + Time.time);
        
        // 5초 동안 대기
        yield return new WaitForSeconds(3);

        Debug.Log("코루틴 종료: " + Time.time);
        __GameOver__();
    }

    void __GameOver__()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


        SceneManager.GetComponent<SceneChanger>().OnChangingBtnClick();
        NetworkManager.Singleton.Shutdown();
    }
}
