using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] PlayerMovement playerMovement;

    private void OnEnable()
    {
        playerMovement.OnWalking += MovementAnimation;
    }

    private void OnDisable()
    {
        playerMovement.OnWalking -= MovementAnimation;
    }

    private void MovementAnimation(bool isWalking)
    {
        anim.SetBool("Walking", isWalking);
    }
}
