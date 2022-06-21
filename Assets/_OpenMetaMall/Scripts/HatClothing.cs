using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatClothing : ClothingObject
{
    public GameObject hat;

    public override void WearItem()
    {
        foreach(Transform go in hat.transform.parent)
        {
            go.gameObject.SetActive(false);
        }

        hat.SetActive(true);
    }
}
