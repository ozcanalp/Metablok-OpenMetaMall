using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugFinder : MonoBehaviour
{
    public GameObject customPlayer;
    public GameObject customThirdPerson;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("customAvatar status:" + customPlayer.activeSelf);
        Debug.Log("customAvatar-PlayerInput status:" + customThirdPerson.GetComponent<UnityEngine.InputSystem.PlayerInput>().enabled);

        customThirdPerson.GetComponent<UnityEngine.InputSystem.PlayerInput>().enabled = true;

        Debug.Log("customAvatar-PlayerInput status:" + customThirdPerson.GetComponent<UnityEngine.InputSystem.PlayerInput>().enabled);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
