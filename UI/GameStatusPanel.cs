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
    /// - Compass showing player positions (N, E, S, W)
    /// - Current player indicator
    /// - Client player position
    /// - Game name and status
    /// - Round number
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

        // Color mapping for factions
        private static readonly Dictionary<string, Color> FactionColors = new Dictionary<string, Color>
        {
            {"red", Color.Red},
            {"blue", Color.Blue},
            {"white", Color.White},
            {"yellow", Color.Yellow}
        };

        // Position to grid coordinates mapping for compass
        // Compass layout:
        //      N
        //  W       E
        //      S
        private static readonly Dictionary<string, Tuple<int, int>> PositionCoordinates = new Dictionary<string, Tuple<int, int>>
        {
            {"N", Tuple.Create(1, 0)},  // Top center
            {"E", Tuple.Create(2, 1)},  // Right middle
            {"S", Tuple.Create(3, 2)},  // Bottom center
            {"W", Tuple.Create(0, 1)}   // Left middle
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
            // Row 0: Compass (left) and Info (right)
            _mainGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            _mainGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));  // Compass
            _mainGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));  // Info

            // Add compass
            AddCompassSection();

            // Add info section
            AddInfoSection();

            this.Widgets.Add(_mainPanel);
        }

        private void AddCompassSection()
        {
            var compassPanel = new Panel()
            {
                Id = "compassPanel",
                Width = 120,
                Height = 120,
                Background = new SolidBrush(Color.Transparent),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var compassGrid = new Grid();
            compassGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));  // N
            compassGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));  // Middle row
            compassGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));  // S
            compassGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // W
            compassGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // Middle column
            compassGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // E

            compassPanel.Widgets.Add(compassGrid);

            // Add compass background image
            var compassTexture = _textures.GetTexture("compass");
            if (compassTexture != null)
            {
                var textureRegion = new TextureRegion(compassTexture);
                var compassImage = new Image()
                {
                    Id = "compassImage",
                    Renderable = textureRegion,
                    Width = 100,
                    Height = 100,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Grid.SetRow(compassImage, 1);
                Grid.SetColumn(compassImage, 1);
                compassGrid.Widgets.Add(compassImage);
            }

            // Add position labels (N, E, S, W)
            foreach (var kvp in PositionCoordinates)
            {
                var position = kvp.Key;
                var row = kvp.Value.Item1;
                var col = kvp.Value.Item2;

                var positionLabel = new Label()
                {
                    Id = $"{position}_position",
                    Text = position,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };

                Grid.SetRow(positionLabel, row);
                Grid.SetColumn(positionLabel, col);
                compassGrid.Widgets.Add(positionLabel);

                // Update the label style based on game status
                UpdatePositionLabel(positionLabel, position);
            }

            Grid.SetColumn(compassPanel, 0);
            Grid.SetRow(compassPanel, 0);
            _mainGrid.Widgets.Add(compassPanel);
        }

        private void AddInfoSection()
        {
            var infoPanel = new Panel()
            {
                Id = "infoPanel",
                Padding = new Thickness(10),
                Background = new SolidBrush(Color.Transparent),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var infoGrid = new Grid();
            infoPanel.Widgets.Add(infoGrid);

            infoGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // My color and name
            infoGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Leader info
            infoGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Game info
            infoGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Status info

            // My color and name
            AddMyInfoSection(infoGrid);

            // Leader info
            AddLeaderInfoSection(infoGrid);

            // Game info
            //AddGameInfoSection(infoGrid);

            // Status info
            AddStatusInfoSection(infoGrid);

            Grid.SetColumn(infoPanel, 1);
            Grid.SetRow(infoPanel, 0);
            _mainGrid.Widgets.Add(infoPanel);
        }

        private void AddMyInfoSection(Grid grid)
        {
            var myInfoPanel = new Panel()
            {
                Id = "myInfoPanel",
                Background = new SolidBrush(Color.Transparent),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var myInfoGrid = new Grid();
            myInfoPanel.Widgets.Add(myInfoGrid);

            myInfoGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // Color dot
            myInfoGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // Name

            // Color indicator
            var colorDot = new Panel()
            {
                Id = "myColorIcon",
                Width = 16,
                Height = 16,
                Background = new SolidBrush(GetFactionColor(_clientPlayer?.Color ?? "white")),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            Grid.SetColumn(colorDot, 0);
            Grid.SetRow(colorDot, 0);
            myInfoGrid.Widgets.Add(colorDot);

            // Player name
            var playerNameLabel = new Label()
            {
                Id = "myName",
                Text = _clientPlayer?.name ?? "Unknown",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            Grid.SetColumn(playerNameLabel, 1);
            Grid.SetRow(playerNameLabel, 0);
            myInfoGrid.Widgets.Add(playerNameLabel);

            Grid.SetRow(myInfoPanel, 0);
            grid.Widgets.Add(myInfoPanel);
        }

        private void AddLeaderInfoSection(Grid grid)
        {
            var leaderPanel = new Panel()
            {
                Id = "leaderPanel",
                Background = new SolidBrush(Color.Transparent),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var leaderLabel = new Label()
            {
                Id = "myLeaderDescription",
                Text = _clientPlayer?.leader?.name ?? "",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            leaderPanel.Widgets.Add(leaderLabel);

            Grid.SetRow(leaderPanel, 1);
            grid.Widgets.Add(leaderPanel);
        }

        private void AddGameInfoSection(Grid grid)
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

            gameInfoGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Game name
            gameInfoGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Round info

            // Game name
            var gameNameLabel = new Label()
            {
                Id = "gameStatusName",
                Text = _gameStatus?.GameName ?? "",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            Grid.SetRow(gameNameLabel, 0);
            gameInfoGrid.Widgets.Add(gameNameLabel);

            // Round info
            var roundLabel = new Label()
            {
                Id = "gameRound",
                Text = $"Round: {_gameStatus?.Round}",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            Grid.SetRow(roundLabel, 1);
            gameInfoGrid.Widgets.Add(roundLabel);

            Grid.SetRow(gameInfoPanel, 2);
            grid.Widgets.Add(gameInfoPanel);
        }

        private void AddStatusInfoSection(Grid grid)
        {
            var statusPanel = new Panel()
            {
                Id = "statusPanel",
                Background = new SolidBrush(Color.Transparent),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var statusGrid = new Grid();
            statusPanel.Widgets.Add(statusGrid);

            statusGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // Status label
            statusGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // Status value

            // Status label
            var statusLabel = new Label()
            {
                Id = "statusLabel",
                Text = "Status: ",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            Grid.SetColumn(statusLabel, 0);
            Grid.SetRow(statusLabel, 0);
            statusGrid.Widgets.Add(statusLabel);

            // Current player indicator
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
            currentPlayerGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // Position

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

            Grid.SetColumn(currentPlayerColorDot, 0);
            Grid.SetRow(currentPlayerColorDot, 0);
            currentPlayerGrid.Widgets.Add(currentPlayerColorDot);

            // Current player position
            var currentPlayerPositionLabel = new Label()
            {
                Id = "currentPlayerPosition",
                Text = "",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            Grid.SetColumn(currentPlayerPositionLabel, 1);
            Grid.SetRow(currentPlayerPositionLabel, 0);
            currentPlayerGrid.Widgets.Add(currentPlayerPositionLabel);

            Grid.SetColumn(currentPlayerPanel, 1);
            Grid.SetRow(currentPlayerPanel, 0);
            statusGrid.Widgets.Add(currentPlayerPanel);

            Grid.SetRow(statusPanel, 3);
            grid.Widgets.Add(statusPanel);
        }

        private Color GetFactionColor(string color)
        {
            if (FactionColors.TryGetValue(color.ToLower(), out var factionColor))
            {
                return factionColor;
            }
            return Color.Gray;
        }

        private void UpdatePositionLabel(Label label, string position)
        {
            // Get player at this position
            var playerAtPosition = _players?.playersByPosition?.GetValueOrDefault(position);

            if (playerAtPosition != null)
            {
                var playerColor = playerAtPosition.Color;
                var isCurrentPlayer = _gameStatus?.CurrentPlayerColor == playerColor;

                // Set background color based on player's faction
                label.Background = new SolidBrush(GetFactionColor(playerColor));

                // If this is the current player, show the position
                if (isCurrentPlayer)
                {
                    label.Text = position;
                }
                else
                {
                    label.Text = "";
                }
            }
            else
            {
                label.Background = new SolidBrush(Color.Transparent);
                label.Text = "";
            }
        }

        public void UpdatePanel(GameStatus gameStatus, Player clientPlayer, GamePlayers players)
        {
            _gameStatus = gameStatus;
            _clientPlayer = clientPlayer;
            _players = players;

            // Update compass positions
            foreach (var kvp in PositionCoordinates)
            {
                var position = kvp.Key;
                var label = (Label)_mainGrid.Widgets.FirstOrDefault(w => w.Id == $"{position}_position");
                if (label != null)
                {
                    UpdatePositionLabel(label, position);
                }
            }

            // Update my info
            var myNameLabel = (Label)_mainGrid.Widgets.FirstOrDefault(w => w.Id == "myName");
            if (myNameLabel != null)
            {
                myNameLabel.Text = _clientPlayer?.name ?? "Unknown";
            }

            // Update leader info
            var leaderLabel = (Label)_mainGrid.Widgets.FirstOrDefault(w => w.Id == "myLeaderDescription");
            if (leaderLabel != null)
            {
                leaderLabel.Text = _clientPlayer?.leader?.description ?? "";
            }

            // Update game info
            var gameNameLabel = (Label)_mainGrid.Widgets.FirstOrDefault(w => w.Id == "gameStatusName");
            if (gameNameLabel != null)
            {
                gameNameLabel.Text = _gameStatus?.GameName ?? "";
            }

            var roundLabel = (Label)_mainGrid.Widgets.FirstOrDefault(w => w.Id == "gameRound");
            if (roundLabel != null)
            {
                roundLabel.Text = $"Round: {_gameStatus?.Round}";
            }

            // Update current player info
            var currentPlayerColorDot = (Panel)_mainGrid.Widgets.FirstOrDefault(w => w.Id == "currentPlayerColor");
            if (currentPlayerColorDot != null)
            {
                var currentPlayerColor = _gameStatus?.CurrentPlayerColor;
                currentPlayerColorDot.Background = new SolidBrush(GetFactionColor(currentPlayerColor ?? "gray"));
            }

            var currentPlayerPositionLabel = (Label)_mainGrid.Widgets.FirstOrDefault(w => w.Id == "currentPlayerPosition");
            if (currentPlayerPositionLabel != null)
            {
                var currentPlayer = _players?.playersByColor?.GetValueOrDefault(_gameStatus?.CurrentPlayerColor ?? "");
                currentPlayerPositionLabel.Text = currentPlayer?.tablePosition ?? "";
            }
        }
    }
}
