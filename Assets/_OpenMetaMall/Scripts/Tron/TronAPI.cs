using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using System;

class UserData
{
    public string email;
    public string password;
}

class GetBids
{
    public string nftAddress;
}

class GiveBid
{
    public string amount;
    public string nftAddress;
}

public class TronAPI : MonoBehaviour
{
    public static TronAPI Instance;

    const string signupURI = "https://metabid-server.herokuapp.com/api/v1/users/signup";
    const string loginURI = "https://metabid-server.herokuapp.com/api/v1/users/login";
    const string getBidsURI = "https://metabid-server.herokuapp.com/api/v1/users/get-bids";
    const string giveBidURI = "https://metabid-server.herokuapp.com/api/v1/users/give-bid";
    const string testURI = "https://reqres.in/api/login";

    public Dictionary<string, string> returnDictionary;

    public event Action<string> OnResponse = delegate { };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    //Method can be "POST" or "GET"
    IEnumerator APIRequest(string URI, string json, string method, string token = "")
    {
        var req = new UnityWebRequest(URI, method);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        if (token.Length > 0)
            req.SetRequestHeader("Authorization", "Bearer " + token);

        //Send the request then wait here until it returns
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error While Sending: " + req.error);
            Debug.Log("Received: " + req.downloadHandler.text);
        }
        else
        {
            //string returnString = req.downloadHandler.text;

            string returnString =
            "{\"status\": \"success\"," +
            "\"token\": \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjYyZGM4MGQ5ZGUwOGFjMTRiNWNkZjI1MyIsImlhdCI6MTY1ODY2Mzk5NCwiZXhwIjoxNjY2NDM5OTk0fQ.NznrfXuissCLO6ejYYU17gO_6ljpM-k-1wM79xBSrE0\"," +
            "\"walletId\": \"123\"}";

            OnResponse(returnString);

            Debug.Log("Received: " + req.downloadHandler.text);

            returnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(returnString);

            /* foreach(string key in returnDictionary.Keys)
            {
                Debug.Log(key + " => " + returnDictionary[key]);
            } */

            if (returnDictionary["status"] == "success")
            {
                PlayerPrefs.SetString("walletId", "walletId");
                SceneManager.LoadSceneAsync("City");
            }
        }
    }

    public void Login(string email, string password)
    {
        //@TODO: call API login
        // Store Token
        // Add Token to headers

        var user = new UserData();
        user.email = email;
        user.password = password;

        string json = JsonUtility.ToJson(user);

        StartCoroutine(APIRequest(loginURI, json, "POST"));
    }

    public void SignUp(string email, string password)
    {
        //@TODO: call API login
        // Store Token
        // Add Token to headers

        var user = new UserData();
        user.email = email;
        user.password = password;

        string json = JsonUtility.ToJson(user);

        StartCoroutine(APIRequest(signupURI, json, "POST"));
    }

    public void GetBids(string nftAddress)
    {
        var getBids = new GetBids();
        getBids.nftAddress = nftAddress;

        string json = JsonUtility.ToJson(getBids);

        StartCoroutine(APIRequest(getBidsURI, json, "POST"));
    }

    public void GiveBid(string amount, string nftAddress)
    {
        var giveBid = new GiveBid();
        giveBid.amount = amount;
        giveBid.nftAddress = nftAddress;

        string json = JsonUtility.ToJson(giveBid);

        StartCoroutine(APIRequest(giveBidURI, json, "POST", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjYyZGM4MGQ5ZGUwOGFjMTRiNWNkZjI1MyIsImlhdCI6MTY1ODYxODA4MiwiZXhwIjoxNjY2Mzk0MDgyfQ.KNjntUsQT8eY_60uqHlXUdB9yxt7hKOIeqxkFgs4geo"));
    }
}