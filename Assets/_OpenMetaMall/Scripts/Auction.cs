using UnityEngine;
using TMPro;

public class Auction : MonoBehaviour
{
    [SerializeField] float currentBidPrice;
    [SerializeField] TextMeshProUGUI currentBidPriceText;
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] TMP_InputField bidInput;

    private void Start()
    {
        currentBidPrice = 0;
        currentBidPriceText.text = currentBidPrice.ToString();
    }

    public void SubmitBid()
    {
        float inputBid;
        Debug.Log(bidInput.text);
        if (float.TryParse(bidInput.text, out inputBid))
        {
            if (inputBid > currentBidPrice)
            {
                currentBidPrice = inputBid;
                currentBidPriceText.text = currentBidPrice.ToString();
                infoText.text = "New bid confirmed";
            }
            else if (inputBid <= currentBidPrice)
            {
                infoText.text = "New bid cannot be lower than current bid";
            }
        }
    }
}
