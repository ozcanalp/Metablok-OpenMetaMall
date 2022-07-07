
/// Set State Of Animator At Start By Name
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAnimatorState : MonoBehaviour
{
    [SerializeField]
    string StateName;

    int hashname;
    Animator m_animator;
    // Start is called before the first frame update
    void Start()
    {
        hashname = Animator.StringToHash(StateName);
        m_animator = GetComponent<Animator>();
        m_animator.Play(hashname);
    }

}
