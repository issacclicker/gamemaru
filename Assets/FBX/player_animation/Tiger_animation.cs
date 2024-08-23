using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiger_animation : MonoBehaviour
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
            if (stateInfo.IsName("Jump")|| stateInfo.IsName("Attack") || stateInfo.IsName("Roar"))
            {
                _animator.SetFloat("Blend", 0);
            }
            else if (stateInfo.IsName("Blend"))
            {
                _animator.SetFloat("Blend", blendValue);
            }
        }
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

    // Attack 애니메이션 트리거
    public void PlayAttackAnimation()
    {
        Debug.Log("Attack animation triggered");
        _animator.Play("Attack");
    }

    public void StopAttackAnimation()
    {
        // Attack 애니메이션이 끝난 후 Blend 애니메이션으로 돌아가도록 설정
        _animator.SetFloat("Blend", playerMovement.BlendValue);
    }

    // Roar 애니메이션 트리거
    public void PlayRoarAnimation()
    {
        Debug.Log("Roar animation triggered");
        _animator.Play("Roar");
    }

    public void StopRoarAnimation()
    {
        // Roar 애니메이션이 끝난 후 Blend 애니메이션으로 돌아가도록 설정
        _animator.SetFloat("Blend", playerMovement.BlendValue);
    }
}
