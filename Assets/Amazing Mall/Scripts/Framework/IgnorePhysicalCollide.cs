/// Ignoring Collisions Between Childs and Taget Colliders

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnorePhysicalCollide : MonoBehaviour
{
    [SerializeField]
    Collider[] TargetColliders;
    [SerializeField]
    Collider[] ChildColliders;

    void Start()
    {
        ChildColliders = GetComponentsInChildren<Collider>();
        foreach (var item in ChildColliders)
        {
            foreach (var itemB in TargetColliders)
            {
                Physics.IgnoreCollision(item, itemB);
            }
            
        }    
    }
}
