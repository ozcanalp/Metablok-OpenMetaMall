using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this scrip allows to attach all child renderers Lightprops info to specific gameobject (Reduce draw calls)
public class RealtimeMeshRendererModifier : MonoBehaviour
{

    [SerializeField]
    Transform anchorOverride;
    [SerializeField]
    GameObject proxyLight;

    MeshRenderer[] allchildRenderers;

    // Start is called before the first frame update
    void Awake()
    {
     
        allchildRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var item in allchildRenderers)
        {
            item.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Simple;
            item.probeAnchor = anchorOverride;
           
            if (proxyLight)
            {
                item.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.UseProxyVolume;
                item.lightProbeProxyVolumeOverride = proxyLight;

            }
        }
    }

}
