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

    string playerToken;

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
            string returnString = req.downloadHandler.text;
            OnResponse(returnString);

            returnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(returnString);

            foreach (string key in returnDictionary.Keys)
            {
                Debug.Log(key + " => " + returnDictionary[key]);
            }

            if (returnDictionary["status"] == "success")
            {
                PlayerPrefs.SetString("walletId", "walletId");
                PlayerPrefs.SetString("token", returnDictionary["token"]);
                //SceneManager.LoadSceneAsync("City");
            }
        }
    }

    public IEnumerator Login(string email, string password)
    {
        //@TODO: call API login
        // Store Token
        // Add Token to headers

        var user = new UserData();
        user.email = email;
        user.password = password;

        string json = JsonUtility.ToJson(user);

        var req = new UnityWebRequest(loginURI, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");


        Debug.Log("request sent");

        //Send the request then wait here until it returns
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error While Sending: " + req.error);
        }
        else
        {
            string returnString = req.downloadHandler.text;
            OnResponse(returnString);

            returnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(returnString);

            foreach (string key in returnDictionary.Keys)
            {
                Debug.Log(key + " => " + returnDictionary[key]);
            }

            playerToken = returnDictionary["token"];
            //PlayerPrefs.SetString("token", returnDictionary["token"]);
            SceneManager.LoadSceneAsync("City");
        }
    }

    public IEnumerator SignUp(string email, string password)
    {
        //@TODO: call API login
        // Store Token
        // Add Token to headers

        var user = new UserData();
        user.email = email;
        user.password = password;

        string json = JsonUtility.ToJson(user);

        var req = new UnityWebRequest(signupURI, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error While Sending: " + req.error);
        }
        else
        {
            string returnString = req.downloadHandler.text;
            OnResponse(returnString);

            returnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(returnString);

            foreach (string key in returnDictionary.Keys)
            {
                Debug.Log(key + " => " + returnDictionary[key]);
            }

            playerToken = returnDictionary["token"];
            //PlayerPrefs.SetString("token", returnDictionary["token"]);
            SceneManager.LoadSceneAsync("City");
        }
    }

    public IEnumerator GetBids(string nftAddress)
    {
        var getBids = new GetBids();
        getBids.nftAddress = nftAddress;

        string json = JsonUtility.ToJson(getBids);

        var req = new UnityWebRequest(getBidsURI, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error While Sending: " + req.error);
            Debug.Log("Received: " + req.downloadHandler.text);
        }
        else
        {
            string returnString = req.downloadHandler.text;
            OnResponse(returnString);

            returnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(returnString);

            foreach (string key in returnDictionary.Keys)
            {
                Debug.Log(key + " => " + returnDictionary[key]);
            }

            AuctionScreen.Instance.AddLastBidToScreen(returnDictionary["bidder"], returnDictionary["amount"]);
        }
    }

    public IEnumerator GiveBid(string amount, string nftAddress)
    {
        var giveBid = new GiveBid();
        giveBid.amount = amount;
        giveBid.nftAddress = nftAddress;

        string json = JsonUtility.ToJson(giveBid);

        var req = new UnityWebRequest(giveBidURI, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Authorization", "Bearer " + /*PlayerPrefs.GetString("token")*/ playerToken);

        //Send the request then wait here until it returns
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error While Sending: " + req.error);
        }
        else
        {
            string returnString = req.downloadHandler.text;
            OnResponse(returnString);

            returnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(returnString);

            foreach (string key in returnDictionary.Keys)
            {
                Debug.Log(key + " => " + returnDictionary[key]);
            }

            yield return new WaitForSeconds(2);
            StartCoroutine(GetBids("TXt7Z1YgPCTujEJ6zMXN6Ywnhga8rUAkax"));
        }
    }
}