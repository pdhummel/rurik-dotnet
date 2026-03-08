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
        
        private readonly Desktop _desktop;

        public MainGameScreen(RurikMonoGame game, Desktop desktop)
        {
            _rurikMonoGame = game;
            _desktop = desktop;
            _window = _desktop.Root as Window;
            game.MainGameScreen = this;
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

            // Add top and bottom panels to right grid
            Grid.SetRow(_rightTopPanel, 0);
            rightGrid.Widgets.Add(_rightTopPanel);

            Grid.SetRow(_rightBottomPanel, 1);
            rightGrid.Widgets.Add(_rightBottomPanel);

            // Add right panel to main grid
            Grid.SetColumn(_rightPanel, 1);
            _mainGrid.Widgets.Add(_rightPanel);

            // Add left panel to main grid
            Grid.SetColumn(_leftPanel, 0);
            _mainGrid.Widgets.Add(_leftPanel);

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
            _window.Content.RemoveFromParent();
            _window.Content = null;
            _window.Content = _panel;
            _window.Title = "Rurik: Game";
            _rurikMonoGame.CurrentMyraScreen = "MainGameScreen";
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

        public void UpdateGameInfo(GameStatus game)
        {
            Globals.Log("MainGameScreen.UpdateGameInfo(): enter");
            _game = game;

            // Update game info labels
            if (game != null)
            {
                // Update left panel with map texture
                updateMapPanel();
            }
        }

        private void updateMapPanel()
        {
            // Clear existing widgets in left panel
            _leftPanel.Widgets.Clear();

            // Add map texture display
            if (_rurikMonoGame.Textures != null)
            {
                var mapTexture = _rurikMonoGame.Textures.GetTexture("map");
                if (mapTexture != null)
                {
                    // Create a label with the map texture name
                    var mapLabel = new Label()
                    {
                        Id = "mapLabel",
                        Text = "Map Display (Texture: " + mapTexture.Name + ")",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    };

                    _leftPanel.Widgets.Add(mapLabel);
                }
                else
                {
                    var noMapLabel = new Label()
                    {
                        Id = "noMapLabel",
                        Text = "Map texture not loaded",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    _leftPanel.Widgets.Add(noMapLabel);
                }
            }
        }
    }
}
