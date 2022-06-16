using System;
using System.Collections.Generic;
using itSeez3D.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class ReactApi : MonoBehaviour
{
    public Text label;
    
    private Dictionary<int, System.Action<string>> cbDict = new Dictionary<int, Action<string>>();
    private int cbIndex;

    public static ReactApi Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void HandleCallback(string jsonData)
    {
        Debug.Log("Received JSON Data: " + jsonData);

        var response = JsonConvert.DeserializeObject<CallbackResponse>(jsonData);
        if (response == null) 
        {
            Debug.Log("Unable to parse JSON cbIndex. There must be no callback");
            return;
        }
        
        if (!string.IsNullOrEmpty(response.error))
        {
            label.text = response.error;
            return;
        }
            
        if (!cbDict.ContainsKey(response.cbIndex))
        {
            Debug.LogError("The cbIndex=" + response.cbIndex + " does not exist in cbDict");
        }

        cbDict[response.cbIndex]?.Invoke(jsonData);
        cbDict.Remove(cbIndex);
    }

    public void RequestPlugConnect(System.Action<string> cb)
    {
        PlugUtils.RequestConnect( GetCallbackIndex(cb) );
    }
    
    public void CheckPlugConnection(System.Action<string> cb)
    {
        PlugUtils.CheckConnection( GetCallbackIndex(cb) );
    }

    public void GetPlugNfts(System.Action<string> cb)
    {
        PlugUtils.GetPlugNfts( GetCallbackIndex(cb) );
    }

    public void Pay(System.Action<string> cb, string account, float amount)
    {
        PlugUtils.Pay(GetCallbackIndex(cb), account, amount);
    }

    private int GetCallbackIndex(System.Action<string> cb)
    {       
        cbIndex = (int)Mathf.Repeat(++cbIndex, 100);

        Debug.Log("cbIndex:" + cbIndex);

        if (cb != null)
        {
            
#if UNITY_EDITOR
            cb.Invoke(string.Empty);
            return cbIndex;
#endif
            cbDict.Add(cbIndex, cb);
        }

        return cbIndex;
    }
}
