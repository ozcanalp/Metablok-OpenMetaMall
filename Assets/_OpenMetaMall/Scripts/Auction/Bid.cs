using System;

public class Bid
{
    string walletId;
    string bidAmount;
    string bidTime;

    public string WalletId { get { return walletId; } }
    public string BidAmount { get { return bidAmount; } }
    public string BidTime { get { return bidTime; } }

    public Bid(string walletId, string bidAmount, string bidTime)
    {
        this.walletId = walletId;
        this.bidAmount = bidAmount;
        this.bidTime = bidTime;
    }
}
