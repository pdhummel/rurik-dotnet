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
        private Window _window;
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
        private string _currentPlayerName;
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
        private string _currentGameId;
        private string _currentGameName;
        private string _currentGameOwner;
        private int _currentNumberOfPlayers;
        private int _targetNumberOfPlayers;
        private string _currentGameState;

        private readonly Desktop Desktop;

        public GameListScreen(RurikMonoGame game, Desktop desktop)
        {
            RurikMonoGame = game;
            Desktop = desktop;
            _window = desktop.Root as Window;
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


            // Main panel
            _panel = new Panel()
            {
                Id = "mainGameListPanel",
                Background = new SolidBrush(Color.Black),
                Width = _window.Width,
                Height = _window.Height,
                //HorizontalAlignment = HorizontalAlignment.Center,
                //VerticalAlignment = VerticalAlignment.Center,
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

        }

        private string GetUniquePlayerName(string playerName)
        {
            // Check if the player name already exists in any game and append timestamp if needed
            if (RurikMonoGame.Client.Games != null)
            {
                //Globals.Log("GetUniquePlayerName(): checking for existing player name: " + playerName);
                var games = RurikMonoGame.Client.Games.ListGames();
                foreach (var game in games)
                {
                    //Globals.Log("GetUniquePlayerName(): checking game: " + game.GameName);
                    if (game.Players?.playersByName != null && game.Players.playersByName.ContainsKey(playerName))
                    {
                        // Name exists, append timestamp
                        return playerName + "_" + DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    }
                }
            }
            return playerName;
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
                Text = "name:",
                Width = 125,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            string currentUser = Environment.UserName;
            
            // Check if the player name already exists in any game and append timestamp if needed
            //string uniquePlayerName = GetUniquePlayerName(currentUser);
            string uniquePlayerName = currentUser;
        
            _playerNameInput = new TextBox()
            {
                Id = "playerNameInput",
                Width = 200,
                Height = 30,
                Text = uniquePlayerName,
                Border = new SolidBrush("#808000FF"),
                BorderThickness = new Thickness(2),
            };
            namePanel.Widgets.Add(loginLabel);
            namePanel.Widgets.Add(_playerNameInput);

            HorizontalStackPanel hostPanel = new HorizontalStackPanel();
            var hostLabel = new Label()
            {
                Id = "hostLabel",
                Width = 125,
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
                Width = 125,
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

            // Helper method to add a row with label on the right and input on the left
            void addPanelRow(VerticalStackPanel parentPanel, Label label, Widget widget)
            {
                var panel = new HorizontalStackPanel();
                panel.Width = 375;
                panel.MaxWidth = 375;
                label.Width = 200;
                parentPanel.Widgets.Add(panel);
                label.HorizontalAlignment = HorizontalAlignment.Right;
                panel.Widgets.Add(label);
                label.Visible = true;
                widget.HorizontalAlignment = HorizontalAlignment.Left;
                widget.Border = new SolidBrush("#808000FF");
                widget.BorderThickness = new Thickness(2);
                panel.Widgets.Add(widget);
                widget.Visible = true;
            }

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
                Text = "2",
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

            //_createGamePanel.Widgets.Add(createGameLabel);
            addPanelRow(_createGamePanel, gameNameLabel, _gameNameInput);
            addPanelRow(_createGamePanel, _numberOfPlayersLabel, _numberOfPlayersInput);
            addPanelRow(_createGamePanel, playerColorLabel, _playerColorSelect);
            addPanelRow(_createGamePanel, playerPositionLabel, _playerPositionSelect);
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
            //Globals.Log("GameListScreen.Show(): enter");
            _isVisible = true;
            _window.Title = "Rurik: Dawn of Kyiv";
            _window.Content = _panel;
            // Add to desktop or parent container
            //Desktop.Root = _window;
            //desktop.Widgets.Add(_panel);
        }

        public void Hide()
        {
            _isVisible = false;
            
            // Remove from desktop or parent container
            //_window.RemoveFromParent();
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
            _gameNameInput.Text = _currentPlayerName + "'s Game";
            // Add the player name label to the grid
            _grid.Widgets.Add(_playerNameLabel);
            RefreshGameList();
        }

        public void RefreshGameList()
        {
            Globals.Log("RefreshGameList(): enter");
            if (RurikMonoGame.Client.Games == null)
                return;
            if (Desktop.Root != _window)
                return;


            // Clear existing items
            _gameListView.Widgets.Clear();

            // Get list of games
            var games = RurikMonoGame.Client.Games.ListGames();
            
            // Add games to list view with grid layout
            foreach (var game in games)
            {
                // Create a grid for each game row
                var gameRowGrid = new Grid()
                {
                    Background = new SolidBrush(Color.Gray),
                    Padding = new Thickness(5),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    ShowGridLines = true,
                    ColumnSpacing = 8,
                    RowSpacing = 8,
                };
                
                // Set up grid columns - 5 columns for: Game, Owner, Players, State, Actions
                gameRowGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
                gameRowGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
                gameRowGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
                gameRowGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
                gameRowGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
                
                // Set up grid rows - 2 rows: header row and data row
                gameRowGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Header row
                gameRowGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Data row
                
                // Header row
                var gameHeaderLabel = new Label()
                {
                    Text = "Game",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = new SolidBrush(Color.DarkGray),
                };
                
                var ownerHeaderLabel = new Label()
                {
                    Text = "Owner",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = new SolidBrush(Color.DarkGray),
                };
                
                var playersHeaderLabel = new Label()
                {
                    Text = "Players",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = new SolidBrush(Color.DarkGray),
                };
                
                var stateHeaderLabel = new Label()
                {
                    Text = "State",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = new SolidBrush(Color.DarkGray),
                };
                
                var actionsHeaderLabel = new Label()
                {
                    Text = "Actions",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = new SolidBrush(Color.DarkGray),
                };
                
                // Add header widgets to grid
                gameRowGrid.Widgets.Add(gameHeaderLabel);
                Grid.SetColumn(gameHeaderLabel, 0);
                Grid.SetRow(gameHeaderLabel, 0);
                
                gameRowGrid.Widgets.Add(ownerHeaderLabel);
                Grid.SetColumn(ownerHeaderLabel, 1);
                Grid.SetRow(ownerHeaderLabel, 0);
                
                gameRowGrid.Widgets.Add(playersHeaderLabel);
                Grid.SetColumn(playersHeaderLabel, 2);
                Grid.SetRow(playersHeaderLabel, 0);
                
                gameRowGrid.Widgets.Add(stateHeaderLabel);
                Grid.SetColumn(stateHeaderLabel, 3);
                Grid.SetRow(stateHeaderLabel, 0);
                
                gameRowGrid.Widgets.Add(actionsHeaderLabel);
                Grid.SetColumn(actionsHeaderLabel, 4);
                Grid.SetRow(actionsHeaderLabel, 0);
                
                // Data row
                // Game info labels
                var gameNameLabel = new Label()
                {
                    Text = $"{game.GameName}",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                
                var ownerLabel = new Label()
                {
                    Text = $"{game.Owner}",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                
                var playersLabel = new Label()
                {
                    Text = $"{game.NumberOfPlayers}/{game.TargetNumberOfPlayers}",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                
                var stateLabel = new Label()
                {
                    Text = $"{game.CurrentState}",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                
                // Join button (only show when game is waitingForPlayers and not full)
                Button joinButton = null;
                if (game.CurrentState == "waitingForPlayers" && game.NumberOfPlayers < game.TargetNumberOfPlayers)
                {
                    joinButton = new Button()
                    {
                        Width = 100,
                        Height = 30,
                        Border = new SolidBrush("#808000FF"),
                        BorderThickness = new Thickness(2)
                    };
                    joinButton.Content = new Label
                    {
                        Text = "Join",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    // Capture current values to avoid closure issues
                    var currentPlayerName = _currentPlayerName;
                    var selectedColor = ((Label)_playerColorSelect.SelectedItem).Text;
                    var selectedPosition = ((Label)_playerPositionSelect.SelectedItem).Text;
                    joinButton.Click += (s, a) => OpenJoinGame(game.GameId);
                }
                
                // Add data widgets to grid row
                gameRowGrid.Widgets.Add(gameNameLabel);
                Grid.SetColumn(gameNameLabel, 0);
                Grid.SetRow(gameNameLabel, 1);
                
                gameRowGrid.Widgets.Add(ownerLabel);
                Grid.SetColumn(ownerLabel, 1);
                Grid.SetRow(ownerLabel, 1);
                
                gameRowGrid.Widgets.Add(playersLabel);
                Grid.SetColumn(playersLabel, 2);
                Grid.SetRow(playersLabel, 1);
                
                gameRowGrid.Widgets.Add(stateLabel);
                Grid.SetColumn(stateLabel, 3);
                Grid.SetRow(stateLabel, 1);
                
                if (joinButton != null)
                {
                    gameRowGrid.Widgets.Add(joinButton);
                    Grid.SetColumn(joinButton, 4);
                    Grid.SetRow(joinButton, 1);
                }
                
                _gameListView.Widgets.Add(gameRowGrid);
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
            
            string playerName = _playerNameInput.Text;
            string playerColor = ((Label)_playerColorSelect.SelectedItem).Text;
            string playerPosition = ((Label)_playerPositionSelect.SelectedItem).Text;
            JoinGameValues joinGameValues = new JoinGameValues(gameStatus.Id, playerName, playerColor, playerPosition);
            JoinGame(gameStatus.Id, joinGameValues);
            if (_createGameWindow != null)
                _createGameWindow.Close();
        }

        private void JoinGame(string gameId, JoinGameValues joinGameValues)
        {
            Globals.Log("JoinGame(): gameId=" + gameId);
            JoinGameAction action = new(RurikMonoGame.Client.ClientIdentifier);
            action.JoinGameValues = joinGameValues;
            RurikMonoGame.Client.SendAction(action);
        }

        private void OpenCreateGame()
        {
            _createGameWindow = new Window();
            _createGameWindow.Title = "Create New Game";
            _createGameWindow.Content = _createGamePanel;
            _createGameWindow.ShowModal(Desktop);
        }

        private void OpenJoinGame(string gameId)
        {
            // Get the game status to check which colors/positions are already taken
            GameStatus gameStatus = null;
            try
            {
                if (RurikMonoGame.Client.Games != null && RurikMonoGame.Client.Games.GameIdToGameStatus.ContainsKey(gameId))
                {
                    gameStatus = RurikMonoGame.Client.Games.GameIdToGameStatus[gameId];
                }
            }
            catch (Exception)
            {
                // If we can't get the game status, we'll show all options
            }

            // Determine which colors and positions are already taken
            var takenColors = new List<string>();
            var takenPositions = new List<string>();
        
            if (gameStatus?.Players != null)
            {
                takenColors = gameStatus.Players.playersByColor.Keys.ToList();
                takenPositions = gameStatus.Players.playersByPosition.Keys.ToList();
            }

            // All available options
            var allColors = new List<string> { "blue", "red", "white", "yellow" };
            var allPositions = new List<string> { "N", "E", "S", "W" };

            // Filter out taken options
            var availableColors = allColors.Where(c => !takenColors.Contains(c)).ToList();
            var availablePositions = allPositions.Where(p => !takenPositions.Contains(p)).ToList();

            // Create a new window for joining a game
            Window joinGameWindow = new Window();
            VerticalStackPanel joinGamePanel = new VerticalStackPanel()
            {
                Id = "joinGamePanel",
                Background = new SolidBrush(Color.Gray),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Game ID label (read-only)
            Label gameIdLabel = new Label()
            {
                Id = "gameIdLabel",
                Text = "Game ID:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Label gameIdValueLabel = new Label()
            {
                Id = "gameIdValueLabel",
                Text = gameId, // Set the game ID from parameter
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Game Name label (read-only)
            Label gameNameLabel = new Label()
            {
                Id = "gameNameLabel",
                Text = "Game Name:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Label gameNameValueLabel = new Label()
            {
                Id = "gameNameValueLabel",
                Text = "", // This will be populated with the actual game name
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _currentPlayerName = GetUniquePlayerName(_currentPlayerName); // Ensure the player name is unique across all games
            // Player Name label (read-only)
            Label playerNameLabel = new Label()
            {
                Id = "playerNameLabel",
                Text = "Player Name:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Label playerNameValueLabel = new Label()
            {
                Id = "playerNameValueLabel",
                Text = _currentPlayerName, // Set the player name from the current player
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Player Color label and combo box (similar to create game panel)
            Label playerColorLabel = new Label()
            {
                Id = "playerColorLabel",
                Text = "Player Color:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            ComboView playerColorSelect = new ComboView()
            {
                Id = "playerColorSelect",
                Width = 100,
                Height = 30,
            };

            foreach (var color in availableColors)
            {
                playerColorSelect.Widgets.Add(new Label() { Text = color });
            }
            if (playerColorSelect.Widgets.Count > 0)
            {
                playerColorSelect.SelectedIndex = 0;
            }

            // Player Position label and combo box (similar to create game panel)
            Label playerPositionLabel = new Label()
            {
                Id = "playerPositionLabel",
                Text = "Player Position:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            ComboView playerPositionSelect = new ComboView()
            {
                Id = "playerPositionSelect",
                Width = 100,
                Height = 30,
            };

            foreach (var position in availablePositions)
            {
                playerPositionSelect.Widgets.Add(new Label() { Text = position });
            }
            if (playerPositionSelect.Widgets.Count > 0)
            {
                playerPositionSelect.SelectedIndex = 0;
            }

            // Join button
            Button joinButton = new Button()
            {
                Id = "joinButton",
                Width = 120,
                Height = 30,
            };
            joinButton.Content = new Label() { Text = "Join Game" };

            // Set up the panel with all elements
            joinGamePanel.Widgets.Add(gameIdLabel);
            joinGamePanel.Widgets.Add(gameIdValueLabel);
            joinGamePanel.Widgets.Add(gameNameLabel);
            joinGamePanel.Widgets.Add(gameNameValueLabel);
            joinGamePanel.Widgets.Add(playerNameLabel);
            joinGamePanel.Widgets.Add(playerNameValueLabel);
            joinGamePanel.Widgets.Add(playerColorLabel);
            joinGamePanel.Widgets.Add(playerColorSelect);
            joinGamePanel.Widgets.Add(playerPositionLabel);
            joinGamePanel.Widgets.Add(playerPositionSelect);
            joinGamePanel.Widgets.Add(joinButton);

            // Set up button click event - this would need to be connected to a selected game
            joinButton.Click += (s, a) =>
            {
                string playerName = _playerNameInput.Text; // Get player name from login panel (already unique)
                playerName = _currentPlayerName;
                RurikMonoGame.Client.ClientIdentifier = playerName; // Set the client identifier to the unique player name
                string playerColor = ((Label)playerColorSelect.SelectedItem).Text; // Get color from join panel
                string playerPosition = ((Label)playerPositionSelect.SelectedItem).Text; // Get position from join panel
           
                JoinGameValues joinGameValues = new JoinGameValues(gameId, playerName, playerColor, playerPosition);
                JoinGame(gameId, joinGameValues);
                joinGameWindow.Close(); // Close the modal window after joining
            };

            // Try to get the game name from the client's game data
            try
            {
                if (RurikMonoGame.Client.Games != null && RurikMonoGame.Client.Games.GameIdToGameStatus.ContainsKey(gameId))
                {
                    var status = RurikMonoGame.Client.Games.GameIdToGameStatus[gameId];
                    gameNameValueLabel.Text = status.GameName;
                }
            }
            catch (Exception)
            {
                // If we can't get the game name, leave it blank
                gameNameValueLabel.Text = "Unknown Game";
            }

            joinGameWindow.Content = joinGamePanel;
            joinGameWindow.ShowModal(Desktop);
        }

        public void OpenGameSetup(GameStatus gameStatus)
        {
            Globals.Log("OpenGameSetup(): enter, window=" + _window.Id + ", panel=" + _window.Content.Id);
            if (gameStatus == null)
            {
                return;
            }
            Globals.Log("Opening Game Setup for game ID: " + gameStatus.GameId);
            _window.Content.RemoveFromParent();
            // Create the GameSetup screen
            //var gameSetup = new GameSetup(RurikMonoGame, Desktop);
            var gameSetup = RurikMonoGame.GameSetup;
            // Populate the GameSetup screen with the game data
            gameSetup.UpdateGameInfo(gameStatus);
            gameSetup.Show();
            this.Hide();
    
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
