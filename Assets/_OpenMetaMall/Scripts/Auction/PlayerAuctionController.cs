using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAuctionController : MonoBehaviour
{
    [SerializeField] GameObject EnterAuctionCanvas;

    private void OnEnable()
    {
        AuctionScreen.Instance.OnAuctionCountDownEnd += TronAPI.Instance.AcceptBidHandler;
    }

    private void OnDisable()
    {
        AuctionScreen.Instance.OnAuctionCountDownEnd -= TronAPI.Instance.AcceptBidHandler;
    }

    public void ShowEnterAuctionCanvas()
    {
        EnterAuctionCanvas.SetActive(true);
    }
}