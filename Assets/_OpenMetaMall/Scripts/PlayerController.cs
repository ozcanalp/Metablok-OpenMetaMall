using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] ItemInspector itemInspector;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerLook playerLook;
    [SerializeField] CinemachineInputProvider cinemachineInputProvider;


    private void OnEnable()
    {
        itemInspector.OnItemInspect += HandlePlayerComponent;
    }

    private void OnDisable()
    {

        itemInspector.OnItemInspect -= HandlePlayerComponent;
    }

    private void HandlePlayerComponent(bool obj)
    {
        Debug.Log(obj);
        if (!obj)
        {
            playerMovement.enabled = false;
            playerLook.enabled = false;
            cinemachineInputProvider.enabled = false;
        }
        else
        {
            playerMovement.enabled = true;
            playerLook.enabled = true;
            cinemachineInputProvider.enabled = true;
        }
    }
}
