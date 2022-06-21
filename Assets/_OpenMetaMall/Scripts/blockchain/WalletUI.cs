using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WalletUI : MonoBehaviour
{
    public TMP_Text label;

    public Button checkPlugConnectionBtn;
    public Button connectPlugWalletBtn;
    public Button getNftsBtn;

    public TMP_InputField accountInputField;
    public TMP_InputField amountInputField;
    public Button payBtn;

    [SerializeField] string sceneToLoad = "Environment";

    void Start()
    {
        if (checkPlugConnectionBtn != null)
        {
            checkPlugConnectionBtn.onClick.AddListener(CheckConnection);
        }

        if (connectPlugWalletBtn != null)
        {
            connectPlugWalletBtn.onClick.AddListener(RequestConnection);
        }

        if (getNftsBtn != null)
        {
            getNftsBtn.onClick.AddListener(GetNfts);
        }

        if (payBtn != null)
        {
            payBtn.onClick.AddListener(Pay);
        }
    }

    void OnDestroy()
    {
        if (checkPlugConnectionBtn != null)
        {
            checkPlugConnectionBtn.onClick.RemoveListener(CheckConnection);
        }
        if (connectPlugWalletBtn != null)
        {
            connectPlugWalletBtn.onClick.RemoveListener(RequestConnection);
        }

        if (getNftsBtn != null)
        {
            getNftsBtn.onClick.RemoveListener(GetNfts);
        }

        if (payBtn != null)
        {
            payBtn.onClick.RemoveListener(Pay);
        }
    }

    void CheckConnection()
    {
        label.text = "Loading...";
        ReactApi.Instance.CheckPlugConnection(OnCheckConnection);
    }

    void OnCheckConnection(string jsonData)
    {
        Debug.Log("WalletUI.OnCheckConnection:" + jsonData);

        var response = JsonUtility.FromJson<CheckPlugConnectionResponse>(jsonData);
        if (response == null)
        {
            Debug.LogError("Unable to parse CheckPlugConnectionResponse -- make sure you are running the project as a WebGL build in browser");
#if UNITY_EDITOR
            SceneManager.LoadScene(sceneToLoad);
#endif
            return;
        }

        label.text = "Checked Plug Connection with response of: " + (response.result);

        SceneManager.LoadScene(sceneToLoad);
    }

    void RequestConnection()
    {
        label.text = "Loading...";
        //SceneManager.LoadScene(sceneToLoad);
        ReactApi.Instance.RequestPlugConnect(OnRequestConnection);
    }

    void OnRequestConnection(string jsonData)
    {
        Debug.Log("WalletUI.OnRequestConnection:" + jsonData);

        var response = JsonUtility.FromJson<RequestPlugConnectResponse>(jsonData);
        if (response == null)
        {
            Debug.LogError("Unable to parse RequestPlugConnectResponse -- make sure you are running the project as a WebGL build in browser");
#if UNITY_EDITOR
            SceneManager.LoadScene(sceneToLoad);
#endif
            return;
        }

        if (response.result == "allowed" || response.result == "true")
        {
            label.text = "Requested Plug Connection with response of: " + response.result;
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            label.text = response.result + " " + response.error;
        }
    }

    void GetNfts()
    {
        label.text = "Loading...";
        ReactApi.Instance.GetPlugNfts(OnGetNfts);
    }

    void OnGetNfts(string jsonData)
    {
        Debug.Log("OnGetNfts:" + jsonData);

        var response = JsonUtility.FromJson<GetDabNftsResponse>(jsonData);
        if (response == null)
        {
            Debug.LogError("Unable to parse GetDabNftsResponse -- make sure you are running the project as a WebGL build in browser");
            return;
        }

        label.text = "Fetched Plug NFTs with response of:\n";

        foreach (var collection in response.collections)
        {
            foreach (var token in collection.tokens)
            {
                label.text += token.collection + " #" + token.index + "\n";
            }
        }
    }

    void Pay()
    {
        label.text = "Loading...";

        // Receiver account id
        string account = "8fa4e53ed364960a8350e545d671fe5b704e717c77af217e4f132ab948636b5d";
        double amount = 0.1f * 1E8; //float.Parse(amountInputField.text);

        Debug.Log("Account:" + account + " amount:" + amount);

        ReactApi.Instance.Pay(OnPay, account, amount);
    }

    void OnPay(string jsonData)
    {
        Debug.Log("OnPay:" + jsonData);

        var response = JsonUtility.FromJson<PayResponse>(jsonData);
        if (response == null)
        {
            Debug.LogError("Unable to parse PayResponse -- make sure you are running the project as a WebGL build in browser");
            return;
        }

        label.text = response.result + " " + response.error;
    }
}
