using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Event : MonoBehaviour
{
    public GameObject RulesPanel;
    public GameObject mainSettingsPanel;
    public GameObject VolumePanel;
    public GameObject KeyPanel;
    public void OnClickRulesBtn()
    {
        Debug.Log("Clicked Rules");
        VolumePanel.SetActive(true);
        mainSettingsPanel.SetActive(false);
        RulesPanel.SetActive(false);
        KeyPanel.SetActive(false);
    }

    public void OnClickQuitBtn()
    {
        Debug.Log("Clicked Quit");
        VolumePanel.SetActive(true);
        mainSettingsPanel.SetActive(false);
        RulesPanel.SetActive(false);
        KeyPanel.SetActive(false);
    }

    public void OnClickKeysBtn()
    {
        Debug.Log("Clicked Keys");
        VolumePanel.SetActive(true);
        mainSettingsPanel.SetActive(false);
        RulesPanel.SetActive(false);
        KeyPanel.SetActive(false);
    }
    public void OnClickVolumeBtn()
    {
        Debug.Log("Clicked Volume");
        VolumePanel.SetActive(true);
        mainSettingsPanel.SetActive(false);
        RulesPanel.SetActive(false);
        KeyPanel.SetActive(false);
    }
}
