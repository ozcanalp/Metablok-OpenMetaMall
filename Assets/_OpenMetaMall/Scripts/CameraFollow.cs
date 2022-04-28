using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
   [SerializeField] Transform objectToFollow;
   [SerializeField] Vector3 offsets = new Vector3(0, 1.5f, -4.75f);

   private void Update() {
       transform.position = objectToFollow.position + offsets;
       transform.rotation = objectToFollow.rotation;
   }
}
