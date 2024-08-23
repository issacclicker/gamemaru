using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class player_animation : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Animator _animator;

    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerMovement != null)
        {
            float blendValue = playerMovement.BlendValue;
            _animator.SetFloat("Blend", blendValue);

            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Eat")||stateInfo.IsName("Die")|| stateInfo.IsName("Jump"))
            {
                _animator.SetFloat("Blend", 0);
            }
            else if (stateInfo.IsName("Blend"))
            {
                _animator.SetFloat("Blend", blendValue);
            }
        }
    }

    // Eat 애니메이션 트리거
    public void PlayEatAnimation()
    {
        Debug.Log("Eat animation triggered");
        _animator.Play("Eat");
    }

    public void StopEatAnimation()
    {
        // Eat 애니메이션이 끝난 후 Blend 애니메이션으로 돌아가도록 설정
        _animator.SetFloat("Blend", playerMovement.BlendValue);
    }

    // Jump 애니메이션 트리거
    public void PlayJumpAnimation()
    {
        Debug.Log("Jump animation triggered");
        _animator.Play("Jump");
    }

    public void StopJumpAnimation()
    {
        // Jump 애니메이션이 끝난 후 Blend 애니메이션으로 돌아가도록 설정
        _animator.SetFloat("Blend", playerMovement.BlendValue);
    }

    // Die 애니메이션 트리거
    public void PlayDieAnimation()
    {
        _animator.SetTrigger("Die");
    }
}
