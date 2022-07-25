using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Photon.Pun;
using System.Collections;

public class AuctionScreen : MonoBehaviourPunCallbacks
{
    public static AuctionScreen Instance;

    public event Action OnAuctionCountDownEnd = delegate { };
    public event Action OnStandUp = delegate { };

    [SerializeField] PhotonView PV;

    List<Bid> lastBids;

    [SerializeField] TextMeshProUGUI lastBidsText;
    int numberOfBidsToSeeInScreen = 5;

    [SerializeField] TextMeshProUGUI countDownText;

    [SerializeField] TMP_InputField walletInput;
    [SerializeField] TMP_InputField amountInput;

    [SerializeField] const int TimerInSecond = 120;
    int timer;

    [SerializeField] ParticleSystem celebrateParticle;
    [SerializeField] int celebrateParticleDuration = 5;

    Coroutine countDownRoutine;

    public bool canBid = true;

    public override void OnEnable()
    {
        base.OnEnable();
        OnAuctionCountDownEnd += TronAPI.Instance.AcceptBidHandler;
    }

    public override void OnDisable()
    {
        base.OnEnable();
        OnAuctionCountDownEnd -= TronAPI.Instance.AcceptBidHandler;
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    IEnumerator Start()
    {
        PV = GetComponent<PhotonView>();
        lastBids = new List<Bid>();

        yield return new WaitForSeconds(3);

        /* if (lastBids.Count <= 0)
            StartCoroutine(TronAPI.Instance.GetBids("TXt7Z1YgPCTujEJ6zMXN6Ywnhga8rUAkax")); */
    }

    public void AddBidWithUIButton()
    {
        //Bid newBid = new Bid(walletInput.text, amountInput.text, DateTime.Now.ToString("HH:mm:ss"));
        //AddNewLastBid(newBid);

        if (!(lastBids.Count > 0) || int.Parse(lastBids[lastBids.Count - 1].BidAmount) < int.Parse(amountInput.text))
        {
            StartCoroutine(TronAPI.Instance.GiveBid(amountInput.text, "TV4nSngdRknVo1hBngEEZYDc5EZDxVamTJ"));
        }

        OnStandUp();
    }

    public void StandUp()
    {
        OnStandUp();
    }

    public void AddLastBidToScreen(string bidder, string amount)
    {
        PV.RPC("AddNewLastBid", RpcTarget.AllBuffered, bidder, amount, DateTime.Now.ToString("HH:mm:ss"));
    }

    [PunRPC]
    void AddNewLastBid(string walletId, string amount, string time)
    {
        Bid newBid = new Bid(walletId, amount, time);
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
            lastBidsText.text += $"{lastBids.Count - i}) {bid.BidAmount} - {bid.BidTime} - {bid.WalletId}\n";
        }
    }


    public void CelebrateLastBid()
    {
        PV.RPC("CelebrateLastBidRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void CelebrateLastBidRPC()
    {
        Bid bid = lastBids[lastBids.Count - 1];
        lastBidsText.text = $"Winner Bid\n{bid.BidAmount} - {bid.BidTime} - {bid.WalletId}";

        StartCoroutine(StartParticles());
    }

    IEnumerator StartParticles()
    {
        celebrateParticle.Play();

        yield return new WaitForSeconds(celebrateParticleDuration);

        celebrateParticle.Stop();
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

        canBid = false;
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
