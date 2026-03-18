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
using System.Runtime.CompilerServices;

namespace rurik.UI
{
    public class GameSetupScreen : IGameScreen
    {
        private Window _window;
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
        private Label _currentStateLabel;
        private Label _currentStateValueLabel;
        private Label _currentPlayerLabel;
        private Label _currentPlayerValueLabel;
        
        private VerticalStackPanel _gameInfoPanel;
        private VerticalStackPanel _playersPanel;
        private Button _closeButton;
        
        public bool IsVisible = false;
        private GameStatus _game;
        private GameMap _gameMap;
        
        private readonly Desktop _desktop;

        public GameSetupScreen(RurikMonoGame game, Desktop desktop)
        {
            _rurikMonoGame = game;
            _desktop = desktop;
            _window = _desktop.Root as Window;
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

            //Window.Width = 600;
            //Window.Height = 500;

            // Main panel
            Panel = new Panel()
            {
                Id = "mainGameSetupPanel",
                Background = new SolidBrush(Color.Black),
                Width = 600,
                Height = 500,
                //HorizontalAlignment = HorizontalAlignment.Center,
                //VerticalAlignment = VerticalAlignment.Center,
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

            // Current State
            HorizontalStackPanel currentStatePanel = new HorizontalStackPanel();
            _currentStateLabel = new Label()
            {
                Id = "currentStateLabel",
                Text = "Game State:",
                Width = 175,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _currentStateValueLabel = new Label()
            {
                Id = "currentStateValueLabel",
                Text = "",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };
            currentStatePanel.Widgets.Add(_currentStateLabel);
            currentStatePanel.Widgets.Add(_currentStateValueLabel);

            // Current Player
            HorizontalStackPanel currentPlayerPanel = new HorizontalStackPanel();
            _currentPlayerLabel = new Label()
            {
                Id = "currentPlayerLabel",
                Text = "Current Player:",
                Width = 175,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _currentPlayerValueLabel = new Label()
            {
                Id = "currentPlayerValueLabel",
                Text = "",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };
            currentPlayerPanel.Widgets.Add(_currentPlayerLabel);
            currentPlayerPanel.Widgets.Add(_currentPlayerValueLabel);

            _gameInfoPanel.Widgets.Add(gameIdPanel);
            _gameInfoPanel.Widgets.Add(gameNamePanel);
            _gameInfoPanel.Widgets.Add(gameOwnerPanel);
            _gameInfoPanel.Widgets.Add(numberOfPlayersPanel);
            _gameInfoPanel.Widgets.Add(currentStatePanel);
            _gameInfoPanel.Widgets.Add(currentPlayerPanel);
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
            //Globals.Log("Show(): enter, window=" + _window.Id + ", panel=" + _window.Content.Id);
            IsVisible = true;
            _window.Content.RemoveFromParent();
            _window.Content = null;
            _window.Content = Panel;
            _window.Title = "Rurik: Game Setup";
            _rurikMonoGame.CurrentMyraScreen = "GameSetup";
            //Globals.Log("Show(): exit, window=" + _window.Id + ", panel=" + _window.Content.Id);
        }

        public void Hide()
        {
            IsVisible = false;
            //_window.Close();
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
            UpdateGameInfo(game, null);
        }

        public void UpdateGameInfo(GameStatus game, GameMap map)
        {
            Globals.Log("UpdateGameInfo(): enter");
            _game = game;
            if (map != null)
            {
                _gameMap = map;
            }
        
            // Update game info labels
            if (game != null)
            {
                _gameIdValueLabel.Text = game.Id;
                _gameNameValueLabel.Text = game.Name;
                _gameOwnerValueLabel.Text = game.Owner;
                _numberOfPlayersValueLabel.Text = $"{game.NumberOfPlayers}/{game.TargetNumberOfPlayers}";
                _currentStateValueLabel.Text = game.CurrentState ?? "";
            }

            // Update players list
            updatePlayersList();

            // Check if game state is waitingForFirstPlayerSelection and show modal
            if (game != null && game.CurrentState == "waitingForFirstPlayerSelection" && ! _rurikMonoGame.ChooseFirstPlayerModal.IsVisible)
            {
                // Check if this client is the game owner
                if (game.Owner != null && _rurikMonoGame.Client.ClientIdentifier != null &&
                    game.Owner.Equals(_rurikMonoGame.Client.ClientIdentifier))
                {
                    // Show the choose first player modal
                    if (_rurikMonoGame.ChooseFirstPlayerModal != null)
                    {
                        _rurikMonoGame.ChooseFirstPlayerModal.UpdateGameInfo(game);
                        _rurikMonoGame.ChooseFirstPlayerModal.Show();
                    }
                }
                return;
            }

            // Check if game state is waitingForLeaderSelection and show modal
            if (game != null && game.CurrentState == "waitingForLeaderSelection" &&
                    game.CurrentPlayerName != null && _rurikMonoGame.Client.ClientIdentifier != null &&
                game.CurrentPlayerName.Equals(_rurikMonoGame.Client.ClientIdentifier))
            {
                // Show the choose leader modal
                if (_rurikMonoGame.ChooseLeaderModal != null && !_rurikMonoGame.ChooseLeaderModal.IsVisible )
                {
                    _rurikMonoGame.ChooseLeaderModal.UpdateGameInfo(game);
                    _rurikMonoGame.ChooseLeaderModal.Show();
                }
                return;
            }

            // Check if game state is waitingForSecretAgendaSelection and show modal
            if (game != null && game.CurrentState == "waitingForSecretAgendaSelection" &&
                    game.CurrentPlayerName != null && _rurikMonoGame.Client.ClientIdentifier != null &&
                game.CurrentPlayerName.Equals(_rurikMonoGame.Client.ClientIdentifier))
            {
                // Show the choose secret agenda modal
                if (_rurikMonoGame.ChooseSecretAgendaModal != null && !_rurikMonoGame.ChooseSecretAgendaModal.IsVisible)
                {
                    _rurikMonoGame.ChooseSecretAgendaModal.UpdateGameInfo(game);
                    _rurikMonoGame.ChooseSecretAgendaModal.Show();
                }
                return;
            }

            // Check if game state is waitingForTroopPlacement and show MainGameScreen
            if (game != null && game.CurrentState == "waitingForTroopPlacement")
            {
                // Switch to MainGameScreen
                MainGameScreen mainGameScreen = _rurikMonoGame.MainGameScreen;
                _rurikMonoGame.CurrentMyraScreen = "MainGameScreen";
                mainGameScreen.UpdateGameInfo(game, _gameMap);
                mainGameScreen.Show();
                this.Hide();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void updatePlayersList()
        {
            // Clear existing player widgets (keep only _playersLabel)
            //_playersPanel.Widgets.Clear();
            //_playersPanel.Widgets.Add(_playersLabel);
            for (int i = _playersPanel.Widgets.Count - 1; i >= 0; i--)
            {
                if (_playersPanel.Widgets[i] != _playersLabel)
                {
                    _playersPanel.Widgets.RemoveAt(i);
                }
            }

            // Update current player value label (in game info panel)
            if (_game != null && !string.IsNullOrEmpty(_game.CurrentPlayerName))
            {
                _currentPlayerValueLabel.Text = _game.CurrentPlayerName;
            }
            else
            {
                _currentPlayerValueLabel.Text = "";
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