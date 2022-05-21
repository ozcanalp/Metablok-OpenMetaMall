using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastTravel : MonoBehaviour
{
  

    [SerializeField] Transform dest;
    [SerializeField] Transform playertrans;

    public void tele()
    {

        playertrans.position = dest.position;
        playertrans.rotation = dest.rotation;


    }
 
}
