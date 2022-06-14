using ItSeez3D.AvatarSdkSamples.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicPlayerAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // [SerializeField] Animator anim;
    public FullbodyAnimationManager anim;

    private void OnEnable()
    {
        if(GetComponent<PlayerMovement>() != null)
        {
            GetComponent<PlayerMovement>().OnWalking += MovementAnimation;
        }
    }

    private void OnDisable()
    {
        if (GetComponent<PlayerMovement>() != null)
        {
            GetComponent<PlayerMovement>().OnWalking -= MovementAnimation;
        }
    }

    private void MovementAnimation(bool isWalking)
    {
        // anim.SetBool("IsWalking", isWalking);
        if(isWalking)
        {
            // Debug.Log("IsWalking: " + isWalking);
            anim.PlayAnimationByName("Walking");
        }
    }

    public void PlayFacialAnimation()
    {
        // anim.SetBool("IsWalking", isWalking);
        
        Debug.Log("AAAAA");
        anim.PlayCurrentAnimation();
        //    anim.PlayAnimationByName("Walking");
        
    }
}
