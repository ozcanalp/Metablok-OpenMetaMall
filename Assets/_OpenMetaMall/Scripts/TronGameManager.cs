using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TronGameManager : MonoBehaviour
{
    public static TronGameManager Instance;

    private void Awake()
    {
        if (null == Instance)
            Instance = this;
        else
            Destroy(gameObject);

    }

    private void Start()
    {
        //HideCursor();
    }

    public void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
