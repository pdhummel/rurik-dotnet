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
    public class GameListScreen : IGameScreen
    {
        private Window _window = new Window();
        private RurikMonoGame RurikMonoGame;
        private Grid _grid;
        private Panel _panel;
        //private Button _closeButton;
        private Button _createGameButton;
        private Button _refreshButton;
        private ListView _gameListView;
        private Label _titleLabel;
        private Label _playerNameLabel;
        private TextBox _playerNameInput;
        private Button _loginButton;
        private Panel _loginPanel;
        private VerticalStackPanel _gameListPanel;
        private Panel _createGamePanel;
        private bool _isLoggedIn = false;
        private string _currentPlayerName = "";
        private bool _isVisible = false;

        private Desktop desktop;

        public GameListScreen(RurikMonoGame game, Desktop desktop)
        {
            RurikMonoGame = game;
            this.desktop = desktop;
            Initialize();
        }

        public void Initialize()
        {
            _window.Width = RurikMonoGame.Window.ClientBounds.Width;
            _window.Height = RurikMonoGame.Window.ClientBounds.Height;

            // Main panel
            _panel = new Panel()
            {
                Id = "gameListPanel",
                Background = new SolidBrush(Color.Black),
                Width = _window.Width,
                Height = _window.Height,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _grid = new Grid()
            {
                Id = "gameListGrid",
                Background = new SolidBrush(Color.Black),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            _grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));

            // Title label
            _titleLabel = new Label()
            {
                Id = "titleLabel",
                Text = "Rurik Game List",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Gray),
            };

            // Login panel
            _loginPanel = new Panel()
            {
                Id = "loginPanel",
                Background = new SolidBrush(Color.Gray),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            var loginLabel = new Label()
            {
                Id = "loginLabel",
                Text = "Who are you?",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _playerNameInput = new TextBox()
            {
                Id = "playerNameInput",
                Width = 200,
                Height = 30,
                Text = "",
            };

            _loginButton = new Button()
            {
                Id = "loginButton",
                Width = 100,
                Height = 30,
            };
            _loginButton.Content = new Label
            {
                Text = "Login",
                Width = 75,
                Border = new SolidBrush("#808000FF"),
                BorderThickness = new Thickness(2)
            };

            _loginButton.Click += (s, a) => Login();

            _loginPanel.Widgets.Add(loginLabel);
            _loginPanel.Widgets.Add(_playerNameInput);
            _loginPanel.Widgets.Add(_loginButton);

            // Game list panel
            _gameListPanel = new VerticalStackPanel()
            {
                Id = "gameListPanel",
                Background = new SolidBrush(Color.Gray),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            var gameListLabel = new Label()
            {
                Id = "gameListLabel",
                Text = "Available Games",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _refreshButton = new Button()
            {
                Id = "refreshButton",
                Width = 130,
                Height = 30,
                Border = new SolidBrush("#808000FF"),
                BorderThickness = new Thickness(2)
            };
            _refreshButton.Content = new Label
            {
                Id = "refreshButtonLabel",
                Text = "Refresh Games",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };            
            _refreshButton.Click += (s, a) => RefreshGameList();


            _gameListView = new ListView()
            {
                Id = "gameListView",
                Width = _panel.Width,
                Height = _panel.Height - _refreshButton.Height -10,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };


            // Create game panel
            _createGamePanel = new Panel()
            {
                Id = "createGamePanel",
                Background = new SolidBrush(Color.Gray),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            var createGameLabel = new Label()
            {
                Id = "createGameLabel",
                Text = "Create New Game",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var gameNameLabel = new Label()
            {
                Id = "gameNameLabel",
                Text = "Game Name:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var gameNameInput = new TextBox()
            {
                Id = "gameNameInput",
                Width = 200,
                Height = 30,
                Text = "",
            };

            var playerColorLabel = new Label()
            {
                Id = "playerColorLabel",
                Text = "Player Color:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var playerColorSelect = new ComboView()
            {
                Id = "playerColorSelect",
                Width = 100,
                Height = 30,
            };

            playerColorSelect.Widgets.Add(new Label() {Text="blue"});
            playerColorSelect.Widgets.Add(new Label() {Text="red"});
            playerColorSelect.Widgets.Add(new Label() {Text="white"});
            playerColorSelect.Widgets.Add(new Label() {Text="yellow"});

            var playerPositionLabel = new Label()
            {
                Id = "playerPositionLabel",
                Text = "Player Position:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var playerPositionSelect = new ComboView()
            {
                Id = "playerPositionSelect",
                Width = 100,
                Height = 30,
            };

            playerPositionSelect.Widgets.Add(new Label() {Text="N"});
            playerPositionSelect.Widgets.Add(new Label() {Text="E"});
            playerPositionSelect.Widgets.Add(new Label() {Text="S"});
            playerPositionSelect.Widgets.Add(new Label() {Text="W"});

            _createGameButton = new Button()
            {
                Id = "createGameButton",
                Width = 120,
                Height = 30,
            };
            _createGameButton.Content = new Label() {Text="Create Game"};

            _createGameButton.Click += (s, a) => CreateGame(gameNameInput.Text, 
                ((Label)playerColorSelect.SelectedItem).Text, 
                ((Label)playerPositionSelect.SelectedItem).Text);

            _createGamePanel.Widgets.Add(createGameLabel);
            _createGamePanel.Widgets.Add(gameNameLabel);
            _createGamePanel.Widgets.Add(gameNameInput);
            _createGamePanel.Widgets.Add(playerColorLabel);
            _createGamePanel.Widgets.Add(playerColorSelect);
            _createGamePanel.Widgets.Add(playerPositionLabel);
            _createGamePanel.Widgets.Add(playerPositionSelect);
            _createGamePanel.Widgets.Add(_createGameButton);

            // Add widgets to grid
            //_grid.Widgets.Add(_titleLabel);
            //_grid.Widgets.Add(_loginPanel);
            Grid.SetRow(_refreshButton, 0);
            _grid.Widgets.Add(_refreshButton);

            _gameListPanel.Widgets.Add(gameListLabel);
            _gameListPanel.Widgets.Add(_gameListView);
            Grid.SetRow(_gameListPanel, 1);
            _grid.Widgets.Add(_gameListPanel);
            //_grid.Widgets.Add(_gameListView);
            //_grid.Widgets.Add(_createGamePanel);

            _panel.Widgets.Add(_grid);
            _window.Content = _panel;


            // Initially hide the create game panel
            _createGamePanel.Visible = false;
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
            // Add to desktop or parent container
            desktop.Root = _window;
            //desktop.Widgets.Add(_panel);
        }

        public void Hide()
        {
            _isVisible = false;
            
            // Remove from desktop or parent container
            _window.RemoveFromParent();
        }

        public void HandleEvent(string eventName, object data)
        {
            switch (eventName)
            {
                case "refreshGames":
                    RefreshGameList();
                    break;
            }
        }

        private void Login()
        {
            _currentPlayerName = _playerNameInput.Text;
            if (!string.IsNullOrEmpty(_currentPlayerName))
            {
                _isLoggedIn = true;
                _loginPanel.Visible = false;
                _gameListPanel.Visible = true;
                _createGamePanel.Visible = true;
                _playerNameLabel = new Label()
                {
                    Id = "playerNameLabel",
                    Text = "Hello " + _currentPlayerName + "!",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                // Add the player name label to the grid
                _grid.Widgets.Add(_playerNameLabel);
                RefreshGameList();
            }
        }

        private void RefreshGameList()
        {
            // Clear existing items
            _gameListView.Widgets.Clear();

            // Get list of games
            var games = Games.GetInstance().ListGames();
            
            // Add games to list view
            foreach (var game in games)
            {
                var gameItem = new Label()
                {
                    Text = $"Game: {game.GameName} | Owner: {game.Owner} | Players: {game.NumberOfPlayers}/{game.TargetNumberOfPlayers} | State: {game.CurrentState}",
                };
                
                // Add click event to join the game
                //gameItem.Click += (s, a) => JoinGame(game.GameId);
                
                _gameListView.Widgets.Add(gameItem);
            }
        }

        private void JoinGame(string gameId)
        {
            // In a real implementation, this would handle joining a game
            // For now, we'll just show a message
            var game = Games.GetInstance().GetGameStatus(gameId);
            if (game != null)
            {
                // This would normally open a join game dialog or redirect to the game
                // For now, just log that a game was joined
                System.Console.WriteLine($"Player {_currentPlayerName} joined game {game.GameName}");
            }
        }

        private void CreateGame(string gameName, string color, string position)
        {
            if (string.IsNullOrEmpty(gameName))
            {
                return;
            }

            var gameStatus = Games.GetInstance().CreateGame(gameName, _currentPlayerName, 4);
            // Join the newly created game
            var game = Games.GetInstance().GetGameStatus(gameStatus.GameId);
            if (game != null)
            {
                // This would normally open a join game dialog or redirect to the game
                // For now, just log that a game was created and joined
                System.Console.WriteLine($"Player {_currentPlayerName} created and joined game {gameName}");
            }
            RefreshGameList();
        }
    }
}
