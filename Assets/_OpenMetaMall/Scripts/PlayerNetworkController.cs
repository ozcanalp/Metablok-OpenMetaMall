using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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

    void Start()
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
            dynamicCharacter.SetActive(true);
        }
    }

    void DisableAllPlayerObjects()
    {
        dynamicCharacter.SetActive(false);
        customCharacter.SetActive(false);
        defaultCharacter.SetActive(false);
    }

}
