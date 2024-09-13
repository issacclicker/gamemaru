using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenuController : MonoBehaviour
{
    [SerializeField]private GameObject settingsPanel;
    [SerializeField]private GameObject orginCanvas;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ���� â�� Ȱ��/��Ȱ�� ���¸� ���
            settingsPanel.SetActive(!settingsPanel.activeSelf);
            orginCanvas.SetActive(!orginCanvas.activeSelf);
        }
    }
}