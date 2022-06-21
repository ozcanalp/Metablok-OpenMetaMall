using ItSeez3D.AvatarSdkSamples.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicPlayerAnimation : MonoBehaviour
{
    public Animator animator = null;
    public FullbodyAnimationManager anim;

    private void OnEnable()
    {
        if (GetComponentInParent<PlayerMovement>() != null)
        {
            GetComponentInParent<PlayerMovement>().OnWalking += MovementAnimation;
        }
    }

    private void OnDisable()
    {
        if (GetComponentInParent<PlayerMovement>() != null)
        {
            GetComponentInParent<PlayerMovement>().OnWalking -= MovementAnimation;
        }
    }

    private void MovementAnimation(bool isWalking)
    {
        if (animator == null)
        {
            if (anim == null)
                return;
            else
                anim.gameObject.TryGetComponent<Animator>(out animator);
        }
        else
        {
            animator.SetBool("Walking", isWalking);
        }

        /* if(isWalking)
        {
            // Debug.Log("IsWalking: " + isWalking);
            anim.PlayAnimationByName("Walking");
        } */

    }

    public void PlayFacialAnimation()
    {
        anim.PlayCurrentAnimation();
    }
}
