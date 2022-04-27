using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] Transform cameraContainerTransform;

    [Range(0, 90)]
    [SerializeField] float verticalCameraLimit = 50;

    Vector3 cameraRotation;
    float cameraRotationX;
    Vector2 lookInput;
    Vector3 look;

    void Update()
    {
        Look(lookInput);
    }

    public void GetLookInput(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    void Look(Vector2 lookInput)
    {
        cameraRotation = cameraContainerTransform.localEulerAngles;
        cameraRotation.x += lookInput.y;
        cameraRotation.y += lookInput.x;

         if (cameraRotation.x < 180 && cameraRotation.x > 0)
        {
           cameraRotation.x = Mathf.Clamp(cameraRotation.x, 0, verticalCameraLimit);
        }
        else if(cameraRotation.x < 360 && cameraRotation.x > 180)
        {
            cameraRotation.x = Mathf.Clamp(cameraRotation.x, 360-verticalCameraLimit, 360);
        }

        cameraContainerTransform.localEulerAngles = cameraRotation;
    }

}
