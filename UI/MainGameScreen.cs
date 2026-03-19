using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using rurik;
using System.Runtime.CompilerServices;

namespace rurik.UI
{
    public class MainGameScreen : IGameScreen
    {
        private Window _window;
        private RurikMonoGame _rurikMonoGame;
        private Panel _panel;
        
        private Grid _mainGrid;
        private Panel _leftPanel;
        private Panel _rightPanel;
        private Panel _rightTopPanel;
        private Panel _rightBottomPanel;
        
        public bool IsVisible = false;
        private GameStatus _game;
        private GameMap _gameMap;
        
        private readonly Desktop _desktop;
        private Texture2D? _mapTexture;
        
        private PlaceTroopsModal? _placeTroopsModal;
        private PlaceLeaderModal? _placeLeaderModal;
        private PlayAdvisorModal? _playAdvisorModal;
        
        private AdvisorBoardPanel? _advisorBoardPanel;
        private AuctionBoard? _auctionBoard;
        private PlayerPanel? _playerPanel;
        private WindowPanel _mapWindowPanel;
        private WindowPanel _playerWindowPanel;
        private WindowPanel _advisorWindowPanel;

        private WindowPanel _gameStatusWindowPanel;

        // Color mapping for factions
        private static readonly Dictionary<string, Color> FactionColors = new Dictionary<string, Color>
        {
            {"red", Color.Red},
            {"blue", Color.Blue},
            {"white", Color.White},
            {"yellow", Color.Yellow}
        };

        // Map location bounds (x, y, width, height) relative to an 800x600 reference image
        // The actual map texture is 1933x2544, so we need to scale from 800x600 to 1933x2544 first
        // Then scale to the panel size at runtime
        private const float ReferenceWidth = 1000f;
        private const float ReferenceHeight = 1000f;
        private const float TextureWidth = 1933f;
        private const float TextureHeight = 2544f;
        private static readonly float ScaleFromReferenceToTextureX = TextureWidth / ReferenceWidth;
        private static readonly float ScaleFromReferenceToTextureY = TextureHeight / ReferenceHeight;

        private Dictionary<string, Vector2> _locationItemsCoordinates = new Dictionary<string, Vector2>
        {
            // Locations have been scaled to 0-1000.
            // Green locations (Novgorod region)
            {"Novgorod", new Vector2(280,100)},
            {"Pskov", new Vector2(80,175)},
            {"Polotsk", new Vector2(310,395)},
            {"Smolensk", new Vector2(460,250)},
            {"Rostov", new Vector2(660,75)},
            {"Chernigov", new Vector2(650,375)},
            {"Suzdal", new Vector2(820,100)},
            {"Pereyaslavl", new Vector2(705,525)},

            // Yellow locations (Kiev region)
            {"Volyn", new Vector2(155,450)},
            {"Kiev", new Vector2(390,565)},
            {"Galich", new Vector2(295,675)},
            {"Murom", new Vector2(850,340)},

            // Brown locations (Azov region)
            {"Brest", new Vector2(10,475)},
            {"Peresech", new Vector2(420,775)},
            {"Azov", new Vector2(700, 660)}
        };

        private Dictionary<string, Vector2> _locationResourceCoordinates = new Dictionary<string, Vector2>
        {
            // Locations have been scaled to 0-1000.
            // Green locations (Novgorod region)
            {"Novgorod", new Vector2(410, 215)},
            {"Pskov", new Vector2(200, 195)},
            {"Polotsk", new Vector2(280, 365)},
            {"Smolensk", new Vector2(540, 375)},
            {"Rostov", new Vector2(685, 230)},
            {"Chernigov", new Vector2(560, 500)},
            {"Suzdal", new Vector2(860, 265)},
            {"Pereyaslavl", new Vector2(640,590)},

            // Yellow locations (Kiev region)
            {"Volyn", new Vector2(185,585)},
            {"Kiev", new Vector2(345, 540)},
            {"Galich", new Vector2(235, 750)},
            {"Murom", new Vector2(840, 475)},

            // Brown locations (Azov region)
            {"Brest", new Vector2(85, 425)},
            {"Peresech", new Vector2(360,840)},
            {"Azov", new Vector2(855, 690)}
        };

        private Dictionary<string, Vector2> _rebelCoordinates = new Dictionary<string, Vector2>
        {
            // Locations have been scaled to 0-1000.
            // Green locations (Novgorod region)
            {"Novgorod", new Vector2(380, 215)},
            {"Pskov", new Vector2(200, 230)},
            {"Polotsk", new Vector2(250, 365)},
            {"Smolensk", new Vector2(505, 375)},
            {"Rostov", new Vector2(665, 230)},
            {"Chernigov", new Vector2(525, 500)},
            {"Suzdal", new Vector2(830, 265)},
            {"Pereyaslavl", new Vector2(610,590)},

            // Yellow locations (Kiev region)
            {"Volyn", new Vector2(235,585)},
            {"Kiev", new Vector2(310, 540)},
            {"Galich", new Vector2(200, 710)},
            {"Murom", new Vector2(840, 505)},

            // Brown locations (Azov region)
            {"Brest", new Vector2(50, 425)},
            {"Peresech", new Vector2(330,840)},
            {"Azov", new Vector2(825, 690)}
        };



        public MainGameScreen(RurikMonoGame game, Desktop desktop)
        {
            _rurikMonoGame = game;
            _desktop = desktop;
            _window = _desktop.Root as Window;
            game.MainGameScreen = this;
            Initialize();
      
            // Initialize the AdvisorBoardPanel
            _advisorBoardPanel = new AdvisorBoardPanel(desktop, new AuctionBoard(4), _rurikMonoGame.Textures);
            
        }

        public void Initialize()
        {
            // Main panel
            _panel = new Panel()
            {
                Id = "mainGameScreenPanel",
                Background = new SolidBrush(Color.Black),
                Width = _window.Width,
                Height = _window.Height,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Main grid to split left and right panels
            _mainGrid = new Grid()
            {
                Id = "mainGameScreenGrid",
                Background = new SolidBrush(Color.Black),
                Padding = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Set up grid columns: left panel (map) takes 60%, right panel takes 40%
            _mainGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _mainGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));

            // Set up grid rows for right panel (will be split into top/bottom)
            _mainGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            // Create left panel (map display)
            _leftPanel = new Panel()
            {
                Id = "leftMapPanel",
                Background = new SolidBrush(Color.Gray),
                Padding = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Create right panel (container for top and bottom panels)
            _rightPanel = new Panel()
            {
                Id = "rightPanel",
                Background = new SolidBrush(Color.DarkGray),
                Padding = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Right panel grid to split top and bottom
            Grid rightGrid = new Grid()
            {
                Id = "rightPanelGrid",
                Background = new SolidBrush(Color.Black),
                Padding = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Set up right grid rows: top takes 50%, bottom takes 50%
            rightGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            rightGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            // Create right top panel (for future use - player info, turn info, etc.)
            _rightTopPanel = new Panel()
            {
                Id = "rightTopPanel",
                Background = new SolidBrush(Color.Gray),
                Padding = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
            };

            // Create right bottom panel (for future use - action buttons, chat, etc.)
            _rightBottomPanel = new Panel()
            {
                Id = "rightBottomPanel",
                Background = new SolidBrush(Color.Gray),
                Padding = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Bottom,
            };

            // Add left panel to main grid
            //Grid.SetColumn(_leftPanel, 0);
            //Grid.SetRow(_leftPanel, 0);
            //_mainGrid.Widgets.Add(_leftPanel);

            Grid.SetColumn(_rightPanel, 1);
            Grid.SetRow(_rightPanel, 0);
            _mainGrid.Widgets.Add(_rightPanel);


            // Add top and bottom panels to right grid
            Grid.SetColumn(_rightTopPanel, 0);
            Grid.SetRow(_rightTopPanel, 0);
            _rightPanel.Widgets.Add(_rightTopPanel);

            Grid.SetColumn(_rightBottomPanel, 0);
            Grid.SetRow(_rightBottomPanel, 1);
            _rightPanel.Widgets.Add(_rightBottomPanel);

            // Add right panel to main grid
            //Grid.SetColumn(_rightPanel, 1);
            //_mainGrid.Widgets.Add(_rightPanel);

            // Add main grid to panel
            _panel.Widgets.Add(_mainGrid);
        }


        public void Update()
        {
            // Update logic if needed
        }

        public void Draw()
        {
            // Draw logic if needed
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Show()
        {
            //Globals.Log("MainGameScreen.Show(): enter, window=" + _window.Id + ", panel=" + _window.Content.Id);
            _rurikMonoGame.CurrentMyraScreen = "MainGameScreen";

            if (_window == null || _panel == null)
                return;

            if (!IsVisible)
            {
                IsVisible = true;
                if (_window != null && _window.Content != null)
                {
                    try
                    {
                        _window.Content.RemoveFromParent();
                    }
                    catch(Exception ex)
                    {
                        Globals.Log(ex.Message);
                    }
                }
                _window.Content = null;
                _window.Content = _panel;
                _window.Title = "Rurik: Dawn of Kyiv";
                _rurikMonoGame.CurrentMyraScreen = "MainGameScreen";
                updateMapPanel();
                if (_mapWindowPanel == null)
                {
                    _mapWindowPanel = new WindowPanel(_rurikMonoGame, _desktop, _leftPanel, "Map", 0, 0, 0, 0);
                    _mapWindowPanel.Show();
                }

            }
            //Globals.Log("MainGameScreen.Show(): exit, window=" + _window.Id + ", panel=" + _window.Content.Id);
        }

        public void Hide()
        {
            IsVisible = false;
            _window.Close();
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

        public void handleLeftClick(MouseState mouseState)
        {
            // Only handle clicks if left panel (map) has widgets
            if (_leftPanel.Widgets.Count == 0)
                return;

            //Globals.Log("handleLeftClick(): monogame window=" + _rurikMonoGame.Window.Position.X + "," + _rurikMonoGame.Window.Position.Y);
            int myraWindowX = _window.ActualBounds.X; // + 8; // Adjust for window border width
            int myraWindowY = _window.ActualBounds.Y + 30; // Adjust for title bar height
            //Globals.Log("handleLeftClick(): myra window=(" + myraWindowX+ "," + myraWindowY + "," + _window.ActualBounds.Width + "," + _window.ActualBounds.Height + ")");

            // Get the mouse position relative to the window
            Point mousePosition = new Point(mouseState.X, mouseState.Y);
            //Globals.Log($"handleLeftClick(): mousePosition=({mouseState.X},{mouseState.Y})");

            // Calculate the position of the left panel relative to the window
            // The left panel is at column 0, row 0 of the main grid
            Point panelPosition = GetPanelPosition(_leftPanel);
            //Globals.Log($"handleLeftClick(): panelPosition=({panelPosition.X},{panelPosition.Y})");

            // Calculate the mouse position relative to the left panel
            Point relativeMousePosition = new Point(mousePosition.X - myraWindowX - panelPosition.X, mousePosition.Y - myraWindowY - panelPosition.Y);

            // Log the mouse position for debugging
            //Globals.Log($"handleLeftClick(): relativeMousePosition=({relativeMousePosition.X},{relativeMousePosition.Y})");

            // Get the current map texture dimensions and panel dimensions for scaling
            if (_mapTexture == null)
                return;

            int textureWidth = _mapTexture.Width;
            int textureHeight = _mapTexture.Height;
            int panelWidth = _leftPanel.Width ?? 0;
            int panelHeight = _leftPanel.Height ?? 0;

            // Calculate scaling factors
            // First scale from 0-1000 reference to actual texture dimensions on panel
            float scaleXFromReference = (float)textureWidth / ReferenceWidth;
            //float scaleXFromReference = (float)textureWidth / panelWidth;
            float scaleYFromReference = (float)textureHeight / ReferenceHeight;
            //float scaleYFromReference = (float)textureHeight / panelHeight;
            // Then scale from texture to panel
            float scaleX = (float)panelWidth / (float)textureWidth;
            float scaleY = (float)panelHeight / (float)textureHeight;

            // Log scaling info for debugging
            //Globals.Log($"handleLeftClick(): texture=({textureWidth}x{textureHeight}), panel=({panelWidth}x{panelHeight}), scaleFromReference=({scaleXFromReference:F4},{scaleYFromReference:F4}), scaleToPanel=({scaleX:F4},{scaleY:F4})");

            // Check which location was clicked
            foreach (var kvp in _locationItemsCoordinates)
            {
                var locationName = kvp.Key;
                var bounds = kvp.Value;

                // First scale from 1000 reference to actual texture dimensions
                float textureX = bounds.X * scaleXFromReference;
                float textureY = bounds.Y * scaleYFromReference;
                // bounds.Width and bounds.Height are actual coordinates.

                int boundsWidth = 75; //bounds.Width - bounds.X;
                int boundsHeight = 100; //bounds.Height - bounds.Y;
                float textureWidthScaled = boundsWidth * scaleXFromReference;
                float textureHeightScaled = boundsHeight * scaleYFromReference;

                // Then scale from texture to panel
                int scaledX = (int)(textureX * scaleX);
                int scaledY = (int)(textureY * scaleY);
                int scaledWidth = (int)(textureWidthScaled * scaleX);
                int scaledHeight = (int)(textureHeightScaled * scaleY);

                // Check if mouse is within this location's bounds
                if (relativeMousePosition.X >= scaledX && relativeMousePosition.X <= scaledX + scaledWidth &&
                    relativeMousePosition.Y >= scaledY && relativeMousePosition.Y <= scaledY + scaledHeight)
                {
                    // Log bounds for debugging
                    //Globals.Log($"handleLeftClick(): location='{locationName}', bounds=({scaledX},{scaledY},{scaledWidth},{scaledHeight})");
                    Globals.Log($"Clicked location: {locationName}");
                    return;
                }
            }

            //Globals.Log("Clicked location: No location found");
        }

        private Point GetPanelPosition(Panel panel)
        {
            // Get the panel's position relative to the window
            // This is a simplified approach - in Myra, widgets have position properties
            if (_window.Content != null && _window.Content is Panel rootPanel)
            {
                // Traverse the widget tree to find the panel's position
                // For now, we'll assume the left panel is at the origin of the grid
                // which is at the origin of the window
                return new Point(0, 0);
            }
            return new Point(0, 0);
        }

        public void UpdateGameInfo(GameStatus game)
        {
            UpdateGameInfo(game, null);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateGameInfo(GameStatus game, GameMap map)
        {
            Globals.Log("MainGameScreen.UpdateGameInfo(): enter, game state=" + game.CurrentState + ", currentPlayer=" + game.CurrentPlayerColor);
            _game = game;

            if (map != null)
            {
                _gameMap = map;
            }


            // Update game info labels
            if (game != null)
            {
                // Update left panel with map texture and location items
                updateMapPanel();

                GameStatusPanel gameStatusPanel = null;
                if (_gameStatusWindowPanel == null)
                {
                    gameStatusPanel = new GameStatusPanel(_desktop, _game, _rurikMonoGame.Textures, _game.ClientPlayer, _game.Players);
                    _gameStatusWindowPanel = new WindowPanel(_rurikMonoGame, _desktop, gameStatusPanel, "Status", 0, 0, 700, 0);
                    _gameStatusWindowPanel.Show();
                }
                gameStatusPanel = (GameStatusPanel)_gameStatusWindowPanel.Panel;
                gameStatusPanel.UpdatePanel(_game, _game.ClientPlayer, _game.Players);
                


                // Check if we should show the PlaceTroops modal
                // Show modal when game state is waitingForTroopPlacement and player is the current player
                if (game.CurrentState == "waitingForTroopPlacement" && game.ClientPlayer != null)
                {
                    // Check if the client player is the current player
                    if (game.CurrentPlayerColor == game.ClientPlayer.Color)
                    {
                        Globals.Log("MainGameScreen.UpdateGameInfo(): Showing PlaceTroops modal");
                        _placeTroopsModal = new PlaceTroopsModal(_rurikMonoGame, _desktop);
                        if (!_placeTroopsModal.IsVisible)
                        {
                            _placeTroopsModal?.UpdateGameInfo(_game, _gameMap);
                            _placeTroopsModal?.Show();
                        }
                    }
                }

                // Check if we should show the PlaceLeader modal
                // Show modal when game state is waitingForLeaderPlacement and player is the current player
                if (game.CurrentState == "waitingForLeaderPlacement" && game.ClientPlayer != null)
                {
                    // Check if the client player is the current player
                    if (game.CurrentPlayerColor == game.ClientPlayer.Color)
                    {
                        Globals.Log("MainGameScreen.UpdateGameInfo(): Showing PlaceLeader modal");
                        _placeLeaderModal = new PlaceLeaderModal(_rurikMonoGame, _desktop);
                        if (!_placeLeaderModal.IsVisible)
                        {
                            _placeLeaderModal?.UpdateGameInfo(_game, _gameMap);
                            _placeLeaderModal?.Show();
                        }
                    }
                }
                
                // Check if we should show the PlayAdvisor modal
                // Show modal when game state is strategyPhase and player is the current player
                if (game.CurrentState == "strategyPhase" && game.ClientPlayer != null)
                {
                    // Check if the client player is the current player
                    if (game.CurrentPlayerColor == game.ClientPlayer.Color)
                    {
                        Globals.Log("MainGameScreen.UpdateGameInfo(): Showing PlayAdvisor modal");
                        _playAdvisorModal = new PlayAdvisorModal(_rurikMonoGame, _desktop);
                        if (!_playAdvisorModal.IsVisible)
                        {
                            _playAdvisorModal?.UpdateGameInfo(_game);
                            _playAdvisorModal?.Show();
                        }
                    }
                }
                else
                {
                    // Hide the PlayAdvisor modal if not in strategyPhase or player is not current player
                    if (_playAdvisorModal != null && _playAdvisorModal.IsVisible)
                    {
                        _playAdvisorModal?.Hide();
                    }
                }

        
                // Check if we should show the AdvisorBoardPanel
                // Show panel when game state is strategyPhase
                if ((game.CurrentState == "strategyPhase" || game.CurrentState.Equals("retrieveAdvisor")))
                {
                    Globals.Log("MainGameScreen.UpdateGameInfo(): Showing AdvisorBoardPanel");
                    if (_advisorBoardPanel != null)
                    {
                        // Update the auction board with the game's auction board data
                        if (game.AuctionBoard != null)
                        {
                            _auctionBoard = game.AuctionBoard;
                            // Update the advisor board panel with the current auction board
                            Globals.Log("UpdateGameInfo(): update auction board");
                            _advisorBoardPanel.SetAuctionBoard(game.AuctionBoard);
                            _advisorBoardPanel.UpdateBoard();
                        }
                        // Add the advisor board panel to the right top panel
                        //_rightTopPanel.Widgets.Clear();
                        //_rightTopPanel.Widgets.Add(_advisorBoardPanel);

                        if (_advisorWindowPanel == null)
                        {
                            _advisorWindowPanel = new WindowPanel(_rurikMonoGame, _desktop, _advisorBoardPanel, "Advisor Board", 0, 0, 600, 0);
                            _advisorWindowPanel.Show();
                        }
                    }
                }
                else
                {
                    // Clear the right top panel if not in strategyPhase
                    _rightTopPanel.Widgets.Clear();
                }

                if (_playerPanel == null && game.ClientPlayer != null)
                {
                    // Initialize the PlayerPanel
                    var player = game.ClientPlayer;
                    _playerPanel = new PlayerPanel(_desktop, player, _rurikMonoGame.Textures);
                }

                // Update PlayerPanel with the client player's data
                if (_playerPanel != null && game.ClientPlayer != null)
                {
                    _playerPanel.SetPlayer(game.ClientPlayer);
                    if (_playerWindowPanel == null)
                    {
                        _playerWindowPanel = new WindowPanel(_rurikMonoGame, _desktop, _playerPanel, "Player Supply and Boat", 0, 0, 600, 300);
                        _playerWindowPanel.Show();
                    }
                    //_rightBottomPanel.Widgets.Clear();
                    //_rightBottomPanel.Widgets.Add(_playerPanel);
                }

            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void updateMapPanel()
        {
            //Globals.Log("MainGameScreen.updateMapPanel(): enter");

            // Clear existing widgets in left panel
            if (_leftPanel == null)
                return;

            // Add map texture display
            if (_rurikMonoGame.Textures != null)
            {
                //Globals.Log("MainGameScreen.updateMapPanel(): got textures");
                _mapTexture = _rurikMonoGame.Textures.GetTexture("map");

                if (_mapTexture != null)
                {
                    //Globals.Log("MainGameScreen.updateMapPanel(): got map texture");
                    // Create an Image widget to display the map texture
                    // Using TextureRegion from MonoGame to wrap the Texture2D
                    var textureRegion = new Myra.Graphics2D.TextureAtlases.TextureRegion(_mapTexture);
                    var terrainImage = new Image();
                    terrainImage.Renderable = textureRegion;
      
                    _leftPanel.Widgets.Add(terrainImage);

                    _leftPanel.Height = _window.Height;
                    float ratio = (float)((float)_window.Height / (float)_mapTexture.Height);
                    int leftPanelWidth = (int)((float)_mapTexture.Width * ratio);
                    _leftPanel.Width = leftPanelWidth;
                  
                    // Draw troop and building overlays for the location
                    drawLocationOverlays();
                }
                else
                {
                    Globals.Log("MainGameScreen.updateMapPanel(): map texture not found");
                }
            }

            // Draw resource overlays for locations with resources
            drawLocationResources();

            // Draw rebel reward indicators for locations with rebels
            drawRebelRewards();
        }

        private void drawLocationResources()
        {
            //Globals.Log("drawLocationResources(): enter");
            if (_gameMap == null || _mapTexture == null)
                return;

            //Globals.Log("drawLocationResources(): leftPanel=" + _leftPanel);
            // Get panel dimensions
            int panelWidth = _leftPanel.Width ?? 0;
            int panelHeight = _leftPanel.Height ?? 0;

            // Calculate scaling factors from reference (0-1000) to panel
            float scaleXFromReference = (float)_mapTexture.Width / ReferenceWidth;
            float scaleYFromReference = (float)_mapTexture.Height / ReferenceHeight;
            float scaleX = (float)panelWidth / (float)_mapTexture.Width;
            float scaleY = (float)panelHeight / (float)_mapTexture.Height;

            // Draw resource overlays for each location that has resources
            foreach (var location in _gameMap.LocationsForGame)
            {
                //Globals.Log("drawLocationResources(): location=" + location.name + ", resourceCount=" + location.resourceCount);
                if (location.resourceCount > 0 && !string.IsNullOrEmpty(location.defaultResource))
                {
                    // Get resource coordinates
                    if (_locationResourceCoordinates.TryGetValue(location.name, out Vector2 coords))
                    {
                        // Scale coordinates to panel coordinates
                        float textureX = coords.X * scaleXFromReference;
                        float textureY = coords.Y * scaleYFromReference;

                        int resourceX = (int)(textureX * scaleX);
                        int resourceY = (int)(textureY * scaleY);

                        // Get the resource texture from TextureMap
                        if (_rurikMonoGame.Textures.TextureMap.TryGetValue(location.defaultResource, out Texture2D? resourceTexture))
                        {
                            //Globals.Log($"drawLocationResources(): drawing {location.defaultResource} at ({resourceX},{resourceY})");

                            // Create an Image widget to display the resource texture
                            var textureRegion = new Myra.Graphics2D.TextureAtlases.TextureRegion(resourceTexture);
                            var resourceImage = new Image();
                            resourceImage.Renderable = textureRegion;
                            resourceImage.Width = 30;  // Set a reasonable size for resource icons
                            resourceImage.Height = 30;

                            resourceImage.Left = resourceX;
                            resourceImage.Top = resourceY;

                            try
                            {
                                _leftPanel.Widgets.Add(resourceImage);
                            }
                            catch (Exception ex)
                            {
                                // Ignore if already added
                            }
                        }
                    }
                }
            }
        }

        private void drawLocationOverlays()
        {
            //Globals.Log("drawTroopOverlays(): enter");
            //Globals.Log("drawTroopOverlays(): gameMap=" + _gameMap);
            //Globals.Log("drawTroopOverlays(): _mapTexture=" + _mapTexture);
            if (_gameMap == null || _mapTexture == null)
                return;

            //Globals.Log("drawTroopOverlays(): leftPanel=" + _leftPanel);
            // Get panel dimensions
            int panelWidth = _leftPanel.Width ?? 0;
            int panelHeight = _leftPanel.Height ?? 0;

            // Calculate scaling factors from reference (0-1000) to panel
            float scaleXFromReference = (float)_mapTexture.Width / ReferenceWidth;
            float scaleYFromReference = (float)_mapTexture.Height / ReferenceHeight;
            float scaleX = (float)panelWidth / (float)_mapTexture.Width;
            float scaleY = (float)panelHeight / (float)_mapTexture.Height;
 
            // Draw overlays for each location that has troops
            foreach (var location in _gameMap.LocationsForGame)
            {
                //Globals.Log("drawTroopOverlays(): location=" + location.name);
                foreach (var kvp in location.troopsByColor)
                {
                    string color = kvp.Key;
                    int troopCount = kvp.Value;
                    int leaderCount = location.leaderByColor[color];
                    //Globals.Log("drawTroopOverlays(): color=" + color + "count=" + troopCount);

                    // remove this
                    //leaderCount = 1;
                    //troopCount = 1;

                    if (troopCount > 0 || leaderCount > 0)
                    {
                        // Get location bounds
                        if (_locationItemsCoordinates.TryGetValue(location.name, out Vector2 coordinates))
                        {
                            // Scale bounds to panel coordinates
                            float textureX = coordinates.X * scaleXFromReference;
                            float textureY = coordinates.Y * scaleYFromReference;

                            int overlayX = (int)(textureX * scaleX);
                            int overlayY = (int)(textureY * scaleY);

                            LocationItemsPanel locationItemsPanel = new LocationItemsPanel(_rurikMonoGame, location);
                            locationItemsPanel.Panel.Left = overlayX;
                            locationItemsPanel.Panel.Top = overlayY;
                            try
                            {
                                _leftPanel.Widgets.Add(locationItemsPanel.Panel);    
                            }
                            catch(Exception ex) {}

                        }
                    }

                }
            }
        }

        private void drawRebelRewards()
        {
            //Globals.Log("drawRebelRewards(): enter");
            if (_gameMap == null || _mapTexture == null)
                return;

            //Globals.Log("drawRebelRewards(): leftPanel=" + _leftPanel);
            // Get panel dimensions
            int panelWidth = _leftPanel.Width ?? 0;
            int panelHeight = _leftPanel.Height ?? 0;

            // Calculate scaling factors from reference (0-1000) to panel
            float scaleXFromReference = (float)_mapTexture.Width / ReferenceWidth;
            float scaleYFromReference = (float)_mapTexture.Height / ReferenceHeight;
            float scaleX = (float)panelWidth / (float)_mapTexture.Width;
            float scaleY = (float)panelHeight / (float)_mapTexture.Height;

            // Draw rebel reward indicators for each location that has rebels
            foreach (var location in _gameMap.LocationsForGame)
            {
                //Globals.Log("drawRebelRewards(): location=" + location.name + ", rebelRewards.Count=" + location.rebelRewards.Count);
                if (location.rebelRewards.Count > 0)
                {
                    // Get rebel coordinates
                    if (_rebelCoordinates.TryGetValue(location.name, out Vector2 coords))
                    {
                        // Scale coordinates to panel coordinates
                        float textureX = coords.X * scaleXFromReference;
                        float textureY = coords.Y * scaleYFromReference;

                        int rebelX = (int)(textureX * scaleX);
                        int rebelY = (int)(textureY * scaleY);

                        // Draw a black square with white count text
                        int squareSize = 20;

                        // Create a panel for the black square background
                        var rebelPanel = new Panel()
                        {
                            Width = squareSize,
                            Height = squareSize,
                            Left = rebelX,
                            Top = rebelY,
                            Background = new SolidBrush(Color.Black),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top
                        };

                        // Add count label inside the panel
                        var countLabel = new Label()
                        {
                            Text = location.rebelRewards.Count.ToString(),
                            TextColor = Color.White,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                        };
                        rebelPanel.Widgets.Add(countLabel);

                        try
                        {
                            _leftPanel.Widgets.Add(rebelPanel);
                        }
                        catch (Exception ex)
                        {
                            // Ignore if already added
                        }
                    }
                }
            }
        }
    }
}
