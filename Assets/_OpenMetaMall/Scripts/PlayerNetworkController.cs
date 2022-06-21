using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using ItSeez3D.AvatarSdkSamples.Core;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetworkController : MonoBehaviourPunCallbacks
{
    [SerializeField] PhotonView PV;

    [SerializeField] PlayerInput playerInput;
    [SerializeField] GameObject camera;
    [SerializeField] GameObject cinemachineFreeLook;

    [SerializeField] GameObject dynamicCharacter;
    [SerializeField] GameObject customCharacter;
    [SerializeField] GameObject defaultCharacter;

    [SerializeField] GameObject Avatars;

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
        }
        else
        {
            EnablePlayer();
        }
    }

    void DisableAllPlayerObjects()
    {
        //Supernova Dynamic Player
        dynamicCharacter.SetActive(false);
        /* customCharacter.SetActive(false);
        defaultCharacter.SetActive(false); */

        foreach (Transform avatar in Avatars.transform)
        {
            avatar.gameObject.SetActive(false);
        }
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
