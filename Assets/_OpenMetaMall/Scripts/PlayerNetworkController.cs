using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using UnityEngine;

public class PlayerNetworkController : MonoBehaviour
{
    PhotonView PV;
    PlayerMovement playerMovement;
    CinemachineFreeLook cinemachineFreeLook;
    Camera cam;

    void Start()
    {
        PV = GetComponentInChildren<PhotonView>();
        playerMovement = GetComponentInChildren<PlayerMovement>();
        cinemachineFreeLook = GetComponentInChildren<CinemachineFreeLook>();
        cam = GetComponentInChildren<Camera>();

        if (!PV.IsMine)
        {
            playerMovement.enabled = false;
            cinemachineFreeLook.enabled = false;
            cam.enabled = false;
        }
    }

}
