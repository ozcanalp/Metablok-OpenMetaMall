using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InspectingObjectRotation : MonoBehaviour, IDragHandler
{
    public Transform objectToRotate;

    public void OnDrag(PointerEventData eventData)
    {
        objectToRotate.eulerAngles += new Vector3(eventData.delta.y, -eventData.delta.x);
    }
}
