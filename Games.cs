using System;
using System.Collections.Generic;
using System.Linq;

namespace rurik
{
    public class Games
    {
        private Dictionary<string, RurikGame> games;
        private Dictionary<string, GameStatus> gameSummaries;
        private Dictionary<string, RurikGame> undoGames;
        private static Games self;

        public Games()
        {
            games = new Dictionary<string, RurikGame>();
            gameSummaries = new Dictionary<string, GameStatus>();
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
            Console.WriteLine("createGame(): gameId=" + game.Id);
            games[game.Id] = game;
            var gameStatus = new GameStatus(game, null);
            return gameStatus;
        }

        public RurikGame GetGameById(string id)
        {
            return games.ContainsKey(id) ? games[id] : null;
        }

        public GameStatus GetGameStatus(string gameId, string clientColor = null)
        {
            if (!games.ContainsKey(gameId))
                return null;
            
            var game = games[gameId];
            var gameStatus = new GameStatus(game, clientColor);
            return gameStatus;
        }

        public List<GameStatus> ListGames()
        {
            var gameStatusList = new List<GameStatus>();
            foreach (var game in games.Values)
            {
                var gameStatus = new GameStatus(game, null);
                gameStatusList.Add(gameStatus);
            }
            return gameStatusList;
        }
    }

}
