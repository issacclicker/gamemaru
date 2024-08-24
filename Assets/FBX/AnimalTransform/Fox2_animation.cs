using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox2_animation : MonoBehaviour
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

            if (stateInfo.IsName("Blend"))
            {
                _animator.SetFloat("Blend", blendValue);
            }
        }
    }
}
