using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;

public class Tiger_roaring : MonoBehaviour
{

    [SerializeField] private GameObject hitBox;

    MeshRenderer meshRenderer;
    SphereCollider boxCollider;

    Collider _other;
    NPCController _npcController;

    bool SkillContinuing;

    public bool SkillCoolingTime;


    void Start() 
    {
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<SphereCollider>();
        meshRenderer.enabled = false;
        boxCollider.enabled = false;
        SkillContinuing = false;
        SkillCoolingTime = false;
    }


    public void Roar() 
    {
        if (SkillContinuing) 
        {
            return;
        }


        Debug.Log("아니 왜");
        meshRenderer.enabled = true;
        boxCollider.enabled = true;
        SkillContinuing = true;
        StartCoroutine(SkillTime());
        
        SkillCoolTime();
    }


    IEnumerator SkillTime() 
    {
        yield return new WaitForSeconds(0.1f);
        Roar_End();
    }

    void Roar_End() 
    {
        meshRenderer.enabled = false;
        boxCollider.enabled = false;
        SkillContinuing = false;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        _other = other;

        if (_other.CompareTag("NPC")) 
        {
            _npcController = _other.GetComponent<NPCController>();
            _npcController.enabled = false;
            Debug.Log("��");
            StartCoroutine(SkillContinuingTime());
        }
    }

    IEnumerator SkillContinuingTime()
    {
        yield return new WaitForSeconds(3);
        FreezeEnd();
    }

    void FreezeEnd()
    {
        _npcController.enabled = true;
        Debug.Log("�Ȥ��ä���");
    }

    void SkillCoolTime()
    {
        SkillCoolingTime = true;
        StartCoroutine(SkillCoolingTime2());
    }
    IEnumerator SkillCoolingTime2()
    {
        yield return new WaitForSeconds(45);
        SkillCoolTimeEnd();
    }

    void SkillCoolTimeEnd()
    {
        SkillCoolingTime = false;
    }
}
