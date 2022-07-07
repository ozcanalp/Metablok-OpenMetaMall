using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using ItSeez3D.AvatarSdkSamples.Core;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class TronPlayerNetworkController : MonoBehaviourPunCallbacks
{
    [SerializeField] PhotonView PV;

    [SerializeField] PlayerInput playerInput;
    [SerializeField] GameObject camera;
    [SerializeField] GameObject cinemachineFreeLook;

    [SerializeField] GameObject customCharacter;

    void Awake()
    {
        if (!PV.IsMine)
        {
            playerInput.enabled = false;
            camera.SetActive(false);
            cinemachineFreeLook.SetActive(false);
        }
        else
        {
        }
    }
}
