using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed = 1f;
    Vector2 movementInput;
    Vector3 movement;

    private void Update() {
        transform.Translate(movement * Time.deltaTime * movementSpeed);
    }

    public void Movement(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();

        movement.x = movementInput.x;
        movement.y = 0;
        movement.z = movementInput.y;
    }
}
