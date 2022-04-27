using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] PhotonView PV;
    [SerializeField] float movementSpeed = 1f;
    Vector2 movementInput;
    Vector3 movement;

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

        transform.Translate(movement * Time.deltaTime * movementSpeed);
    }
}
