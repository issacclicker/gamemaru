using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public HalfScreen halfScr; //호랑이 패널티 3

    public string playerState = ""; //호랑이인지 여우인지 구분

    // public void LogState()
    // {
    //     Debug.Log(playerState);
    // }

    //호랑이 UI 변수
    public int health_tiger; //호랑이 체력
    public GameObject Heart1, Heart2, Heart3;

    public GameObject Tiger_Skills_UI;
    public GameObject Fox_Skills_UI;

    //여우 UI 변수
    public int beadCount; //여의주 갯수
    public Text beadCountText; //여의주 갯수 텍스트
    public GameObject HealthBar; //체력바

    //변수 초기화, UI 비활성화
    void Start (){
        Heart1.SetActive(false);
        Heart2.SetActive(false);
        Heart3.SetActive(false);
        HealthBar.SetActive(false);
        health_tiger = 3;
        beadCount = 0;
    }

    public void UIEnable(){ //호랑이와 여우 UI 구분
        
        if(playerState=="Tiger")
        {
            UpdateHeartUI();
            Tiger_Skills_UI.SetActive(true);
            Fox_Skills_UI.SetActive(false);
        }
        else if(playerState=="Fox")
        {
            HealthBar.SetActive(true);
            UpdateBeadCountText();
            Tiger_Skills_UI.SetActive(false);
            Fox_Skills_UI.SetActive(true);
        }

    }

    

    //하트 UI 표시(호랑이)
    public void UpdateHeartUI()
    {
        Heart1.SetActive(health_tiger >= 1);
        Heart2.SetActive(health_tiger >= 2);
        Heart3.SetActive(health_tiger >= 3);
    }

    //여의주 갯수 표시(여우)
    public void UpdateBeadCountText()
    {
        beadCountText.text = "Number: " + beadCount;
    }
}
