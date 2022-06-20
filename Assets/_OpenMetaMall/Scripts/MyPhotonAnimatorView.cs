using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MyPhotonAnimatorView : PhotonAnimatorView
{

    public Animator myAnimator;

    private void Awake()
    {
        Debug.LogWarning("myAnimator");

        m_Animator = myAnimator;
    }

}
