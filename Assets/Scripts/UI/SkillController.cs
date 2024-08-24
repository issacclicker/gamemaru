using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillController : MonoBehaviour
{
    public GameObject[] hideSkillButtons;
    public GameObject[] textPros;
    public TextMeshProUGUI[] hideSkillTimeTexts;
    public Image[] hideSkillImages;
    private bool[] isHideSkills = { false, false };
    private float[] skillTimes = { 45, 1 };
    private float[] getSkillTimes = { 0, 0 };
    private bool SkillCoolingTime_Click;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < textPros.Length; i++)
        {
            hideSkillTimeTexts[i] = textPros[i].GetComponent<TextMeshProUGUI>();
            hideSkillButtons[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            HideSkillSetting(0);
        }

        if (Input.GetMouseButtonDown(0) && !SkillCoolingTime_Click)
        {
            HideSkillSetting(1);
            SkillCoolTime();
        }

        HideSkillChk();
    }

    void SkillCoolTime()
    {
        SkillCoolingTime_Click = true;
        StartCoroutine(SkillCoolingTime2());
    }
    IEnumerator SkillCoolingTime2()
    {
        yield return new WaitForSeconds(1f);
        SkillCoolTimeEnd();
    }

    void SkillCoolTimeEnd()
    {
        SkillCoolingTime_Click = false;
    }

    public void HideSkillSetting(int skillNum)
    {
        hideSkillButtons[skillNum].SetActive(true);
        getSkillTimes[skillNum] = skillTimes[skillNum];
        isHideSkills[skillNum] = true;
    }

    private void HideSkillChk()
    {
        if (isHideSkills[0])
        {
            StartCoroutine(SkillTimeChk(0));
        }

        if (isHideSkills[1])
        {
            StartCoroutine(SkillTimeChk(1));
        }
    }

    IEnumerator SkillTimeChk(int skillNum)
    {
        yield return null;

        if (getSkillTimes[skillNum] > 0)
        {
            getSkillTimes[skillNum] -= Time.deltaTime;

            if (getSkillTimes[skillNum] < 0)
            {
                getSkillTimes[skillNum] = 0;
                isHideSkills[skillNum] = false;
                hideSkillButtons[skillNum].SetActive(false);
            }

            hideSkillTimeTexts[skillNum].text = getSkillTimes[skillNum].ToString("00");

            float time = getSkillTimes[skillNum] / skillTimes[skillNum];
            hideSkillImages[skillNum].fillAmount = time;
        }
    }
}