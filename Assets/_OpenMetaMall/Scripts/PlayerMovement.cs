using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using System;
using ExitGames.Client.Photon;
using Photon.Realtime;
using ItSeez3D.AvatarSdkSamples.Core;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] PhotonView PV;
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform camTransform;
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float turnSmoothTime = 0.1f;
    [SerializeField] CharacterController characterController;


    Vector2 movementInput;
    Vector3 movement;
    Vector3 moveDir;
    Vector3 gravity = Physics.gravity;

    public event Action<bool> OnWalking = delegate { };

    float turnSmoothVelocity;

    void Reset()
    {
        PV = GetComponent<PhotonView>();
        characterController = GetComponent<CharacterController>();
        camTransform = GetComponentInChildren<Camera>().transform;
    }

    private void Update()
    {
        if (PV.IsMine)
            Move(movementInput);
    }

    public void GetMovementInput(InputAction.CallbackContext context)
    {
        /*  if (context.started)
         {
             SendWalkingAnimation(true);
         }
         else if (context.canceled)
         {
             SendWalkingAnimation(false);
         } 
         */

        if (Keyboard.current.IsPressed())
            Debug.LogError("Input Event Received");

        movementInput = context.ReadValue<Vector2>();
    }
    /* 
        void SendWalkingAnimation(bool isWalking)
        {
            byte eventCode = 197; // make up event codes at will
            object[] content = new object[] { MyGettingStarted.initParams.avatarCode ,isWalking }; // Array contains the target position and the IDs of the selected units
            System.Collections.Hashtable evData = new System.Collections.Hashtable(); // put your data into a key-value hashtable
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; // You would have to set the Receivers to All in order to receive this event on the local client as well

            PhotonNetwork.RaiseEvent(eventCode, content, raiseEventOptions, SendOptions.SendReliable);
        }
     */
    void Move(Vector2 movementInput)
    {
        movement.x = movementInput.x;
        movement.z = movementInput.y;

        if (movement.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            playerTransform.rotation = Quaternion.Euler(0f, angle, 0f);
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            characterController.Move((moveDir.normalized + gravity) * Time.deltaTime * movementSpeed);
            OnWalking(true);
        }
        else
        {
            OnWalking(false);
        }
    }
}
