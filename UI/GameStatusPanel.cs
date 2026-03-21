using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using rurik;

namespace rurik.UI
{
    /// <summary>
    /// UI Panel for displaying game status including:
    /// - Client player's name, color, and leader name
    /// - Round number
    /// - Game state
    /// - Current player (color + name)
    /// </summary>
    public class GameStatusPanel : Panel
    {
        private readonly Desktop _desktop;
        private GameStatus _gameStatus;
        private readonly Textures _textures;
        private Player _clientPlayer;
        private GamePlayers _players;

        private Panel _mainPanel;
        private Grid _mainGrid;

        private Label stateLabel;
        private Label currentPlayerNameLabel;
        private Label roundLabel;

        // Color mapping for factions
        private static readonly Dictionary<string, Color> FactionColors = new Dictionary<string, Color>
        {
            {"red", Color.Red},
            {"blue", Color.Blue},
            {"white", Color.White},
            {"yellow", Color.Yellow}
        };

        public GameStatusPanel(Desktop desktop, GameStatus gameStatus, Textures textures, Player clientPlayer, GamePlayers players)
            : base()
        {
            _desktop = desktop;
            _gameStatus = gameStatus;
            _textures = textures;
            _clientPlayer = clientPlayer;
            _players = players;

            Initialize();
        }

        private void Initialize()
        {
            // Set base panel properties
            this.Id = "gameStatusPanel";
            this.HorizontalAlignment = HorizontalAlignment.Center;
            this.VerticalAlignment = VerticalAlignment.Top;

            // Create main container
            _mainPanel = new Panel()
            {
                Id = "gameStatusMain",
                Background = new SolidBrush(new Color(80, 80, 80, 200)),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
            };

            _mainGrid = new Grid();
            _mainPanel.Widgets.Add(_mainGrid);

            // Grid layout:
            // Row 0: Client player info (name, color, leader)
            // Row 1: Game info (round, state)
            // Row 2: Current player info
            _mainGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));  // Client info
            _mainGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));  // Game info
            _mainGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));  // Current player

            // Add client player info section
            AddClientInfoSection();

            // Add game info section
            AddGameInfoSection();

            // Add current player info section
            AddCurrentPlayerSection();

            this.Widgets.Add(_mainPanel);
        }

        private void AddClientInfoSection()
        {
            var clientPanel = new Panel()
            {
                Id = "clientInfoPanel",
                Background = new SolidBrush(Color.Transparent),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var clientGrid = new Grid();
            clientPanel.Widgets.Add(clientGrid);

            clientGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // Color dot
            clientGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // Name
            clientGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // Leader

            // Color indicator
            var colorDot = new Panel()
            {
                Id = "clientColorIcon",
                Width = 16,
                Height = 16,
                Background = new SolidBrush(GetFactionColor(_clientPlayer?.Color ?? "white")),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            Grid.SetColumn(colorDot, 0);
            Grid.SetRow(colorDot, 0);
            clientGrid.Widgets.Add(colorDot);

            // Player name
            var playerNameLabel = new Label()
            {
                Id = "clientName",
                Text = " " + _clientPlayer?.name ?? "Unknown",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            Grid.SetColumn(playerNameLabel, 1);
            Grid.SetRow(playerNameLabel, 0);
            clientGrid.Widgets.Add(playerNameLabel);

            // Leader name
            var leaderLabel = new Label()
            {
                Id = "clientLeader",
                Text = " " + _clientPlayer?.leader?.name ?? "",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            Grid.SetColumn(leaderLabel, 2);
            Grid.SetRow(leaderLabel, 0);
            clientGrid.Widgets.Add(leaderLabel);

            Grid.SetRow(clientPanel, 0);
            _mainGrid.Widgets.Add(clientPanel);
        }

        private void AddGameInfoSection()
        {
            var gameInfoPanel = new Panel()
            {
                Id = "gameInfoPanel",
                Background = new SolidBrush(Color.Transparent),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var gameInfoGrid = new Grid();
            gameInfoPanel.Widgets.Add(gameInfoGrid);

            gameInfoGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // Round
            gameInfoGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // State

            // Round info
            roundLabel = new Label()
            {
                Id = "gameRound",
                Text = $"Round: {_gameStatus?.Round}",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            Grid.SetColumn(roundLabel, 0);
            Grid.SetRow(roundLabel, 0);
            gameInfoGrid.Widgets.Add(roundLabel);

            // Game state
            stateLabel = new Label()
            {
                Id = "gameState",
                Text = $" {_gameStatus?.CurrentState ?? "N/A"}",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            Grid.SetColumn(stateLabel, 1);
            Grid.SetRow(stateLabel, 0);
            gameInfoGrid.Widgets.Add(stateLabel);

            Grid.SetRow(gameInfoPanel, 1);
            _mainGrid.Widgets.Add(gameInfoPanel);
        }

        private void AddCurrentPlayerSection()
        {
            var currentPlayerPanel = new Panel()
            {
                Id = "currentPlayerPanel",
                Background = new SolidBrush(Color.Transparent),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var currentPlayerGrid = new Grid();
            currentPlayerPanel.Widgets.Add(currentPlayerGrid);

            currentPlayerGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // Color dot
            currentPlayerGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // Name

            // Current player color dot
            var currentPlayerColorDot = new Panel()
            {
                Id = "currentPlayerColor",
                Width = 16,
                Height = 16,
                Background = new SolidBrush(Color.Gray), // Default gray if no current player
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            //Grid.SetColumn(currentPlayerColorDot, 0);
            //Grid.SetRow(currentPlayerColorDot, 0);
            //currentPlayerGrid.Widgets.Add(currentPlayerColorDot);

            // Current player name
            currentPlayerNameLabel = new Label()
            {
                Id = "currentPlayerName",
                Text = "currentPlayer: " + _gameStatus?.CurrentPlayerName ?? "N/A",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            //Grid.SetColumn(currentPlayerNameLabel, 1);
            Grid.SetRow(currentPlayerNameLabel, 0);
            currentPlayerGrid.Widgets.Add(currentPlayerNameLabel);

            Grid.SetRow(currentPlayerPanel, 2);
            _mainGrid.Widgets.Add(currentPlayerPanel);
        }

        private Color GetFactionColor(string color)
        {
            if (FactionColors.TryGetValue(color.ToLower(), out var factionColor))
            {
                return factionColor;
            }
            return Color.Gray;
        }

        public void UpdatePanel(GameStatus gameStatus, Player clientPlayer, GamePlayers players)
        {
            Globals.Log("UpdatePanel(): enter");
            _gameStatus = gameStatus;
            _clientPlayer = clientPlayer;
            _players = players;

            if (stateLabel != null)
            {
                stateLabel.Text = $" {_gameStatus?.CurrentState ?? "N/A"}";
            }
            if (currentPlayerNameLabel != null)
            {
                currentPlayerNameLabel.Text = "currentPlayer: " + _gameStatus?.CurrentPlayerName ?? "N/A";
            }

            if (roundLabel != null)
            {
                roundLabel.Text = $"Round: {_gameStatus?.Round}";
            }


        }
    }
}
