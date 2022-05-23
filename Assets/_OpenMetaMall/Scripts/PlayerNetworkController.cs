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

    IEnumerator Start()
    {
        VR.SetActive(false);
        ThirdPerson.SetActive(false);

        yield return new WaitForSeconds(1);

        if (!PV.IsMine)
        {
            playerMovement.enabled = false;
            cinemachineFreeLook.enabled = false;
            cam.enabled = false;
            itemInspectorCamera.enabled = false;

            ThirdPerson.SetActive(true);
        }
        else
        {

#if UNITY_EDITOR
            VR.SetActive(true);

#elif UNITY_WEBGL
            ThirdPerson.SetActive(true);
#endif
        }
    }

}
