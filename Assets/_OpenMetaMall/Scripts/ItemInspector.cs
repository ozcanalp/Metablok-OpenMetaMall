using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemInspector : MonoBehaviour
{
    [SerializeField] PlayerLook playerLook;
    [SerializeField] RenderTexture rt;
    [SerializeField] GameObject itemInspectorBackground;
    [SerializeField] GameObject dragRotationArea;
    [SerializeField] TextMeshProUGUI itemPriceText;
    [SerializeField] Button btn_Try;

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
        GameManager.Instance.ShowCursor();

        OnItemInspect(false);

        //itemPriceText.SetText(UnityEngine.Random.Range(20, 30).ToString());
        itemPriceText.SetText("0.1 ICP");

        itemInspectorBackground.SetActive(true);

        if (inspectingObject != null)
            Destroy(inspectingObject);

        inspectingObject = Instantiate(obj, Vector3.zero, Quaternion.identity).gameObject;
        inspectingObjectTransform = inspectingObject.transform;
        inspectingObjectTransform.parent = transform;
        inspectingObjectTransform.localPosition = Vector3.zero;

        //inspectingObject.layer = LayerMask.NameToLayer("InspectingItem");
        SetLayerRecursively(inspectingObject, LayerMask.NameToLayer("InspectingItem"));

        ClothingObject clothingObject;
        if (inspectingObject.GetComponent<ClothingObject>())
        {
            clothingObject = inspectingObject.GetComponent<ClothingObject>();
            btn_Try.onClick.AddListener((clothingObject).WearItem);
        }

        dragRotationArea.GetComponent<InspectingObjectRotation>().objectToRotate = inspectingObjectTransform;
    }

    public void EndInspectObject()
    {
        if (inspectingObject != null)
            Destroy(inspectingObject);
        OnItemInspect(true);
        itemInspectorBackground.SetActive(false);

        GameManager.Instance.HideCursor();
    }

    public void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
