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

    public event Action<bool> OnSit = delegate { };

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
        playerMovement = GetComponent<PlayerMovement>();
        playerLook = GetComponent<PlayerLook>();
        cinemachineInputProvider = GetComponentInChildren<CinemachineInputProvider>();
    }

    private void OnEnable()
    {
        itemInspector.OnItemInspect += HandlePlayerComponent;
        AuctionScreen.Instance.OnStandUp += StandUp;
    }

    private void OnDisable()
    {

        itemInspector.OnItemInspect -= HandlePlayerComponent;
        AuctionScreen.Instance.OnStandUp += StandUp;
    }

    public void SitDown(EnterAuction obj)
    {
        transform.parent = obj.transform;
        transform.localPosition = new Vector3(-.1f, .2f, 0);
        transform.localRotation = Quaternion.Euler(0, -90, 0);
        OnSit(true);
        HandlePlayerComponent(false);
        TronGameManager.Instance.ShowCursor();
    }

    public void StandUp()
    {
        transform.parent = null;
        OnSit(false);
        HandlePlayerComponent(true);
        TronGameManager.Instance.HideCursor();
    }

    public void HandlePlayerComponent(bool obj)
    {
        if (cinemachineInputProvider == null)
        {
            cinemachineInputProvider = GetComponentInChildren<CinemachineInputProvider>();

            if (cinemachineInputProvider == null)
                return;
        }

        if (obj)
        {
            playerMovement.enabled = true;
            playerLook.currentlyInspecting = false;
            playerLook.enabled = true;
            cinemachineInputProvider.enabled = true;
            playerHud.enabled = true;
        }
        else
        {
            playerMovement.enabled = false;
            playerLook.currentlyInspecting = true;
            playerLook.enabled = false;
            cinemachineInputProvider.enabled = false;
            playerHud.enabled = false;

        }
    }
}
