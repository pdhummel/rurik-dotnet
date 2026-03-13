using System;
using System.Collections.Generic;

namespace rurik
{
    /// <summary>
    /// Represents a single cell in the Advisor Board (auction space).
    /// Contains the advisor, color, and bid coins.
    /// </summary>
    public class AuctionSpace
    {
        public string actionName { get; set; } = "";
        public int quantity { get; set; }
        public int row { get; set; }
        public int extraCoin { get; set; }
        public string? color { get; set; }
        public int advisor { get; set; }
        public int bidCoins { get; set; }

        public AuctionSpace(string actionName, int quantity, int row, int extraCoin = 0)
        {
            this.actionName = actionName;
            this.quantity = quantity;
            this.row = row;
            this.extraCoin = extraCoin;
            this.color = null;
            this.advisor = 0;
            this.bidCoins = 0;
        }

        public void CopyFrom(AuctionSpace source)
        {
            this.color = source.color;
            this.advisor = source.advisor;
            this.bidCoins = source.bidCoins;
        }

        public AuctionSpace Clone()
        {
            return new AuctionSpace(actionName, quantity, row, extraCoin)
            {
                color = color,
                advisor = advisor,
                bidCoins = bidCoins
            };
        }
    }
}
