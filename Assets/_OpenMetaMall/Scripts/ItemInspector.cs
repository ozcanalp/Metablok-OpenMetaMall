using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInspector : MonoBehaviour
{
    [SerializeField] PlayerLook playerLook;
    [SerializeField] RenderTexture rt;
    [SerializeField] GameObject itemInspectorBackground;
    [SerializeField] GameObject dragRotationArea;

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

        dragRotationArea.GetComponent<InspectingObjectRotation>().objectToRotate = inspectingObjectTransform;
    }

    public void EndInspectObject()
    {
        if (inspectingObject != null)
            Destroy(inspectingObject);
        OnItemInspect(true);
        itemInspectorBackground.SetActive(false);
    }
}
