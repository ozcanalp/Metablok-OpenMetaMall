using itSeez3D.Newtonsoft.Json;
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
        checkPlugConnectionBtn.onClick.AddListener(CheckConnection);
        connectPlugWalletBtn.onClick.AddListener(RequestConnection);
        getNftsBtn.onClick.AddListener(GetNfts);
        payBtn.onClick.AddListener(Pay);
    }

    void OnDestroy()
    {
        checkPlugConnectionBtn.onClick.RemoveListener(CheckConnection);
        connectPlugWalletBtn.onClick.RemoveListener(RequestConnection);
        getNftsBtn.onClick.RemoveListener(GetNfts);
    }

    void CheckConnection()
    {
        label.text = "Loading...";
        ReactApi.Instance.CheckPlugConnection(OnCheckConnection);
    }

    void OnCheckConnection(string jsonData)
    {
        Debug.Log("OnCheckConnection:" + jsonData);

        var response = JsonConvert.DeserializeObject<CheckPlugConnectionResponse>(jsonData);
        if (response == null)
        {
            Debug.LogError("Unable to parse CheckPlugConnectionResponse -- make sure you are running the project as a WebGL build in browser");
            //SceneManager.LoadScene(sceneToLoad);
            return;
        }

        label.text = "Checked Plug Connection with response of: " + (response.result);

        //SceneManager.LoadScene(sceneToLoad);
    }

    void RequestConnection()
    {
        label.text = "Loading...";
        ReactApi.Instance.RequestPlugConnect(OnRequestConnection);
    }

    void OnRequestConnection(string jsonData)
    {
        Debug.Log("OnRequestConnection:" + jsonData);

        var response = JsonConvert.DeserializeObject<RequestPlugConnectResponse>(jsonData);
        if (response == null)
        {
            Debug.LogError("Unable to parse RequestPlugConnectResponse -- make sure you are running the project as a WebGL build in browser");
            return;
        }

        label.text = "Requested Plug Connection with response of: " + response.result;
    }

    void GetNfts()
    {
        label.text = "Loading...";
        ReactApi.Instance.GetPlugNfts(OnGetNfts);
    }

    void OnGetNfts(string jsonData)
    {
        Debug.Log("OnGetNfts:" + jsonData);

        var response = JsonConvert.DeserializeObject<GetDabNftsResponse>(jsonData);
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

        string account = accountInputField.text;
        float amount = float.Parse(amountInputField.text);

        Debug.Log("Account:" + account + " amount:" + amount);

        ReactApi.Instance.Pay(OnPay, account, amount);
    }

    void OnPay(string jsonData)
    {
        Debug.Log("OnPay:" + jsonData);

        var response = JsonConvert.DeserializeObject<PayResponse>(jsonData);
        if (response == null)
        {
            Debug.LogError("Unable to parse PayResponse -- make sure you are running the project as a WebGL build in browser");
            return;
        }

        label.text = response.data;
    }
}
