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
    [SerializeField] CinemachineInputProvider cinemachineInputProvider;
    [SerializeField] Canvas playerHud;

    private void Awake()
    {
        itemInspector = GameObject.FindGameObjectWithTag("ItemInspector").GetComponent<ItemInspector>();
    }

    private void Reset()
    {
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
