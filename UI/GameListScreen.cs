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
        private Label _portLabel;
        private TextBox _portInput;
        private TextBox _hostInput;
        private TextBox _gameNameInput;
        private ComboView _playerColorSelect;
        private ComboView _playerPositionSelect;
        private Label _numberOfPlayersLabel;
        private TextBox _numberOfPlayersInput;

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

            string currentUser = Environment.UserName;
            _playerNameInput = new TextBox()
            {
                Id = "playerNameInput",
                Width = 200,
                Height = 30,
                Text = currentUser,
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

            _hostInput = new TextBox()
            {
                Id = "hostInput",
                Width = 200,
                Height = 30,
                Text = "127.0.0.1",
                Border = new SolidBrush("#808000FF"),
                BorderThickness = new Thickness(2),
            };
            hostPanel.Widgets.Add(hostLabel);
            hostPanel.Widgets.Add(_hostInput);

            HorizontalStackPanel portPanel = new HorizontalStackPanel();
            _portLabel = new Label()
            {
                Id = "portLabel",
                Text = "port:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _portInput = new TextBox()
            {
                Id = "portInput",
                Width = 200,
                Height = 30,
                Text = "5005",
                Border = new SolidBrush("#808000FF"),
                BorderThickness = new Thickness(2),
            };
            portPanel.Widgets.Add(_portLabel);
            portPanel.Widgets.Add(_portInput);

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

            _gameNameInput = new TextBox()
            {
                Id = "gameNameInput",
                Width = 200,
                Height = 30,
                Text = "",
            };

            _numberOfPlayersLabel = new Label()
            {
                Id = "numberOfPlayersLabel",
                Text = "Number of Players:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _numberOfPlayersInput = new TextBox()
            {
                Id = "numberOfPlayersInput",
                Width = 200,
                Height = 30,
                Text = "4",
            };



            var playerColorLabel = new Label()
            {
                Id = "playerColorLabel",
                Text = "Player Color:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _playerColorSelect = new ComboView()
            {
                Id = "playerColorSelect",
                Width = 100,
                Height = 30,
            };

            _playerColorSelect.Widgets.Add(new Label() {Text="blue"});
            _playerColorSelect.Widgets.Add(new Label() {Text="red"});
            _playerColorSelect.Widgets.Add(new Label() {Text="white"});
            _playerColorSelect.Widgets.Add(new Label() {Text="yellow"});
            _playerColorSelect.SelectedIndex = 0;

            var playerPositionLabel = new Label()
            {
                Id = "playerPositionLabel",
                Text = "Player Position:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _playerPositionSelect = new ComboView()
            {
                Id = "playerPositionSelect",
                Width = 100,
                Height = 30,
            };

            _playerPositionSelect.Widgets.Add(new Label() {Text="N"});
            _playerPositionSelect.Widgets.Add(new Label() {Text="E"});
            _playerPositionSelect.Widgets.Add(new Label() {Text="S"});
            _playerPositionSelect.Widgets.Add(new Label() {Text="W"});
            _playerPositionSelect.SelectedIndex = 0;

            _createGameButton = new Button()
            {
                Id = "createGameButton",
                Width = 120,
                Height = 30,
            };
            _createGameButton.Content = new Label() {Text="Create Game"};

            _createGameButton.Click += (s, a) => CreateGame(_gameNameInput.Text, 
                ((Label)_playerColorSelect.SelectedItem).Text, 
                ((Label)_playerPositionSelect.SelectedItem).Text);

            _createGamePanel.Widgets.Add(createGameLabel);
            _createGamePanel.Widgets.Add(gameNameLabel);
            _createGamePanel.Widgets.Add(_gameNameInput);
            _createGamePanel.Widgets.Add(_numberOfPlayersLabel);
            _createGamePanel.Widgets.Add(_numberOfPlayersInput);
            _createGamePanel.Widgets.Add(playerColorLabel);
            _createGamePanel.Widgets.Add(_playerColorSelect);
            _createGamePanel.Widgets.Add(playerPositionLabel);
            _createGamePanel.Widgets.Add(_playerPositionSelect);
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

        // JoinServer()
        private void Login()
        {
            _currentPlayerName = _playerNameInput.Text;
            if (string.IsNullOrEmpty(_currentPlayerName))
                return;

            int port = validateTextBoxInteger(_portLabel.Text, _portInput, 1024, 49151);
            JoinServerValues joinServerValues = new JoinServerValues(_hostInput.Text, port, _playerNameInput.Text);
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

        // CreateServer()
        private void StartServer()
        {
            ServerSettings serverSettings = new ServerSettings();
            serverSettings.Port = validateTextBoxInteger(_portLabel.Text, _portInput, 1024, 49151);
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

            CreateGameAction action = new(RurikMonoGame.Client.ClientIdentifier);
            CreateGameValues gameValues = new CreateGameValues(_gameNameInput.Text, RurikMonoGame.Client.ClientIdentifier);
            int numberOfPlayers = validateTextBoxInteger(_numberOfPlayersLabel.Text, _numberOfPlayersInput, 1, 4);
            gameValues.NumberOfPlayers = numberOfPlayers;
            action.CreateGameValues = gameValues;
            RurikMonoGame.Client.SendAction(action);
            Globals.Log("CreateGame(): exit");
        }

        public void JoinGameAfterGameCreated(GameStatus gameStatus)
        {
            Globals.Log("JoinGameAfterGameCreated(): enter");
            JoinGame(gameStatus.Id);
            _createGameWindow.Close();
        }

        private void JoinGame(string gameId)
        {
            Globals.Log("JoinGame(): gameId=" + gameId);
            JoinGameAction action = new(RurikMonoGame.Client.ClientIdentifier);
            string playerColor = ((Label)_playerColorSelect.SelectedItem).Text;
            string playerPosition = ((Label)_playerPositionSelect.SelectedItem).Text;
            action.JoinGameValues = new JoinGameValues(gameId, _playerNameInput.Text, playerColor, playerPosition);
            RurikMonoGame.Client.SendAction(action);
        }

        private void OpenCreateGame()
        {
            _createGameWindow = new Window();
            _createGameWindow.Content = _createGamePanel;
            _createGameWindow.ShowModal(Desktop);
        }


        private int validateTextBoxInteger(string fieldName, TextBox textBox, int min, int max)
        {
            int number = 0;
            bool isValid = true;
            try
            {
                number = (Int32.Parse(textBox.Text));
                if (number < min || number > max)
                {
                    isValid = false;
                    showMessage(fieldName + " must have a value between " + min + " and " + max + ".");
                }
            }
            catch (Exception e)
            {
                isValid = false;
                showMessage("Could not parse " + fieldName + ".");
            }
            if (!isValid)
            {
                throw new Exception(fieldName + " was not valid.");
            }
            return number;
        }

        private void showMessage(string message)
        {
            Window window = new Window
            {
                Title = message
            };
            window.ShowModal(Desktop);
        }

    }


}
