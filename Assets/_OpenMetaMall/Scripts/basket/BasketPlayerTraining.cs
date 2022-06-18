using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketPlayerTraining : MonoBehaviour
{
    public Animator basketPlayer;
    public Animator basketBall;

    public BAS currentAnim;

    public BAS[] animList;
    int currentAnimIndex = 0;

    public enum BAS
    {
        Chest_Pass_Left,
        Chest_Pass_Right,
        Defense,
        Dribble,
        Idle_With_Ball,
        Jog_BWD_No_Ball,
        Jog_BWD_Start_No_Ball,
        Jog_BWD_Start_With_Ball,
        Jog_BWD_Stop_No_Ball,
        Jog_BWD_Stop_With_Ball,
        Jog_BWD_With_Ball,
        Jog_FWD_No_Ball,
        Jog_FWD_Start_No_Ball,
        Jog_FWD_Start_With_Ball,
        Jog_FWD_Stop_No_Ball,
        Jog_FWD_Stop_With_Ball,
        Jog_FWD_With_Ball,
        Jog_L_No_Ball,
        Jog_L_Start_No_Ball,
        Jog_L_Start_With_Ball,
        Jog_L_Stop_No_Ball,
        Jog_L_Stop_With_Ball,
        Jog_L_With_Ball,
        Jog_R_No_Ball,
        Jog_R_Start_No_Ball,
        Jog_R_Start_With_Ball,
        Jog_R_Stop_No_Ball,
        Jog_R_Stop_With_Ball,
        Jog_R_With_Ball,
        Jump_Shot,
        Receive_The_Ball_Left,
        Receive_The_Ball_Right,
        Standing_Idle_No_Ball,
        Standing_Idle_With_Ball,
    };

    // Start is called before the first frame update
    void Start()
    {

        // basketPlayer.set

        // SetAnimation(AllAnimations[Random.Range(0, AllAnimations.Length)]);

        // SetAnimation("Dribble");

        //Debug.Log("Basket started!");

        StartAnimation();
    }

    public void StartAnimation()
    {
        StartCoroutine(DummyActions());
    }

    private IEnumerator DummyActions()
    {
        while (true)
        {
            foreach (BAS anim in animList)
            {
                SetAnimation(anim);
                yield return new WaitForSeconds(basketPlayer.GetCurrentAnimatorStateInfo(0).length);
            }
        }
/* 
        Debug.Log("Basket anim!");

        SetAnimation(BAS.Idle_With_Ball);
        yield return new WaitForSeconds(2);
        SetAnimation(BAS.Jog_FWD_Start_With_Ball);
        yield return new WaitForSeconds(2);
        SetAnimation(BAS.Standing_Idle_No_Ball);
        yield return new WaitForSeconds(2);
         */
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentAnim)
        {
            case BAS.Jog_FWD_Start_With_Ball:
                break;
            default:
                break;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            SetAnimation(currentAnim);
    }

    private void SetAnimation(BAS anim)
    {
        string animation = anim.ToString().Replace('_', ' ');
        //Debug.Log("basket anim:" + animation);

        currentAnim = anim;

        basketPlayer.SetTrigger(animation);
        basketBall.SetTrigger(animation);
        // yield return new WaitUntil(() => basketPlayer.GetCurrentAnimatorStateInfo(0).IsName(animation) && basketPlayer.GetCurrentAnimatorStateInfo(0).normalizedTime > 1);
        // yield return new WaitUntil(() => basketBall.GetCurrentAnimatorStateInfo(0).IsName(animation) && basketBall.GetCurrentAnimatorStateInfo(0).normalizedTime > 1);
    }
}
