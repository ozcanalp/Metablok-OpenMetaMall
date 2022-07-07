using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupernovaGameManager : MonoBehaviour
{
    public static SupernovaGameManager Instance;

    public enum AVATAR_TYPES { Custom, Dynamic };
    public AVATAR_TYPES avatarType;
    public int imageIndex = 0;
    public GameObject lastCreatedDynamicAvatar;

    public Dictionary<string, GameObject> avatars;

    private void Awake()
    {
        if (null == Instance)
            Instance = this;
        else
            Destroy(gameObject);

        avatars = new Dictionary<string, GameObject>();
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
