using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] PhotonView PV;
    [SerializeField] Transform camTransform;
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float turnSmoothTime = 0.1f;
    [SerializeField] CharacterController characterController;


    Vector2 movementInput;
    Vector3 movement;
    Vector3 moveDir;

    float turnSmoothVelocity;

    void Reset()
    {
        PV = GetComponent<PhotonView>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (PV.IsMine)
            Move(movementInput);
    }

    public void GetMovementInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    void Move(Vector2 movementInput)
    {
        movement.x = movementInput.x;
        movement.y = 0;
        movement.z = movementInput.y;

        if (movement.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveDir.normalized * Time.deltaTime * movementSpeed);
        }
    }
}
