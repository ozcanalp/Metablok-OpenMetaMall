//This script will animate the material properties
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedPropertyBlock : MonoBehaviour
{
    public Renderer[] _Renderers;
    public string _materialName;
    [SerializeField]
    [ColorUsage(false, true)]
    Color FirstColor, SecondColor;
    [SerializeField]
    string PropertyName;
    [SerializeField]
    float Speed;

    Color CurrentColor, TargetColor;

    int LastMaterialNumber;

    // Start is called before the first frame update
    private void Start()
    {
        CurrentColor = FirstColor;
        TargetColor = SecondColor;
        StartCoroutine(AnimationProgress());
    }

    void ChangeTargetColor()
    {
        TargetColor = TargetColor == FirstColor ? SecondColor : FirstColor;

    }

    IEnumerator AnimationProgress()
    {
        while (true)
        {
            var MPB = new MaterialPropertyBlock();
           
            for (int i = 0; i < _Renderers.Length; i++)
            {
                for (int j = 0; j < _Renderers[i].sharedMaterials.Length; j++)
                {

                    var Materials = _Renderers[i].sharedMaterials[j];
                    
                    if (Materials.name.Equals(_materialName))
                    {
                        CurrentColor = Color.Lerp(CurrentColor, TargetColor, Time.deltaTime * Speed);
                        MPB.SetColor(PropertyName, CurrentColor);
                        LastMaterialNumber = j;
                    }
                    var Colordistance = Vector4.Distance(CurrentColor, TargetColor) < 1 ? true : false;
                    if (Colordistance)
                        ChangeTargetColor();

                    _Renderers[i].SetPropertyBlock(MPB, LastMaterialNumber);
                }
            }
            yield return null;
        }

    }
}
