using System;
using System.Collections.Generic;

namespace rurik
{
    public class AuctionBoard
    {
        public int numberOfPlayers { get; set; }
        public Dictionary<string, List<AuctionSpace>> board { get; set; }
        public int numberOfRows { get; set; }

        public AuctionBoard(int numberOfPlayers)
        {
            this.numberOfPlayers = numberOfPlayers;
            this.board = new Dictionary<string, List<AuctionSpace>>();
            Reset();
            this.numberOfRows = this.board["muster"].Count;
        }

        public bool IsPlayerAlreadyInColumn(string actionName, string color)
        {
            for (int i = 0; i < numberOfRows; i++)
            {
                if (this.board[actionName][i].color == color)
                {
                    return true;
                }
            }
            return false;        
        }

        public bool IsPlayerIn3Columns(string color)
        {
            int count = 0;
            List<string> columns = new List<string>(this.board.Keys);
            for (int k = 0; k < columns.Count; k++)
            {
                string actionName = columns[k];
                for (int i = 0; i < numberOfRows; i++)
                {
                    if (this.board[actionName][i].color == color)
                    {
                        count = count + 1;
                        break;
                    }
                }
            }
            if (count >= 3)
            {
                return true;
            }
            return false;
        }

        public List<AuctionSpace> GetNextAuctionSpaceAdvisor(string color)
        {
            int currentValue = 6;
            List<AuctionSpace> advisors = new List<AuctionSpace>();
            List<string> columns = new List<string>(this.board.Keys);
            for (int k = 0; k < columns.Count; k++)
            {
                string actionName = columns[k];
                for (int i = 0; i < numberOfRows; i++)
                {
                    if (this.board[actionName][i].color == color)
                    {
                        if (this.board[actionName][i].advisor == currentValue)
                        {
                            this.board[actionName][i].row = i;
                            advisors.Add(this.board[actionName][i]);
                        }
                        else if (this.board[actionName][i].advisor < currentValue)
                        {
                            advisors.Clear();
                            this.board[actionName][i].row = i;
                            advisors.Add(this.board[actionName][i]);
                            currentValue = this.board[actionName][i].advisor;
                        }    
                    }
                }
            }
            return advisors;
        }

        // row=0-3
        public void AuctionBid(string actionName, string color, int advisor, int bidCoins = 0)
        {
            Globals.Log("auctionBid(): " + color + " " + actionName + " " + advisor);
            if (IsColumnFull(actionName))
            {
                throw new Exception("Cannot place advisor in " + actionName + " column, because it is full.");
            }
            // check for 2 advisors from the same player
            if (IsPlayerAlreadyInColumn(actionName, color))
            {
                if (!IsPlayerIn3Columns(color))
                {
                    throw new Exception("Cannot place advisor in " + actionName + " column, because you can only place a second advisor in the same column, if you are in three or more different columns.");
                }
            }

            int totalBid = advisor + bidCoins;
            Globals.Log("auctionBid(): totalBid=" + totalBid);
            for (int i = 0; i < numberOfRows; i++)
            {
                int currentBid = this.board[actionName][i].advisor + this.board[actionName][i].bidCoins;
                Globals.Log("auctionBid(): currentBid=" + currentBid + ", row " + i);
                if (totalBid > currentBid)
                {
                    // "move" everything down
                    for (int j = numberOfRows - 1; j > i; j--)
                    {
                        AuctionSpace aboveAuctionSpace = this.board[actionName][j - 1];
                        Globals.Log("auctionBid(): aboveAuctionSpace " + (j - 1) + "=" + aboveAuctionSpace.ToString());
                        this.board[actionName][j].CopyBid(aboveAuctionSpace);
                    }
                    Globals.Log("auctionBid(): auctionBid " + i + ":" + color + " " + advisor + " " + bidCoins);
                    this.board[actionName][i].AuctionBid(color, advisor, bidCoins);
                    break;
                }
            }
        }

        public bool IsColumnFull(string actionName)
        {
            bool isFull = false;
            int count = 0;
            for (int i = 0; i < numberOfRows; i++)
            {
                if (this.board[actionName][i].advisor > 0)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            if (count >= numberOfRows)
            {
                isFull = true;
            }
            return isFull;
        }

        public void Reset()
        {
            this.board["muster"] = new List<AuctionSpace>();
            this.board["move"] = new List<AuctionSpace>();
            this.board["attack"] = new List<AuctionSpace>();
            this.board["tax"] = new List<AuctionSpace>();
            this.board["build"] = new List<AuctionSpace>();
            this.board["scheme"] = new List<AuctionSpace>();
            
            if (this.numberOfPlayers <= 2)
            {
                AddAuctionSpace("muster", 3, 0);
                AddAuctionSpace("muster", 2, 0);
                AddAuctionSpace("muster", 1, 1);
                AddAuctionSpace("move", 3, 0);
                AddAuctionSpace("move", 2, 0);
                AddAuctionSpace("move", 1, 0);
                AddAuctionSpace("attack", 2, 0);
                AddAuctionSpace("attack", 1, 0);
                AddAuctionSpace("attack", 1, 1);
                AddAuctionSpace("tax", 2, 0);
                AddAuctionSpace("tax", 1, 0);
                AddAuctionSpace("tax", 1, 1);
                AddAuctionSpace("build", 2, 0);
                AddAuctionSpace("build", 1, 0);
                AddAuctionSpace("build", 1, 1);
                AddAuctionSpace("scheme", 3, 0);
                AddAuctionSpace("scheme", 2, 0);
                AddAuctionSpace("scheme", 1, 0);
            }
            else
            {
                AddAuctionSpace("muster", 3, 0);
                AddAuctionSpace("muster", 2, 0);
                AddAuctionSpace("muster", 1, 0);
                AddAuctionSpace("muster", 1, 1);
                AddAuctionSpace("move", 4, 0);
                AddAuctionSpace("move", 3, 0);
                AddAuctionSpace("move", 2, 0);
                AddAuctionSpace("move", 1, 0);
                AddAuctionSpace("attack", 2, 0);
                AddAuctionSpace("attack", 1, 0);
                AddAuctionSpace("attack", 1, 1);
                AddAuctionSpace("attack", 1, 2);
                AddAuctionSpace("tax", 3, 0);
                AddAuctionSpace("tax", 2, 0);
                AddAuctionSpace("tax", 1, 0);
                AddAuctionSpace("tax", 1, 1);
                AddAuctionSpace("build", 2, 0);
                AddAuctionSpace("build", 1, 0);
                AddAuctionSpace("build", 1, 1);
                AddAuctionSpace("build", 1, 2);
                AddAuctionSpace("scheme", 3, 0);
                AddAuctionSpace("scheme", 2, 0);
                AddAuctionSpace("scheme", 1, 0);
                AddAuctionSpace("scheme", 1, 1);
            }
        }

        public void AddAuctionSpace(string action, int quantity, int extraCoin)
        {
            int row = this.board[action].Count;
            this.board[action].Add(new AuctionSpace(action, quantity, row, extraCoin));
        }
    }
}
