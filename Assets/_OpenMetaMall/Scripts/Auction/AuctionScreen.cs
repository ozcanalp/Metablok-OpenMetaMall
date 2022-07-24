using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Photon.Pun;
using System.Collections;

public class AuctionScreen : MonoBehaviourPunCallbacks
{
    public AuctionScreen Instance;

    public event Action OnAuctionCountDownEnd = delegate { };

    [SerializeField] PhotonView PV;

    List<Bid> lastBids;

    [SerializeField] TextMeshProUGUI lastBidsText;
    int numberOfBidsToSeeInScreen = 5;

    [SerializeField] TextMeshProUGUI countDownText;

    [SerializeField] TMP_InputField walletInput;
    [SerializeField] TMP_InputField amountInput;

    [SerializeField] const int TimerInSecond = 5;
    int timer;

    Coroutine countDownRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Start()
    {
        PV = GetComponent<PhotonView>();
        lastBids = new List<Bid>();
    }

    public void AddBidWithUIButton()
    {
        //Bid newBid = new Bid(walletInput.text, amountInput.text, DateTime.Now.ToString("HH:mm:ss"));
        //AddNewLastBid(newBid);
        PV.RPC("AddNewLastBid", RpcTarget.AllBuffered, walletInput.text, amountInput.text, DateTime.Now.ToString("HH:mm:ss"));
    }

    [PunRPC]
    void AddNewLastBid(string walletId, string amount, string time)
    {
        Bid newBid = new Bid(PlayerPrefs.GetString("walletId", "defaultWalletId"), amount, time);
        lastBids.Add(newBid);
        UpdateScreen();
        ResetTimer();
    }

    void UpdateScreen()
    {
        if (lastBids.Count <= 0)
            return;

        lastBidsText.text = "";
        for (int i = lastBids.Count - 1; i >= lastBids.Count - numberOfBidsToSeeInScreen; i--)
        {
            if (i < 0)
                break;

            Bid bid = lastBids[i];
            lastBidsText.text += $"{lastBids.Count - i}) {bid.WalletId} - {bid.BidAmount} - {bid.BidTime}\n";
        }
    }

    void ResetTimer()
    {
        timer = TimerInSecond;

        if (countDownRoutine != null)
            StopCoroutine(countDownRoutine);

        countDownRoutine = StartCoroutine(StartCountDown());
    }

    IEnumerator StartCountDown()
    {

        while (timer > 0)
        {
            if (PV.IsMine)
                PV.RPC("DecreaseTimer", RpcTarget.AllBuffered);
            UpdateCountDownUI();
            yield return new WaitForSeconds(1f);
        }

        OnAuctionCountDownEnd();
    }

    [PunRPC]
    void DecreaseTimer()
    {
        timer--;
    }

    void UpdateCountDownUI()
    {
        countDownText.SetText(timer.ToString());
    }
}
