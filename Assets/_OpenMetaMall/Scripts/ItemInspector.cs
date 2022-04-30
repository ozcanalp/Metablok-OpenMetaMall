using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemInspector : MonoBehaviour, IDragHandler
{
    [SerializeField] PlayerLook playerLook;
    [SerializeField] RenderTexture rt;
    [SerializeField] GameObject itemInspectorBackground;

    GameObject inspectingObject;
    Transform inspectingObjectTransform;

    public event Action<bool> OnItemInspect = delegate { };

    private void OnEnable()
    {
        playerLook.OnObjectInspect += StartInspectObject;
    }

    private void OnDisable()
    {
        playerLook.OnObjectInspect -= StartInspectObject;
    }

    private void StartInspectObject(InspectableObject obj)
    {
        OnItemInspect(false);

        itemInspectorBackground.SetActive(true);

        if (inspectingObject != null)
            Destroy(inspectingObject);

        inspectingObject = Instantiate(obj, Vector3.zero, Quaternion.identity).gameObject;
        inspectingObjectTransform = inspectingObject.transform;
        inspectingObjectTransform.parent = transform;
        inspectingObjectTransform.localPosition = Vector3.zero;
        inspectingObject.layer = LayerMask.NameToLayer("InspectingItem");
    }

    public void EndInspectObject()
    {
        if (inspectingObject != null)
            Destroy(inspectingObject);
        OnItemInspect(true);
        itemInspectorBackground.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        inspectingObjectTransform.eulerAngles += new Vector3(eventData.delta.y, -eventData.delta.x);
    }
}
