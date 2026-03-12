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
    public class PlaceLeaderModal : IGameScreen
    {
        private Window _window = new Window();
        private Panel _panel;
        private Grid _grid;
        private VerticalStackPanel _contentPanel;
        
        private Label _titleLabel;
        private Label _instructionsLabel;
        private Label _locationLabel;
        private ComboView _locationSelect;
        private Button _placeButton;
        private Button _closeButton;
        
        private bool _isVisible = false;
        private GameStatus _game;
        private GameMap? _gameMap;
        private readonly Desktop _desktop;
        private readonly RurikMonoGame _rurikMonoGame;
        private string _selectedLocation = "";

        public PlaceLeaderModal(RurikMonoGame game, Desktop desktop)
        {
            _rurikMonoGame = game;
            _desktop = desktop;
            Initialize();
        }

        public void Initialize()
        {
            _window.Title = "Place Leader";
            _window.Width = 400;
            _window.Height = 300;
            _window.CloseButton.Visible = false;

            // Main panel
            _panel = new Panel()
            {
                Id = "placeLeaderPanel",
                Background = new SolidBrush(Color.Black),
                Width = 400,
                Height = 300,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _grid = new Grid()
            {
                Id = "placeLeaderGrid",
                Background = new SolidBrush(Color.Black),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Title
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Instructions
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Location Select
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Buttons

            _grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));

            setupContentPanel();

            _grid.Widgets.Add(_contentPanel);
            Grid.SetRow(_contentPanel, 3);

            _panel.Widgets.Add(_grid);
            _window.Content = _panel;
        }

        private void setupContentPanel()
        {
            _contentPanel = new VerticalStackPanel()
            {
                Id = "placeLeaderContentPanel",
                Background = new SolidBrush(Color.Gray),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Title label
            _titleLabel = new Label()
            {
                Id = "placeLeaderTitleLabel",
                Text = "Place Leader",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.DarkGray),
            };

            // Instructions label
            _instructionsLabel = new Label()
            {
                Id = "placeLeaderInstructionsLabel",
                Text = "Select location to place your leader:",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Location label
            _locationLabel = new Label()
            {
                Id = "locationLabel",
                Text = "Location:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Location selection combo box
            _locationSelect = new ComboView()
            {
                Id = "locationSelect",
                Width = 200,
                Height = 30,
            };

            // Place button
            _placeButton = new Button()
            {
                Id = "placeButton",
                Width = 150,
                Height = 30,
                Border = new SolidBrush("#808000FF"),
                BorderThickness = new Thickness(2),
            };
            _placeButton.Content = new Label
            {
                Text = "Place Leader",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _placeButton.Click += (s, a) => OnPlaceButtonClicked();

            // Close button
            _closeButton = new Button()
            {
                Id = "closeButton",
                Width = 100,
                Height = 30,
                Border = new SolidBrush("#808000FF"),
                BorderThickness = new Thickness(2),
            };
            _closeButton.Content = new Label
            {
                Text = "Close",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _closeButton.Click += (s, a) => Hide();

            // Add widgets to panel
            _contentPanel.Widgets.Add(_titleLabel);
            _contentPanel.Widgets.Add(_instructionsLabel);
            _contentPanel.Widgets.Add(_locationLabel);
            _contentPanel.Widgets.Add(_locationSelect);
            _contentPanel.Widgets.Add(_placeButton);
            //_contentPanel.Widgets.Add(_closeButton);
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
            // Populate location list when showing the modal
            PopulateLocationList();
            _window.ShowModal(_desktop);
        }
        
        private void PopulateLocationList()
        {
            _locationSelect.Widgets.Clear();

            // Add location options to combo box
            var locationNames = new List<string>();
            if (_gameMap != null)
            {
                locationNames = _gameMap.GetLocationsForGameNames();
            }
            else
            {
                Globals.Log("PopulateLocationList(): gameMap is null, using default locationNames.");
                locationNames = new List<string>
                {
                    "Novgorod", "Pskov", "Polotsk", "Smolensk", "Rostov", "Chernigov", "Suzdal", "Pereyaslavl",
                    "Volyn", "Kiev", "Galich", "Murom", "Brest", "Peresech", "Azov"
                };
            }
            locationNames.Sort(); // Sort locations alphabetically

            foreach (var locationName in locationNames)
            {
                _locationSelect.Widgets.Add(new Label() { Text = locationName });
            }

            // Select the first location by default
            if (_locationSelect.Widgets.Count > 0)
            {
                _locationSelect.SelectedIndex = 0;
                _selectedLocation = locationNames[0];
            }
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
                    UpdateGameInfo(data as GameStatus, null);
                    break;
            }
        }

        public void UpdateGameInfo(GameStatus game, GameMap map)
        {
            _game = game;
            _gameMap = map;
        }

        private void OnPlaceButtonClicked()
        {
            Globals.Log("PlaceLeaderModal.OnPlaceButtonClicked(): enter");
            if (_game == null || _locationSelect.SelectedIndex == null || _locationSelect.SelectedIndex < 0)
                return;

            // Get the selected location
            var selectedItem = _locationSelect.Widgets[(int)_locationSelect.SelectedIndex] as Label;
            if (selectedItem == null)
                return;
            _selectedLocation = selectedItem.Text;

            // Get the current player's color from the game status
            string playerColor = "";
            if (!string.IsNullOrEmpty(_game.CurrentPlayerColor))
            {
                playerColor = _game.CurrentPlayerColor;
            }

            if (string.IsNullOrEmpty(playerColor))
            {
                Globals.Log("PlaceLeaderModal.OnPlaceButtonClicked(): Current player color not found");
                return;
            }

            // Create and send the action
            PlaceLeaderAction action = new PlaceLeaderAction(_rurikMonoGame.Client.ClientIdentifier);
            PlaceLeaderValues values = new PlaceLeaderValues
            {
                GameId = _game.Id,
                PlayerColor = playerColor,
                TargetLocation = _selectedLocation
            };
            action.PlaceLeaderValues = values;
            _rurikMonoGame.Client.SendAction(action);

            Hide();
        }
    }
}
