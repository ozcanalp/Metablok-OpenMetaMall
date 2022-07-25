using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAuctionController : MonoBehaviour
{
    [SerializeField] GameObject EnterAuctionCanvas;

    public void ShowEnterAuctionCanvas()
    {
        EnterAuctionCanvas.SetActive(true);
    }
}