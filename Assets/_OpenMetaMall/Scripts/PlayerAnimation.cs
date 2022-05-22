using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator anim;

    private void OnEnable()
    {
        GetComponent<PlayerMovement>().OnWalking += MovementAnimation;
    }

    private void OnDisable()
    {
        GetComponent<PlayerMovement>().OnWalking -= MovementAnimation;
    }

    private void MovementAnimation(bool isWalking)
    {
        //Debug.Log("IsWalking: " + isWalking);
        anim.SetBool("IsWalking", isWalking);
    }
}