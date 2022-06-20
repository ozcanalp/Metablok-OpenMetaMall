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

        Debug.Log(MyGettingStarted.initParams.imageIndex);

        characterController = GetComponent<CharacterController>();

        DisableAllPlayerObjects();

        if (!PV.IsMine)
        {
            playerInput.enabled = false;
            camera.SetActive(false);
            cinemachineFreeLook.SetActive(false);

            //dynamicCharacter.SetActive(true);
            //Avatars.transform.GetChild(MyGettingStarted.initParams.imageIndex).gameObject.SetActive(true);
            //defaultCharacter.SetActive(true);
        }
        else
        {
            EnablePlayer();
        }
    }

    void Start()
    {
        PV.RPC("SendImageIndex", RpcTarget.All);
    }

    [PunRPC]
    void SendImageIndex()
    {
        Debug.Log("RPC Message:" + MyGettingStarted.initParams.imageIndex);
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
