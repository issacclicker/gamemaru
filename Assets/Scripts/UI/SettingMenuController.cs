using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenuController : MonoBehaviour
{
    [SerializeField]private GameObject settingsPanel;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 설정 창의 활성/비활성 상태를 토글
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
    }
}