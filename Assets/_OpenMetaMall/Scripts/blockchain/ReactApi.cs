using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReactApi : MonoBehaviour
{
    public TMP_Text label;
    
    private Dictionary<int, System.Action<string>> cbDict = new Dictionary<int, Action<string>>();
    private int cbIndex;

    public static ReactApi Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void HandleCallback(string jsonData)
    {
        Debug.Log("HandleCallback: Received JSON Data:" + jsonData);

        label.text = label.text + " " +jsonData;

        var response = JsonUtility.FromJson<CallbackResponse>(jsonData);
        // var response = JsonConvert.DeserializeObject<CallbackResponse>(jsonData);
        if (response == null) 
        {
            Debug.Log("Unable to parse JSON cbIndex. There must be no callback");
            return;
        }

        label.text += response.result;

        // Debug.Log("HandleCallback: result:" + response.result);

        /* if ("true".Equals(response.result) || "allowed".Equals(response.result))
        {
            label.text = response.result;
            return;
        }*/

        Debug.Log("HandleCallback: result:" + response.result);

        /* if (!cbDict.ContainsKey(response.cbIndex))
        {
            Debug.LogError("HandleCallback: The cbIndex=" + response.cbIndex + " does not exist in cbDict");
        }*/

        // cbDict[response.cbIndex]?.Invoke(jsonData);
        // cbDict.Remove(cbIndex);
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
