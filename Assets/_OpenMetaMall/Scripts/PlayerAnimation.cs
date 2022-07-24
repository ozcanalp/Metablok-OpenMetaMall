using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerController playerController;

    private void OnEnable()
    {
        playerMovement.OnWalking += MovementAnimation;
        playerController.OnSit += SitAnimation;
    }

    private void OnDisable()
    {
        playerMovement.OnWalking -= MovementAnimation;
        playerController.OnSit -= SitAnimation;
    }

    private void MovementAnimation(bool isWalking)
    {
        anim.SetBool("Walking", isWalking);
    }

    private void SitAnimation(bool isSitting)
    {
        anim.SetBool("Sitting", isSitting);
    }
}
