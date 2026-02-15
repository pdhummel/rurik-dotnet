using System;
using System.Collections.Generic;

namespace rurik
{
    public class GameStatus
    {
        public string GameId { get; set; }
        public string Id { get; set; }
        public string GameName { get; set; }
        public string Owner { get; set; }
        public string Name { get; set; }
        public string PlayerNames { get; set; }
        public string CurrentPlayer { get; set; }
        public Player ClientPlayer { get; set; }
        public string EndGameSummary { get; set; }
        public int NumberOfPlayers { get; set; }
        public int TargetNumberOfPlayers { get; set; }
        public string CurrentState { get; set; }
        public string FirstPlayer { get; set; }
        public Leader ClientLeader { get; set; }
        public string ClientName { get; set; }
        public int ClientPosition { get; set; }
        public string StatusMessage { get; set; }
        public List<string> AvailableActions { get; set; }
        public string NextPlayer { get; set; }
        public string NextFirstPlayer { get; set; }
        public int Round { get; set; }

        public GameStatus(RurikGame game, string clientColor = null)
        {
            GameId = game.Id;
            Id = game.Id;
            GameName = game.Name;
            Owner = game.Owner;
            Name = game.Name;
            PlayerNames = "";
            CurrentPlayer = null;
            ClientPlayer = null;
            EndGameSummary = "";
            NumberOfPlayers = game.Players.getNumberOfPlayers();
            TargetNumberOfPlayers = game.TargetNumberOfPlayers;
            var currentGameState = game.GameStates.GetCurrentState();
            CurrentState = null;
            if (currentGameState != null)
            {
                CurrentState = currentGameState.Name;
            }
            var firstPlayer = game.Players.getFirstPlayer();
            if (firstPlayer != null)
            {
                FirstPlayer = firstPlayer.Color;
            }
            if (clientColor != null && clientColor.Length > 0)
            {
                ClientPlayer = game.GetPlayer(clientColor);
                if (ClientPlayer != null)
                {
                    ClientLeader = ClientPlayer.leader;
                    ClientName = ClientPlayer.name;
                    // TODO: ClientPosition = ClientPlayer.Position;
                }
            }
            if (CurrentState != "waitingForPlayers" && currentGameState != null)
            {
                Round = game.CurrentRound;
                var currentPlayer = game.Players.currentPlayer;
                if (currentPlayer != null)
                {
                    CurrentPlayer = currentPlayer.Color;
                    if (currentPlayer.Color == clientColor)
                    {
                        StatusMessage = "Waiting on you";
                        AvailableActions = currentGameState.AllowedActions;
                    }
                    else if (clientColor == null || clientColor.Length < 1)
                    {
                        StatusMessage = "Waiting on " + currentPlayer.Color;
                        AvailableActions = currentGameState.AllowedActions;
                    }
                    else
                    {
                        StatusMessage = "Waiting on " + currentPlayer.Color;
                        AvailableActions = new List<string>();
                    }
                }
                var nextPlayer = game.Players.getNextPlayer(currentPlayer);
                if (nextPlayer != null)
                {
                    NextPlayer = nextPlayer.Color;
                }
                var nextFirstPlayer = game.Players.nextFirstPlayer;
                if (nextFirstPlayer != null)
                {
                    NextFirstPlayer = nextFirstPlayer.Color;
                }
            }
        }
    }
}
