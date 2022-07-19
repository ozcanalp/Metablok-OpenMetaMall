using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShirtClothing : ClothingObject
{
    public Renderer characterShirtRenderer;
    [SerializeField] Material shirtMaterial;


    public override void WearItem()
    {
        characterShirtRenderer.material = shirtMaterial;
    }
}
