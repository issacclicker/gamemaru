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
            // ���� â�� Ȱ��/��Ȱ�� ���¸� ���
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
    }
}