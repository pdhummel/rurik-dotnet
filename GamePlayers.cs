using System;
using System.Collections.Generic;
using System.Linq;

namespace rurik
{
    public class GamePlayers
    {
        public int targetNumberOfPlayers { get; set; }
        public List<Player> players { get; set; }
        public List<Player> sortedPlayers { get; set; }
        public Dictionary<string, Player> playersByColor { get; set; }
        public Dictionary<string, Player> playersByName { get; set; }
        public Dictionary<string, Player> playersByPosition { get; set; }
        public long lastActionTimeStamp { get; set; }
        public Player firstPlayer { get; set; }
        public Player nextFirstPlayer { get; set; }
        public Player currentPlayer { get; set; }

        public GamePlayers(int targetNumberOfPlayers)
        {
            this.targetNumberOfPlayers = targetNumberOfPlayers;
            this.players = new List<Player>();
            this.sortedPlayers = new List<Player>();
            this.playersByColor = new Dictionary<string, Player>();
            this.playersByName = new Dictionary<string, Player>();
            this.playersByPosition = new Dictionary<string, Player>();
            this.lastActionTimeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public Player addPlayer(string name, string color, string position, bool isPlayerAi, Cards cards)
        {
            if (this.players.Count >= this.targetNumberOfPlayers)
            {
                throw new Exception("Target player count has already been reached.");
            }
            if (this.playersByColor.ContainsKey(color))
            {
                throw new Exception("Color " + color + " has already been selected.");
            }
            if (this.playersByPosition.ContainsKey(position))
            {
                throw new Exception("Position " + position + " has already been selected.");
            }
            if (this.playersByName.ContainsKey(name))
            {
                throw new Exception("Player " + name + " has already joined the game.");
            }
            var player = new Player(name, color, position, isPlayerAi);
            this.players.Add(player);
            this.playersByColor[color] = player;
            this.playersByName[name] = player;
            this.playersByPosition[position] = player;
            // Note the player much choose between these 2 cards 
            player.addSecretAgenda(cards.dealRandomSecretAgendaCard());
            player.addSecretAgenda(cards.dealRandomSecretAgendaCard());
            return player;
        }

        public void setTroopsToDeploy(int count)
        {
            foreach (var player in this.players)
            {
                player.troopsToDeploy = count;
            }
        }

        public void setAdvisors(List<string> advisors)
        {
            foreach (var player in this.players)
            {
                player.setAdvisors(advisors.ToList());
                player.advisorCountForTurn = advisors.Count;
                player.advisorsToAuctionSpace = new Dictionary<int, List<AuctionSpace>>();
                player.advisorsToAuctionSpace[1] = new List<AuctionSpace>();
                player.advisorsToAuctionSpace[2] = new List<AuctionSpace>();
                player.advisorsToAuctionSpace[3] = new List<AuctionSpace>();
                player.advisorsToAuctionSpace[4] = new List<AuctionSpace>();
                player.advisorsToAuctionSpace[5] = new List<AuctionSpace>();
            }
        }

        public void endRoundForPlayers(GameLogic game, ClaimBoard claimBoard)
        {
            foreach (var player in this.players)
            {
                player.boat.canPlayMusterConversionTile = true;
                player.boat.canPlayAttackConversionTile = true;
                player.boat.canPlayBuildConversionTile = true;
                var coinIncome = player.boat.CalculateCoinIncome();
                player.boat.money = player.boat.money + coinIncome;
                // TODO: game.log.info(player.Color + " received " + coinIncome + " money from boat resources.");
                var coinCompensation = claimBoard.CalculateCoins(player.Color);
                // TODO: game.log.info(player.color + " received " + coinCompensation + " money from claim tracks.");
                player.boat.money = player.boat.money + coinCompensation;
                player.finishedRound = false;
                player.aiCard = null;
                player.aiStrategy = null;
                player.usedGlebAttack = false;
                player.usedTheofanaTax = false;
                player.usedTheofanaTax = false;
                player.usedPredslavaMove = false;
            }
        }

        public void setFirstPlayer(Player player)
        {
            if (this.firstPlayer != null)
            {
                this.firstPlayer.isFirstPlayer = false;
            }
            this.firstPlayer = player;
            player.isFirstPlayer = true;
        }

        public void setFirstPlayerByColor(string color)
        {
            if (this.firstPlayer != null)
            {
                this.firstPlayer.isFirstPlayer = false;
            }
            this.firstPlayer = this.playersByColor[color];
            this.firstPlayer.isFirstPlayer = true;
        }

        public void setNextFirstPlayerByColor(string color)
        {
            if (this.nextFirstPlayer != null)
            {
                this.nextFirstPlayer.isNextFirstPlayer = false;
            }
            this.nextFirstPlayer = this.playersByColor[color];
            this.nextFirstPlayer.isNextFirstPlayer = true;
        }

        public void setCurrentPlayer(Player player)
        {
            this.currentPlayer = player;
        }

        public void setCurrentPlayerByColor(string color)
        {
            this.currentPlayer = this.playersByColor[color];
        }

        public Player getFirstPlayer()
        {
            return this.firstPlayer;
        }

        public Player GetCurrentPlayer()
        {
            return this.currentPlayer;
        }

        public Player getPlayerByColor(string color)
        {
            return this.playersByColor[color];
        }

        public Player getPlayerByName(string name)
        {
            return this.playersByName[name];
        }

        public Player getPlayerByPosition(string position)
        {
            return this.playersByPosition[position];
        }

        public Player getNextPlayer(Player player)
        {
            if (player == null)
            {
                return null;
            }
            for (int i = 0; i < this.sortedPlayers.Count; i++)
            {
                if (this.sortedPlayers[i].Color == player.Color)
                {
                    int nextIndex = i + 1;
                    if (nextIndex >= this.sortedPlayers.Count)
                    {
                        nextIndex = 0;
                    }
                    return this.sortedPlayers[nextIndex];
                }
            }
            return null;
        }

        public void advanceToNextPlayer()
        {
            for (int i = 0; i < this.sortedPlayers.Count; i++)
            {
                if (this.sortedPlayers[i].Color == this.currentPlayer.Color)
                {
                    int nextIndex = i + 1;
                    if (i >= this.sortedPlayers.Count)
                    {
                        nextIndex = 0;
                    }
                    this.currentPlayer = this.sortedPlayers[nextIndex];
                    break;
                }
            }
        }

        public int getNumberOfPlayers()
        {
            return this.players.Count;
        }

        public void sortPlayers()
        {
            if (this.playersByPosition.ContainsKey("N"))
            {
                this.sortedPlayers.Add(this.playersByPosition["N"]);
            }
            if (this.playersByPosition.ContainsKey("E"))
            {
                this.sortedPlayers.Add(this.playersByPosition["E"]);
            }
            if (this.playersByPosition.ContainsKey("S"))
            {
                this.sortedPlayers.Add(this.playersByPosition["S"]);
            }
            if (this.playersByPosition.ContainsKey("W"))
            {
                this.sortedPlayers.Add(this.playersByPosition["W"]);
            }
        }

        public Player getRandomPlayer()
        {
            var i = new Random().Next(this.getNumberOfPlayers());
            return this.players[i];
        }

        public void mapAdvisorsToAuctionSpacesByAction(AuctionBoard auctionBoard, string action)
        {
            for (int i = 0; i < auctionBoard.numberOfRows; i++)
            {
                var auctionSpace = auctionBoard.board[action][i];
                var color = auctionSpace.color;
                if (color != null)
                {
                    var player = this.getPlayerByColor(color);
                    var advisor = auctionSpace.advisor;
                    player.advisorsToAuctionSpace[advisor].Add(auctionSpace);
                }
            }
        }

        public void mapAdvisorsToAuctionSpaces(AuctionBoard auctionBoard)
        {
            this.mapAdvisorsToAuctionSpacesByAction(auctionBoard, "muster");
            this.mapAdvisorsToAuctionSpacesByAction(auctionBoard, "move");
            this.mapAdvisorsToAuctionSpacesByAction(auctionBoard, "attack");
            this.mapAdvisorsToAuctionSpacesByAction(auctionBoard, "build");
            this.mapAdvisorsToAuctionSpacesByAction(auctionBoard, "tax");
            this.mapAdvisorsToAuctionSpacesByAction(auctionBoard, "scheme");
        }
    }
}
