using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShirtClothing : ClothingObject
{
    public Renderer characterShirtRenderer;
    [SerializeField] Material shirtMaterial;

    public override void WearItem()
    {
        characterShirtRenderer.material = shirtMaterial;
    }
}
