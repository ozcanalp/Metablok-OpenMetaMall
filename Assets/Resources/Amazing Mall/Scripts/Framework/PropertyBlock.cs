/// using for blocking materials properties
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PropertyBlock : MonoBehaviour
{

    [SerializeField]
    Renderer[] _Renderer;
    [SerializeField]
    string MaterialName;

    [SerializeField]
    Base[] Blocks;
    [System.Serializable]
    struct Base
    {
        public string PropertyName;
        public float PropertySet;
        public bool IsColor;
        [ColorUsage(true,true)]
        public Color PropertyColor;
        public bool RandomColor;

    }

    List<int> MaterialNumber = new List<int>();
    Color[] ColorPack;
    int randomColorIndex;

    void Awake()
    {
        RainbowColors(out ColorPack,true);
    }
    void Start()
    {

        //Get materials on renderers 
        for (int i = 0; i < _Renderer.Length; i++)
        {
            for (int j = 0; j < _Renderer[i].sharedMaterials.Length; j++)
            {
                if (_Renderer[i].sharedMaterials[j].name.Equals(MaterialName))
                    MaterialNumber.Add(j);

            }
        }
         var MPB = new MaterialPropertyBlock();

        //unpacking the blocks and set the colors and properties (floats) on material(s) 
        foreach (var item in Blocks)
        {
            if (item.IsColor)
            {
                if (item.RandomColor)
                {
                    randomColorIndex = Random.Range(0,ColorPack.Length);
                    MPB.SetColor(item.PropertyName,ColorPack[randomColorIndex]);
                    continue;
                }

                MPB.SetColor(item.PropertyName, item.PropertyColor);
                continue;
            }
            MPB.SetFloat(item.PropertyName, item.PropertySet);
        }
        for (int i = 0; i < _Renderer.Length; i++)
        {
            _Renderer[i].SetPropertyBlock(MPB,MaterialNumber[i]);
        }
      

    }


    public void RainbowColors(out Color[] Colors, bool ExcludeWhite = false)
    {
        Colors = new Color[] {
            Color.cyan,
            Color.green,
            Color.magenta,
            Color.yellow,
            ExcludeWhite ? Color.yellow :Color.white,
            Color.red,
            new Vector4 ( 1, 0, .82f),
            new Vector4 ( .92f, .6f, 0),
    };
    }
}
