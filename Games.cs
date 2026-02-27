using System;
using System.Collections.Generic;
using System.Linq;

namespace rurik
{
    public class Games
    {
        private Dictionary<string, RurikGame> GameIdToGameMap {get; set;}
        public Dictionary<string, GameStatus> GameIdToGameStatus  {get; set;}
        private Dictionary<string, RurikGame> undoGames;
        private static Games self;

        public Games()
        {
            GameIdToGameMap = new Dictionary<string, RurikGame>();
            GameIdToGameStatus = new Dictionary<string, GameStatus>();
            undoGames = new Dictionary<string, RurikGame>();
        }

        public static Games GetInstance()
        {
            if (self == null)
            {
                self = new Games();
            }
            return self;
        }

        public GameStatus CreateGame(string name, string owner, int targetNumberOfPlayers, string password = null)
        {
            var game = new RurikGame(name, owner, targetNumberOfPlayers, password);
            Globals.Log("createGame(): gameId=" + game.Id);
            GameIdToGameMap[game.Id] = game;
            var gameStatus = new GameStatus(game, null);
            GameIdToGameStatus[game.Id] = gameStatus;
            return gameStatus;
        }

        public RurikGame GetGameById(string id)
        {
            return GameIdToGameMap.ContainsKey(id) ? GameIdToGameMap[id] : null;
        }

        public GameStatus GetGameStatus(string gameId, string clientColor = null)
        {
            if (!GameIdToGameMap.ContainsKey(gameId))
                return null;
            
            var game = GameIdToGameMap[gameId];
            var gameStatus = new GameStatus(game, clientColor);
            return gameStatus;
        }

        public List<GameStatus> ListGames()
        {
            var gameStatusList = new List<GameStatus>();
            foreach (var gameStatus in GameIdToGameStatus.Values)
            {
                gameStatusList.Add(gameStatus);
            }
            return gameStatusList;
        }
    }

}
