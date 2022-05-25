using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerLook))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] ItemInspector itemInspector;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerLook playerLook;
    [SerializeField] Canvas playerHud;
    [SerializeField] CinemachineInputProvider cinemachineInputProvider;

    private void Reset()
    {
        itemInspector = GetComponentInChildren<ItemInspector>();
        playerMovement = GetComponent<PlayerMovement>();
        playerLook = GetComponent<PlayerLook>();
        cinemachineInputProvider = GetComponentInChildren<CinemachineInputProvider>();
    }

    private void Start()
    {
        Reset();
    }

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
            playerLook.currentlyInspecting = true;
            playerLook.enabled = false;
            cinemachineInputProvider.enabled = false;
            playerHud.enabled = false;
        }
        else
        {
            playerMovement.enabled = true;
            playerLook.currentlyInspecting = false;
            playerLook.enabled = true;
            cinemachineInputProvider.enabled = true;
            playerHud.enabled = true;
        }
    }
}
