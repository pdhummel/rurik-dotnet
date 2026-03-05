using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using rurik;

namespace rurik.UI
{
    public class GameSetup : IGameScreen
    {
        public Window Window {get;} = new Window();
        private RurikMonoGame _rurikMonoGame;
        private Grid _grid;
        public Panel Panel { get; set; }
        
        private Label _titleLabel;
        private Label _gameIdLabel;
        private Label _gameIdValueLabel;
        private Label _gameNameLabel;
        private Label _gameNameValueLabel;
        private Label _gameOwnerLabel;
        private Label _gameOwnerValueLabel;
        private Label _numberOfPlayersLabel;
        private Label _numberOfPlayersValueLabel;
        private Label _playersLabel;
        
        private VerticalStackPanel _gameInfoPanel;
        private VerticalStackPanel _playersPanel;
        private Button _closeButton;
        
        private bool _isVisible = false;
        private GameStatus _game;
        
        private readonly Desktop _desktop;

        public GameSetup(RurikMonoGame game, Desktop desktop)
        {
            _rurikMonoGame = game;
            _desktop = desktop;
            game.GameSetup = this;
            Initialize();
        }

        public void Initialize()
        {
            // Title label
            _titleLabel = new Label()
            {
                Id = "titleLabel",
                Text = "Game Setup",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Gray),
            };

            Window.Width = 600;
            Window.Height = 500;
            Window.Title = "Rurik: Game Setup";

            // Main panel
            Panel = new Panel()
            {
                Id = "mainGameSetupPanel",
                Background = new SolidBrush(Color.Black),
                Width = 600,
                Height = 500,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _grid = new Grid()
            {
                Id = "gameSetupGrid",
                Background = new SolidBrush(Color.Black),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Title
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Game Info
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Players
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Buttons

            _grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // Label column
            _grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // Value column

            setupGameInfoPanel();
            setupPlayersPanel();
            //setupCloseButton();

            Grid.SetRow(_gameInfoPanel, 1);
            _grid.Widgets.Add(_gameInfoPanel);

            Grid.SetRow(_playersPanel, 2);
            _grid.Widgets.Add(_playersPanel);

            //Grid.SetRow(_closeButton, 3);
            //_grid.Widgets.Add(_closeButton);

            Panel.Widgets.Add(_grid);
            Window.Content = Panel;
        }

        private void setupGameInfoPanel()
        {
            _gameInfoPanel = new VerticalStackPanel()
            {
                Id = "gameInfoPanel",
                Background = new SolidBrush(Color.Gray),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Game ID
            HorizontalStackPanel gameIdPanel = new HorizontalStackPanel();
            _gameIdLabel = new Label()
            {
                Id = "gameIdLabel",
                Text = "Game ID:",
                Width = 175,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _gameIdValueLabel = new Label()
            {
                Id = "gameIdValueLabel",
                Text = "",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };
            gameIdPanel.Widgets.Add(_gameIdLabel);
            gameIdPanel.Widgets.Add(_gameIdValueLabel);

            // Game Name
            HorizontalStackPanel gameNamePanel = new HorizontalStackPanel();
            _gameNameLabel = new Label()
            {
                Id = "gameNameLabel",
                Text = "Game Name:",
                Width = 175,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _gameNameValueLabel = new Label()
            {
                Id = "gameNameValueLabel",
                Text = "",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };
            gameNamePanel.Widgets.Add(_gameNameLabel);
            gameNamePanel.Widgets.Add(_gameNameValueLabel);

            // Game Owner
            HorizontalStackPanel gameOwnerPanel = new HorizontalStackPanel();
            _gameOwnerLabel = new Label()
            {
                Id = "gameOwnerLabel",
                Text = "Game Owner:",
                Width = 175,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _gameOwnerValueLabel = new Label()
            {
                Id = "gameOwnerValueLabel",
                Text = "",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };
            gameOwnerPanel.Widgets.Add(_gameOwnerLabel);
            gameOwnerPanel.Widgets.Add(_gameOwnerValueLabel);

            // Number of Players
            HorizontalStackPanel numberOfPlayersPanel = new HorizontalStackPanel();
            _numberOfPlayersLabel = new Label()
            {
                Id = "numberOfPlayersLabel",
                Text = "Number of Players:",
                Width = 175,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _numberOfPlayersValueLabel = new Label()
            {
                Id = "numberOfPlayersValueLabel",
                Text = "",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };
            numberOfPlayersPanel.Widgets.Add(_numberOfPlayersLabel);
            numberOfPlayersPanel.Widgets.Add(_numberOfPlayersValueLabel);

            _gameInfoPanel.Widgets.Add(gameIdPanel);
            _gameInfoPanel.Widgets.Add(gameNamePanel);
            _gameInfoPanel.Widgets.Add(gameOwnerPanel);
            _gameInfoPanel.Widgets.Add(numberOfPlayersPanel);
        }

        private void setupPlayersPanel()
        {
            _playersPanel = new VerticalStackPanel()
            {
                Id = "playersPanel",
                Background = new SolidBrush(Color.Gray),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            _playersLabel = new Label()
            {
                Id = "playersLabel",
                Text = "Players:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _playersPanel.Widgets.Add(_playersLabel);
        }

        private void setupCloseButton()
        {
            _closeButton = new Button()
            {
                Id = "closeButton",
                Width = 100,
                Height = 30,
                Border = new SolidBrush("#808000FF"),
                BorderThickness = new Thickness(2)
            };
            _closeButton.Content = new Label
            {
                Text = "Close",
            };
            _closeButton.Click += (s, a) => Hide();
        }

        public void Update()
        {
            // Update logic if needed
        }

        public void Draw()
        {
            // Draw logic if needed
        }

        public void Show()
        {
            _isVisible = true;
            //Window.ShowModal(_desktop);
            //_desktop.Root = Window;
            Window window = (Window)(_desktop.Root);
            Panel.Width = window.Width;
            Panel.Height = window.Height;
            window.Content = Panel;
            window.Title = "Game Setup";
        }

        public void Hide()
        {
            _isVisible = false;
            Window.Close();
        }

        public void HandleEvent(string eventName, object data)
        {
            switch (eventName)
            {
                case "updateGameInfo":
                    UpdateGameInfo(data as GameStatus);
                    break;
            }
        }

        public void UpdateGameInfo(GameStatus game)
        {
            Globals.Log("UpdateGameInfo(): enter");
            _game = game;
            
            // Update game info labels
            if (game != null)
            {
                _gameIdValueLabel.Text = game.Id;
                _gameNameValueLabel.Text = game.Name;
                _gameOwnerValueLabel.Text = game.Owner;
                _numberOfPlayersValueLabel.Text = game.NumberOfPlayers.ToString();
            }

            // Update players list
            updatePlayersList();
        }

        private void updatePlayersList()
        {
            // Clear existing player widgets
            for (int i = _playersPanel.Widgets.Count - 1; i >= 0; i--)
            {
                if (_playersPanel.Widgets[i] != _playersLabel)
                {
                    _playersPanel.Widgets.RemoveAt(i);
                }
            }

            // Add player info
            if (_game != null)
            {
                foreach (var player in _game.Players.players)
                {
                    HorizontalStackPanel playerPanel = new HorizontalStackPanel();
                    
                    Label playerLabel = new Label()
                    {
                        Id = "playerLabel_" + player.Color,
                        Text = player.Color + ": " + player.name,
                        Width = 200,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    
                    Label positionLabel = new Label()
                    {
                        Id = "positionLabel_" + player.Color,
                        Text = "Position: " + player.tablePosition,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    
                    playerPanel.Widgets.Add(playerLabel);
                    playerPanel.Widgets.Add(positionLabel);
                    
                    _playersPanel.Widgets.Add(playerPanel);
                }
            }
        }
    }
}