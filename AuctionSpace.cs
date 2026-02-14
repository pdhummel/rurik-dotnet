using System;
using System.Collections.Generic;

namespace rurik
{
    public class AuctionSpace
    {
        public string actionName { get; set; }
        public int quantity { get; set; }
        public int extraCoin { get; set; }
        public string color { get; set; }
        public int advisor { get; set; }
        public int bidCoins { get; set; }
        public int row { get; set; }

        public AuctionSpace(string actionName, int quantity, int row, int extraCoin = 0)
        {
            this.actionName = actionName;
            this.quantity = quantity;
            this.extraCoin = extraCoin;
            this.color = null;
            this.advisor = 0;
            this.bidCoins = 0;
            this.row = row;
        }

        public void AuctionBid(string color, int advisor, int bidCoins)
        {
            this.color = color;
            this.advisor = advisor;
            this.bidCoins = bidCoins;
        }

        public void CopyBid(AuctionSpace auctionSpace)
        {
            this.color = auctionSpace.color;
            this.advisor = auctionSpace.advisor;
            this.bidCoins = auctionSpace.bidCoins;
        }
        
        public AuctionSpace Clone()
        {
            AuctionSpace auctionSpace = new AuctionSpace(this.actionName, this.quantity, this.extraCoin);
            return auctionSpace;
        }
    }
}
