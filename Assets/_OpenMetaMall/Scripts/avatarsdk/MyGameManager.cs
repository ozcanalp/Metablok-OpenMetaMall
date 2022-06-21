using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static MyGameManager Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void Add(int hashcode, string code)
    {
        PlayerPrefs.SetString(string.Format("avatar-{0}", hashcode.ToString()), code);
    }

    public string Get(int hashcode)
    {
        return PlayerPrefs.GetString(string.Format("avatar-{0}", hashcode.ToString()), null);
    }
}
