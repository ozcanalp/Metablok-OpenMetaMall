using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using UnityEngine;

public class PlayerNetworkController : MonoBehaviour
{
    [SerializeField] PhotonView PV;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] CinemachineFreeLook cinemachineFreeLook;
    [SerializeField] Camera cam;
    [SerializeField] Camera itemInspectorCamera;

    [SerializeField] GameObject ThirdPerson;
    [SerializeField] GameObject VR;

    void Start()
    {
        if (!PV.IsMine)
        {
            playerMovement.enabled = false;
            cinemachineFreeLook.enabled = false;
            cam.enabled = false;
            itemInspectorCamera.enabled = false;

            VR.SetActive(false);
        }
        else
        {
            #if UNITY_EDITOR
            ThirdPerson.SetActive(false);
            #endif
        }
    }

}
