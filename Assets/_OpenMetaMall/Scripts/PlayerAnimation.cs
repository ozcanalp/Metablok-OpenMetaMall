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
        //Debug.Log("IsWalking: " + isWalking);
        anim.SetBool("IsWalking", isWalking);
    }
}
