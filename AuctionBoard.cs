using System;
using System.Collections.Generic;
using System.Linq;

namespace rurik
{
    /// <summary>
    /// Represents the Advisor Board (auction board) for the game.
    /// Has 6 columns: muster, move, attack, tax, build, scheme.
    /// Each column has 3-4 rows depending on player count.
    /// </summary>
    public class AuctionBoard
    {
        public Dictionary<string, List<AuctionSpace>> Board {get; set;} 
        public int numberOfRows { get; private set; }

        public AuctionBoard(int numberOfPlayers)
        {
            NumberOfPlayers = numberOfPlayers;
            Board = new Dictionary<string, List<AuctionSpace>>
            {
                { "muster", new List<AuctionSpace>() },
                { "move", new List<AuctionSpace>() },
                { "attack", new List<AuctionSpace>() },
                { "tax", new List<AuctionSpace>() },
                { "build", new List<AuctionSpace>() },
                { "scheme", new List<AuctionSpace>() }
            };

            Reset();
        }

        public int NumberOfPlayers { get; private set; }



        public List<AuctionSpace> GetColumn(string actionName)
        {
            return Board[actionName];
            //return Board.TryGetValue(actionName, out var column) ? column : new List<AuctionSpace>();
        }

        public bool IsColumnFull(string actionName)
        {
            var column = GetColumn(actionName);
            var count = column.Count(space => space.advisor > 0);
            return count >= numberOfRows;
        }

        public bool IsPlayerAlreadyInColumn(string actionName, string color)
        {
            var column = GetColumn(actionName);
            return column.Any(space => space.color == color);
        }

        public bool IsPlayerIn3Columns(string color)
        {
            var count = 0;
            foreach (var column in Board.Values)
            {
                if (column.Any(space => space.color == color))
                {
                    count++;
                }
            }
            return count >= 3;
        }

        public List<AuctionSpace> GetNextAuctionSpaceAdvisor(string color)
        {
            var currentValue = 6;
            var advisors = new List<AuctionSpace>();
            
            foreach (var column in Board.Values)
            {
                foreach (var space in column)
                {
                    if (space.color == color)
                    {
                        if (space.advisor == currentValue)
                        {
                            space.row = column.IndexOf(space);
                            advisors.Add(space);
                        }
                        else if (space.advisor < currentValue)
                        {
                            advisors.Clear();
                            space.row = column.IndexOf(space);
                            advisors.Add(space);
                            currentValue = space.advisor;
                        }
                    }
                }
            }
            return advisors;
        }

        public void AuctionBid(string actionName, string color, int advisor, int bidCoins = 0)
        {
            Globals.Log("AuctionBid(): enter: actionName=" + actionName + ", advisor="+ advisor);
            if (IsColumnFull(actionName))
            {
                throw new InvalidOperationException($"Cannot place advisor in {actionName} column, because it is full.");
            }

            if (IsPlayerAlreadyInColumn(actionName, color))
            {
                if (!IsPlayerIn3Columns(color))
                {
                    throw new InvalidOperationException($"Cannot place advisor in {actionName} column, because you can only place a second advisor in the same column, if you are in three or more different columns.");
                }
            }

            var totalBid = advisor + bidCoins;
            var column = GetColumn(actionName);

            for (int i = 0; i < numberOfRows; i++)
            {
                var currentBid = column[i].advisor + column[i].bidCoins;
                if (totalBid > currentBid)
                {
                    if (currentBid > 0)
                    {
                        // Move everything down
                        Globals.Log("AuctionBid(): bump down");
                        for (int j = numberOfRows - 1; j > i; j--)
                        {
                            var aboveSpace = column[j - 1];
                            column[j].CopyFrom(aboveSpace);
                        }
                    }
                    Globals.Log("AuctionBid(): set advisor for " + column[i].actionName + " at row " + i);
                    column[i].color = color;
                    column[i].advisor = advisor;
                    column[i].bidCoins = bidCoins;
                    break;
                }
            }
        }

        public void Reset()
        {
            Board["muster"].Clear();
            Board["move"].Clear();
            Board["attack"].Clear();
            Board["tax"].Clear();
            Board["build"].Clear();
            Board["scheme"].Clear();

            if (NumberOfPlayers <= 2)
            {
                // 3 rows per column for 1-2 players
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
                // 4 rows per column for 3-4 players
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

            numberOfRows = NumberOfPlayers <= 2 ? 3 : 4;
        }

        private void AddAuctionSpace(string action, int quantity, int extraCoin)
        {
            var row = Board[action].Count;
            Board[action].Add(new AuctionSpace(action, quantity, row, extraCoin));
        }
    }
}
