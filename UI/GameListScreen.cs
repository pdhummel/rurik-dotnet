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
using rurik.Actions;

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
        private Button _createServerButton;
        private Button _openCreateGameButton;
        private VerticalStackPanel _loginPanel;
        private VerticalStackPanel _gameListPanel;
        private VerticalStackPanel _createGamePanel;
        private bool _isLoggedIn = false;
        private string _currentPlayerName = "";
        private bool _isVisible = false;
        private Window _createGameWindow;

        private readonly Desktop Desktop;

        public GameListScreen(RurikMonoGame game, Desktop desktop)
        {
            RurikMonoGame = game;
            Desktop = desktop;
            Initialize();
        }

        public void Initialize()
        {
            // Title label
            _titleLabel = new Label()
            {
                Id = "titleLabel",
                Text = "Rurik Game List",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Gray),
            };

            _window.Width = RurikMonoGame.Window.ClientBounds.Width;
            _window.Height = RurikMonoGame.Window.ClientBounds.Height;
            _window.Title = "Rurik: Dawn of Kyiv";

            // Main panel
            _panel = new Panel()
            {
                Id = "mainGameListPanel",
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


            setupLoginPanel();
            setupGameListPanel();
            setupCreateGamePanel();



            // Add widgets to grid
            //_grid.Widgets.Add(_titleLabel);
            Grid.SetRow(_loginPanel, 0);
            _grid.Widgets.Add(_loginPanel);


            Grid.SetRow(_gameListPanel, 1);
            _grid.Widgets.Add(_gameListPanel);

            _panel.Widgets.Add(_grid);
            _window.Content = _panel;


        }

        private void setupLoginPanel()
        {
            // Login panel
            _loginPanel = new VerticalStackPanel()
            {
                Id = "loginPanel",
                Background = new SolidBrush(Color.Gray),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            HorizontalStackPanel namePanel = new HorizontalStackPanel();
            var loginLabel = new Label()
            {
                Id = "loginLabel",
                Text = "Name?",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _playerNameInput = new TextBox()
            {
                Id = "playerNameInput",
                Width = 200,
                Height = 30,
                Text = "",
                Border = new SolidBrush("#808000FF"),
                BorderThickness = new Thickness(2),
            };
            namePanel.Widgets.Add(loginLabel);
            namePanel.Widgets.Add(_playerNameInput);

            HorizontalStackPanel hostPanel = new HorizontalStackPanel();
            var hostLabel = new Label()
            {
                Id = "hostLabel",
                Text = "host:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var hostInput = new TextBox()
            {
                Id = "hostInput",
                Width = 200,
                Height = 30,
                Text = "127.0.0.1",
                Border = new SolidBrush("#808000FF"),
                BorderThickness = new Thickness(2),
            };
            hostPanel.Widgets.Add(hostLabel);
            hostPanel.Widgets.Add(hostInput);

            HorizontalStackPanel portPanel = new HorizontalStackPanel();
            var portLabel = new Label()
            {
                Id = "portLabel",
                Text = "port:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var portInput = new TextBox()
            {
                Id = "portInput",
                Width = 200,
                Height = 30,
                Text = "5005",
                Border = new SolidBrush("#808000FF"),
                BorderThickness = new Thickness(2),
            };
            portPanel.Widgets.Add(portLabel);
            portPanel.Widgets.Add(portInput);

            HorizontalStackPanel loginButtonPanel = new HorizontalStackPanel();
            _createServerButton = new Button()
            {
                Id = "createServerButton",
                Width = 125,
                Height = 30,
                Border = new SolidBrush("#808000FF"),
                BorderThickness = new Thickness(2)
            };
            _createServerButton.Content = new Label
            {
                Text = "Create Server",
            };
            _createServerButton.Click += (s, a) => StartServer();

            var joinServerButton = new Button()
            {
                Id = "joinServerButton",
                Width = 125,
                Height = 30,
                Border = new SolidBrush("#808000FF"),
                BorderThickness = new Thickness(2)
            };
            joinServerButton.Content = new Label
            {
                Text = "Join Server",
            };
            joinServerButton.Click += (s, a) => Login();
            loginButtonPanel.Widgets.Add(_createServerButton);
            loginButtonPanel.Widgets.Add(joinServerButton);


            _loginPanel.Widgets.Add(namePanel);
            _loginPanel.Widgets.Add(hostPanel);
            _loginPanel.Widgets.Add(portPanel);
            _loginPanel.Widgets.Add(loginButtonPanel);
        }

        private void setupGameListPanel()
        {
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

            _openCreateGameButton = new Button()
            {
                Id = "openCreateGameButton",
                Width = 130,
                Height = 30,
                Border = new SolidBrush("#808000FF"),
                BorderThickness = new Thickness(2)
            };
            _openCreateGameButton.Content = new Label
            {
                Id = "openCreateGameButtonLabel",
                Text = "Create Game",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };            
            _openCreateGameButton.Click += (s, a) => OpenCreateGame();

            _gameListView = new ListView()
            {
                Id = "gameListView",
                Width = _panel.Width,
                Height = _panel.Height - _refreshButton.Height -10,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            HorizontalStackPanel gameListButtonPanel = new HorizontalStackPanel();
            gameListButtonPanel.Widgets.Add(_refreshButton);
            gameListButtonPanel.Widgets.Add(_openCreateGameButton);

            _gameListPanel.Visible = false;
            _gameListPanel.Widgets.Add(gameListButtonPanel);
            _gameListPanel.Widgets.Add(gameListLabel);
            _gameListPanel.Widgets.Add(_gameListView);

        }

        private void setupCreateGamePanel()
        {
            // Create game panel
            _createGamePanel = new VerticalStackPanel()
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
            Desktop.Root = _window;
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

        // JoinServer
        private void Login()
        {
            _currentPlayerName = _playerNameInput.Text;
            if (string.IsNullOrEmpty(_currentPlayerName))
                return;

            JoinServerValues joinServerValues = new JoinServerValues("127.0.0.1",5005, "Paul");
            RurikMonoGame.Client.ClientIdentifier = _currentPlayerName;
            RurikMonoGame.Client.Connect(joinServerValues, "rurik");

            _isLoggedIn = true;
            _loginPanel.Visible = false;
            _gameListPanel.Visible = true;
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

        public void RefreshGameList()
        {
            if (RurikMonoGame.Client.Games == null)
                return;

            // Clear existing items
            _gameListView.Widgets.Clear();

            // Get list of games
            var games = RurikMonoGame.Client.Games.ListGames();
            
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

        private void StartServer()
        {
            ServerSettings serverSettings = new ServerSettings();
            RurikMonoGame.Server = new Server();
            RurikMonoGame.Server.StartAsHost(serverSettings, "rurik");
            _createServerButton.Visible = false;
        }

        private void CreateGame(string gameName, string color, string position)
        {
            if (string.IsNullOrEmpty(gameName) || RurikMonoGame.Client.ClientIdentifier == null)
            {
                return;
            }

            // TODO: make this dynamic
            CreateGameAction action = new(RurikMonoGame.Client.ClientIdentifier);
            action.CreateGameValues = new CreateGameValues("test", RurikMonoGame.Client.ClientIdentifier);

            RurikMonoGame.Client.SendAction(action);
            //var gameStatus = Games.GetInstance().CreateGame(gameName, _currentPlayerName, 4);
            // Join the newly created game
            //var game = Games.GetInstance().GetGameStatus(gameStatus.GameId);
            //if (game != null)
            //{
                // This would normally open a join game dialog or redirect to the game
                // For now, just log that a game was created and joined
                //System.Console.WriteLine($"Player {_currentPlayerName} created and joined game {gameName}");
            //}
            _createGameWindow.Close();
            //RefreshGameList();
        }

        private void OpenCreateGame()
        {
            _createGameWindow = new Window();
            _createGameWindow.Content = _createGamePanel;
            _createGameWindow.ShowModal(Desktop);
        }
    }
}
