using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAuctionController : MonoBehaviour
{
    [SerializeField] GameObject EnterAuctionCanvas;

    [SerializeField] AuctionScreen auctionScreen;
    [SerializeField] Button giveBidButton;

    private void Awake()
    {
    }

    public void ShowEnterAuctionCanvas()
    {
        EnterAuctionCanvas.SetActive(true);
    }

    public void EnterAuction()
    {
        Debug.Log("Entered Auction");
    }


}