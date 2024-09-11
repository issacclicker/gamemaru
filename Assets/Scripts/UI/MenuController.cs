using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject RulesPanel;
    public GameObject RulesPanel2;
    public GameObject RulesPanel3;
    public GameObject RulesPanel4;
    public GameObject RulesPanel5;
    public GameObject RulesPanel6;
    public GameObject mainSettingsPanel;
    public GameObject VolumePanel;
    public GameObject KeyPanel;

    public void OnClickVolumeButton()
    {
        VolumePanel.SetActive(true);
        
        mainSettingsPanel.SetActive(false);
        
        RulesPanel.SetActive(false);
        RulesPanel2.SetActive(false);
        RulesPanel3.SetActive(false);
        RulesPanel4.SetActive(false);
        RulesPanel5.SetActive(false);
        RulesPanel6.SetActive(false);

        KeyPanel.SetActive(false);
    }

    // Quit ��ư Ŭ�� �� ȣ��� �Լ�
    public void OnClickQuitButton()
    {  
        Debug.Log("Game is quitting...");
        VolumePanel.SetActive(false);
       
        mainSettingsPanel.SetActive(false);
        
        RulesPanel.SetActive(false);
        RulesPanel2.SetActive(false);
        RulesPanel3.SetActive(false);
        RulesPanel4.SetActive(false);
        RulesPanel5.SetActive(false);
        RulesPanel6.SetActive(false);

        KeyPanel.SetActive(false);
    
    }

    

    public void OnClickRulesButton()
    {
        mainSettingsPanel.SetActive(false);
        
        RulesPanel.SetActive(true);
        RulesPanel2.SetActive(false);
        RulesPanel3.SetActive(false);
        RulesPanel4.SetActive(false);
        RulesPanel5.SetActive(false);
        RulesPanel6.SetActive(false);

        VolumePanel.SetActive(false);
        
        KeyPanel.SetActive(false);
    }

    public void OnClickKeysButton()
    {
        mainSettingsPanel.SetActive(false);
        
        RulesPanel.SetActive(false);
        RulesPanel2.SetActive(false);
        RulesPanel3.SetActive(false);
        RulesPanel4.SetActive(false);
        RulesPanel5.SetActive(false);
        RulesPanel6.SetActive(false);

        VolumePanel.SetActive(false);
        
        KeyPanel.SetActive(true);
    }

    public void OnClickBackButton()
    {
        mainSettingsPanel.SetActive(true);
        
        RulesPanel.SetActive(false);
        RulesPanel2.SetActive(false);
        RulesPanel3.SetActive(false);
        RulesPanel4.SetActive(false);
        RulesPanel5.SetActive(false);
        RulesPanel6.SetActive(false);

        VolumePanel.SetActive(false);
        
        KeyPanel.SetActive(false);
    }

    public void OnClick1_2nextbutton()
    {
        mainSettingsPanel.SetActive(false);

        RulesPanel.SetActive(false);
        RulesPanel2.SetActive(true);
        RulesPanel3.SetActive(false);
        RulesPanel4.SetActive(false);
        RulesPanel5.SetActive(false);
        RulesPanel6.SetActive(false);

        VolumePanel.SetActive(false);

        KeyPanel.SetActive(false);
    }
   
    public void OnClick2_3nextbutton()
    {
        mainSettingsPanel.SetActive(false);

        RulesPanel.SetActive(false);
        RulesPanel2.SetActive(false);
        RulesPanel3.SetActive(true);
        RulesPanel4.SetActive(false);
        RulesPanel5.SetActive(false);
        RulesPanel6.SetActive(false);

        VolumePanel.SetActive(false);

        KeyPanel.SetActive(false);
    }
    public void OnClick2_3priorbutton()
    {
        mainSettingsPanel.SetActive(false);

        RulesPanel.SetActive(true);
        RulesPanel2.SetActive(false);
        RulesPanel3.SetActive(false);
        RulesPanel4.SetActive(false);
        RulesPanel5.SetActive(false);
        RulesPanel6.SetActive(false);

        VolumePanel.SetActive(false);

        KeyPanel.SetActive(false);
    }
    public void OnClick3_4nextbutton()
    {
        mainSettingsPanel.SetActive(false);

        RulesPanel.SetActive(false);
        RulesPanel2.SetActive(false);
        RulesPanel3.SetActive(false);
        RulesPanel4.SetActive(true);
        RulesPanel5.SetActive(false);
        RulesPanel6.SetActive(false);

        VolumePanel.SetActive(false);

        KeyPanel.SetActive(false);
    }
    public void OnClick3_4priorbutton()
    {
        mainSettingsPanel.SetActive(false);

        RulesPanel.SetActive(false);
        RulesPanel2.SetActive(true);
        RulesPanel3.SetActive(false);
        RulesPanel4.SetActive(false);
        RulesPanel5.SetActive(false);
        RulesPanel6.SetActive(false);

        VolumePanel.SetActive(false);

        KeyPanel.SetActive(false);
    }
    public void OnClick4_5nextbutton()
    {
        mainSettingsPanel.SetActive(false);

        RulesPanel.SetActive(false);
        RulesPanel2.SetActive(false);
        RulesPanel3.SetActive(false);
        RulesPanel4.SetActive(false);
        RulesPanel5.SetActive(true);
        RulesPanel6.SetActive(false);

        VolumePanel.SetActive(false);

        KeyPanel.SetActive(false);
    }
    public void OnClick4_5priorbutton()
    {
        mainSettingsPanel.SetActive(false);

        RulesPanel.SetActive(false);
        RulesPanel2.SetActive(false);
        RulesPanel3.SetActive(true);
        RulesPanel4.SetActive(false);
        RulesPanel5.SetActive(false);
        RulesPanel6.SetActive(false);

        VolumePanel.SetActive(false);

        KeyPanel.SetActive(false);
    }
    public void OnClick5_6nextbutton()
    {
        mainSettingsPanel.SetActive(false);

        RulesPanel.SetActive(false);
        RulesPanel2.SetActive(false);
        RulesPanel3.SetActive(false);
        RulesPanel4.SetActive(false);
        RulesPanel5.SetActive(false);
        RulesPanel6.SetActive(true);

        VolumePanel.SetActive(false);

        KeyPanel.SetActive(false);
    }
    public void OnClick5_6priorbutton()
    {
        mainSettingsPanel.SetActive(false);

        RulesPanel.SetActive(false);
        RulesPanel2.SetActive(false);
        RulesPanel3.SetActive(false);
        RulesPanel4.SetActive(true);
        RulesPanel5.SetActive(false);
        RulesPanel6.SetActive(false);

        VolumePanel.SetActive(false);

        KeyPanel.SetActive(false);
    }
    public void OnClick6priorbutton()
    {
        mainSettingsPanel.SetActive(false);

        RulesPanel.SetActive(false);
        RulesPanel2.SetActive(false);
        RulesPanel3.SetActive(false);
        RulesPanel4.SetActive(false);
        RulesPanel5.SetActive(true);
        RulesPanel6.SetActive(false);

        VolumePanel.SetActive(false);

        KeyPanel.SetActive(false);
    }
}