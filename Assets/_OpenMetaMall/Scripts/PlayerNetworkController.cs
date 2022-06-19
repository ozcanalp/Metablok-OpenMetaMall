using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using ItSeez3D.AvatarSdkSamples.Core;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetworkController : MonoBehaviour
{
    [SerializeField] PhotonView PV;

    [SerializeField] PlayerInput playerInput;
    [SerializeField] GameObject camera;
    [SerializeField] GameObject cinemachineFreeLook;

    [SerializeField] GameObject dynamicCharacter;
    [SerializeField] GameObject customCharacter;
    [SerializeField] GameObject defaultCharacter;

    CharacterController characterController;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();

        DisableAllPlayerObjects();

        if (!PV.IsMine)
        {
            playerInput.enabled = false;
            camera.SetActive(false);
            cinemachineFreeLook.SetActive(false);

            defaultCharacter.SetActive(true);
        }
        else
        {
            EnablePlayer();
        }
    }

    void DisableAllPlayerObjects()
    {
        dynamicCharacter.SetActive(false);
        customCharacter.SetActive(false);
        defaultCharacter.SetActive(false);
    }

    void EnablePlayer()
    {
        if (MyGettingStarted.initParams != null && MyGettingStarted.initParams.isCustomPlayer)
        {
            customCharacter.SetActive(true);
            GameManager.Instance.avatarType = GameManager.AVATAR_TYPES.Custom;
        }
        else
        {
            dynamicCharacter.SetActive(true);
            GameManager.Instance.avatarType = GameManager.AVATAR_TYPES.Dynamic;
        }
    }

}
