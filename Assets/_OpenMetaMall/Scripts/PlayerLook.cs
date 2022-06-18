using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] PhotonView PV;
    [SerializeField] GameObject crosshair;
    [SerializeField] Camera cam;
    [SerializeField] ItemInspector itemInspector;
    [SerializeField] float interactionDistance = 10f;

    public bool currentlyInspecting = false;

    RaycastHit hitInfo;

    private void Reset()
    {
        PV = GetComponent<PhotonView>();
        cam = GetComponentInChildren<Camera>();
    }

    public void GetFireInput(InputAction.CallbackContext context)
    {
        if (PV.IsMine)
        {
            if (context.performed)
                ShootRaycast();
        }
    }

    void ShootRaycast()
    {
        if(currentlyInspecting == true)
            return;

        Ray ray = cam.ScreenPointToRay(crosshair.transform.position);
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.blue, 1f);
        if (Physics.Raycast(ray, out hitInfo, interactionDistance))
        {
            InspectableObject hitObject = hitInfo.collider.GetComponent<InspectableObject>();
            if (hitObject != null)
            {
                Debug.Log(hitObject.name);
                itemInspector.StartInspectObject(hitObject);
            }
        }
    }

}
