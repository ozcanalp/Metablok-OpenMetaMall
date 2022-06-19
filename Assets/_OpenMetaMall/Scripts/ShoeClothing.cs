using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoeClothing : ClothingObject
{
    public GameObject leftShoe;
    public GameObject rightShoe;

    public override void WearItem()
    {
        foreach(Transform go in leftShoe.transform.parent)
        {
            go.gameObject.SetActive(false);
        }

        foreach(Transform go in rightShoe.transform.parent)
        {
            go.gameObject.SetActive(false);
        }

        leftShoe.SetActive(true);
        rightShoe.SetActive(true);
    }
   
}
