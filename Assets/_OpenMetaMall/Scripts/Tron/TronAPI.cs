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
    public string tokenId;
}

class GiveBid
{
    public string amount;
    public string nftAddress;
    public string tokenId;
}

class AcceptBidData
{
    public string nftAddress;
    public string tokenId;
}

class Transfer
{
    public string amount;
}

public class TronAPI : MonoBehaviour
{
    public static TronAPI Instance;

    const string signupURI = "https://metabid-server.herokuapp.com/api/v1/users/signup";
    const string loginURI = "https://metabid-server.herokuapp.com/api/v1/users/login";
    const string getBidsURI = "https://metabid-server.herokuapp.com/api/v1/users/get-bids";
    const string giveBidURI = "https://metabid-server.herokuapp.com/api/v1/users/give-bid";
    const string acceptBidURI = "https://metabid-server.herokuapp.com/api/v1/users/accept-bid";
    const string transferURI = "https://metabid-server.herokuapp.com/api/v1/users/transfer";

    const string testURI = "https://reqres.in/api/login";

    public Dictionary<string, string> returnDictionary;

    public event Action<bool> OnResponse = delegate { };

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

    public void AcceptBidHandler()
    {
        Debug.Log("Accept Bid Handler");
        StartCoroutine(AcceptBid());
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
            OnResponse(false);
        }
        else
        {
            string returnString = req.downloadHandler.text;
            OnResponse(true);

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
            OnResponse(false);
        }
        else
        {
            string returnString = req.downloadHandler.text;
            OnResponse(true);

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
            OnResponse(false);
        }
        else
        {
            string returnString = req.downloadHandler.text;
            OnResponse(true);

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
        getBids.tokenId = "106";

        string json = JsonUtility.ToJson(getBids);

        var req = new UnityWebRequest(getBidsURI, "POST");
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
            Debug.Log("Received: " + req.downloadHandler.text);
        }
        else
        {
            string returnString = req.downloadHandler.text;
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
        giveBid.tokenId = "106";

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

            returnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(returnString);

            foreach (string key in returnDictionary.Keys)
            {
                Debug.Log(key + " => " + returnDictionary[key]);
            }

            yield return new WaitForSeconds(2);
            StartCoroutine(GetBids("TV4nSngdRknVo1hBngEEZYDc5EZDxVamTJ"));
        }
    }

    IEnumerator AcceptBid()
    {
        Debug.Log("Bid Accept Request Sent");
        AuctionScreen.Instance.CelebrateLastBid();
        yield return null;

        var data = new AcceptBidData();
        data.nftAddress = "TV4nSngdRknVo1hBngEEZYDc5EZDxVamTJ";
        data.tokenId = "106";

        string json = JsonUtility.ToJson(data);

        var req = new UnityWebRequest(acceptBidURI, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        string suleymanToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjYyZGRhN2NhMWRiMjgwZDFmNjFlMmU4OSIsImlhdCI6MTY1ODgzMzYxNywiZXhwIjoxNjY2NjA5NjE3fQ.nTdoqiJLC6La5M-7GfCDAEVoUW4qkL0w7sAJHLomZwY";
        string beytullahToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjYyZGQ3YmQwMGFkOTI5ZjRmNDFiZTE1NyIsImlhdCI6MTY1ODgzMzUxOCwiZXhwIjoxNjY2NjA5NTE4fQ.k1hnEusad7cvSuPcARo-T-ePkndwL0ClK8_KnnjcD3I";
        req.SetRequestHeader("Authorization", "Bearer " + beytullahToken);

        //Send the request then wait here until it returns
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error While Sending: " + req.error);
        }
        else
        {
            string returnString = req.downloadHandler.text;
            returnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(returnString);

            foreach (string key in returnDictionary.Keys)
            {
                Debug.Log(key + " => " + returnDictionary[key]);
            }
        }
    }

    public void OnClickTransfer()
    {
        StartCoroutine(TronAPI.Instance.Transfer("100"));
    }

    IEnumerator Transfer(string amount)
    {
        Transfer transfer = new Transfer();
        transfer.amount = amount;

        string json = JsonUtility.ToJson(transfer);

        var req = new UnityWebRequest(transferURI, "POST");
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

            returnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(returnString);

            foreach (string key in returnDictionary.Keys)
            {
                Debug.Log(key + " => " + returnDictionary[key]);
            }
        }
    }
}