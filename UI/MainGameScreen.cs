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
//using SharpDX.Direct3D11;

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
        
        private bool _isVisible = false;
        private GameStatus _game;
        private GameMap _gameMap;
        
        private readonly Desktop _desktop;
        private Texture2D? _mapTexture;
        
        private PlaceTroopsScreen? _placeTroopsModal;


        // Map location bounds (x, y, width, height) relative to an 800x600 reference image
        // The actual map texture is 1933x2544, so we need to scale from 800x600 to 1933x2544 first
        // Then scale to the panel size at runtime
        private const float ReferenceWidth = 1000f;
        private const float ReferenceHeight = 1000f;
        private const float TextureWidth = 1933f;
        private const float TextureHeight = 2544f;
        private static readonly float ScaleFromReferenceToTextureX = TextureWidth / ReferenceWidth;
        private static readonly float ScaleFromReferenceToTextureY = TextureHeight / ReferenceHeight;

        private Dictionary<string, Rectangle> _locationBounds = new Dictionary<string, Rectangle>
        {
            // Locations have been scaled to 0-1000.
            // Green locations (Novgorod region)
            {"Novgorod", new Rectangle(352,197,522,271)},
            {"Pskov", new Rectangle(155,175,287,248)},
            {"Polotsk", new Rectangle(252,367,370,437)},
            {"Smolensk", new Rectangle(499,366,632,448)},
            {"Rostov", new Rectangle(661,208,777,281)},
            {"Chernigov", new Rectangle(517,511,666,586)},
            {"Suzdal", new Rectangle(824,250,957,325)},
            {"Pereyaslavl", new Rectangle(591,608,758,687)},


            // Yellow locations (Kiev region)
            {"Volyn", new Rectangle(155,602,262,679)},
            {"Kiev", new Rectangle(316,554,429,632)},
            {"Galich", new Rectangle(240,783,343,851)},
            {"Murom", new Rectangle(813,473,952,552)},


            // Brown locations (Azov region)
            {"Brest", new Rectangle(60,424,180,504)},
            {"Peresech", new Rectangle(304,861,465,948)},
            {"Azov", new Rectangle(845, 708, 955, 794)}
        };


        public MainGameScreen(RurikMonoGame game, Desktop desktop)
        {
            _rurikMonoGame = game;
            _desktop = desktop;
            _window = _desktop.Root as Window;
            game.MainGameScreen = this;
            _placeTroopsModal = new PlaceTroopsScreen(game, desktop);
            Initialize();
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

            // Set up right grid rows: top takes 60%, bottom takes 40%
            rightGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            rightGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            // Create right top panel (for future use - player info, turn info, etc.)
            _rightTopPanel = new Panel()
            {
                Id = "rightTopPanel",
                Background = new SolidBrush(Color.Gray),
                Padding = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Create right bottom panel (for future use - action buttons, chat, etc.)
            _rightBottomPanel = new Panel()
            {
                Id = "rightBottomPanel",
                Background = new SolidBrush(Color.LightGray),
                Padding = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Add left panel to main grid
            Grid.SetColumn(_leftPanel, 0);
            Grid.SetRow(_leftPanel, 0);
            _mainGrid.Widgets.Add(_leftPanel);

            // Add top and bottom panels to right grid
            Grid.SetColumn(_rightTopPanel, 1);
            Grid.SetRow(_rightTopPanel, 0);
            _mainGrid.Widgets.Add(_rightTopPanel);

            Grid.SetColumn(_rightBottomPanel, 1);
            Grid.SetRow(_rightBottomPanel, 1);
            _mainGrid.Widgets.Add(_rightBottomPanel);

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

        public void Show()
        {
            //Globals.Log("MainGameScreen.Show(): enter, window=" + _window.Id + ", panel=" + _window.Content.Id);
            _isVisible = true;
            if (_window.Content != null)
            {
                _window.Content.RemoveFromParent();
            }
            _window.Content = null;
            _window.Content = _panel;
            _window.Title = "Rurik: Game";
            _rurikMonoGame.CurrentMyraScreen = "MainGameScreen";
            updateMapPanel();
            //Globals.Log("MainGameScreen.Show(): exit, window=" + _window.Id + ", panel=" + _window.Content.Id);
        }

        public void Hide()
        {
            _isVisible = false;
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

            Globals.Log("handleLeftClick(): monogame window=" + _rurikMonoGame.Window.Position.X + "," + _rurikMonoGame.Window.Position.Y);
            int myraWindowX = _window.ActualBounds.X; // + 8; // Adjust for window border width
            int myraWindowY = _window.ActualBounds.Y + 30; // Adjust for title bar height
            Globals.Log("handleLeftClick(): myra window=(" + myraWindowX+ "," + myraWindowY + "," + _window.ActualBounds.Width + "," + _window.ActualBounds.Height + ")");

            // Get the mouse position relative to the window
            Point mousePosition = new Point(mouseState.X, mouseState.Y);
            Globals.Log($"handleLeftClick(): mousePosition=({mouseState.X},{mouseState.Y})");

            // Calculate the position of the left panel relative to the window
            // The left panel is at column 0, row 0 of the main grid
            Point panelPosition = GetPanelPosition(_leftPanel);
            Globals.Log($"handleLeftClick(): panelPosition=({panelPosition.X},{panelPosition.Y})");

            // Calculate the mouse position relative to the left panel
            Point relativeMousePosition = new Point(mousePosition.X - myraWindowX - panelPosition.X, mousePosition.Y - myraWindowY - panelPosition.Y);

            // Log the mouse position for debugging
            Globals.Log($"handleLeftClick(): relativeMousePosition=({relativeMousePosition.X},{relativeMousePosition.Y})");

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
            Globals.Log($"handleLeftClick(): texture=({textureWidth}x{textureHeight}), panel=({panelWidth}x{panelHeight}), scaleFromReference=({scaleXFromReference:F4},{scaleYFromReference:F4}), scaleToPanel=({scaleX:F4},{scaleY:F4})");

            // Check which location was clicked
            foreach (var kvp in _locationBounds)
            {
                var locationName = kvp.Key;
                var bounds = kvp.Value;

                // First scale from 1000 reference to actual texture dimensions
                float textureX = bounds.X * scaleXFromReference;
                float textureY = bounds.Y * scaleYFromReference;
                // bounds.Width and bounds.Height are actual coordinates.
                int boundsWidth = bounds.Width - bounds.X;
                int boundsHeight = bounds.Height - bounds.Y;
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
                    Globals.Log($"handleLeftClick(): location='{locationName}', bounds=({scaledX},{scaledY},{scaledWidth},{scaledHeight})");
                    Globals.Log($"Clicked location: {locationName}");
                    return;
                }
            }

            Globals.Log("Clicked location: No location found");
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

        public void UpdateGameInfo(GameStatus game, GameMap map)
        {
            Globals.Log("MainGameScreen.UpdateGameInfo(): enter");
            _game = game;

            if (map != null)
            {
                _gameMap = map;
            }


            // Update game info labels
            if (game != null)
            {
                // Update left panel with map texture
                updateMapPanel();

                // Check if we should show the PlaceTroops modal
                // Show modal when game state is waitingForTroopPlacement and player is the current player
                if (game.CurrentState == "waitingForTroopPlacement" && game.ClientPlayer != null)
                {
                    // Check if the client player is the current player
                    if (game.CurrentPlayerColor == game.ClientPlayer.Color)
                    {
                        Globals.Log("MainGameScreen.UpdateGameInfo(): Showing PlaceTroops modal");
                        _placeTroopsModal?.Show();
                    }
                }
            }
        }

        private void updateMapPanel()
        {
            //Globals.Log("MainGameScreen.updateMapPanel(): enter");

            // Clear existing widgets in left panel
            _leftPanel.Widgets.Clear();

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
                    
                    // Log the location bounds with scaled coordinates
                    //logLocationBounds(_mapTexture.Width, _mapTexture.Height, leftPanelWidth, (int)_leftPanel.Height);

                }
                else
                {
                    Globals.Log("MainGameScreen.updateMapPanel(): map texture not found");
                }
            }
        }
    }
}
