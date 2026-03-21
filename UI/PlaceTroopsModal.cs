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
    public class PlaceTroopsModal : IGameScreen
    {
        private WindowPanel _windowPanel;
        private Panel _panel;
        private Grid _grid;
        private VerticalStackPanel _contentPanel;
        
        private Label _titleLabel;
        private Label _instructionsLabel;
        private Label _troopCountLabel;
        private TextBox _troopCountInput;
        private Label _locationLabel;
        private ComboView _locationSelect;
        private Button _placeButton;
        private Button _closeButton;
        
        public bool IsVisible {get; set;} = false;
        private GameStatus _game;
        private readonly Desktop _desktop;
        private readonly RurikMonoGame _rurikMonoGame;
        private int _troopCount = 1;
        private string _selectedLocation = "";

        public PlaceTroopsModal(RurikMonoGame game, Desktop desktop)
        {
            _rurikMonoGame = game;
            _desktop = desktop;
            Initialize();
        }

        public void Initialize()
        {
            // Main panel
            _panel = new Panel()
            {
                Id = "placeTroopsPanel",
                Background = new SolidBrush(Color.Black),
                Width = 400,
                Height = 300,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _grid = new Grid()
            {
                Id = "placeTroopsGrid",
                Background = new SolidBrush(Color.Black),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Title
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Instructions
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Troop Count Input
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Location Select
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Buttons

            _grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));

            setupContentPanel();

            _grid.Widgets.Add(_contentPanel);
            Grid.SetRow(_contentPanel, 4);

            _panel.Widgets.Add(_grid);
        }

        private void setupContentPanel()
        {
            _contentPanel = new VerticalStackPanel()
            {
                Id = "placeTroopsContentPanel",
                Background = new SolidBrush(Color.Gray),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Title label
            _titleLabel = new Label()
            {
                Id = "placeTroopsTitleLabel",
                Text = "Place Troops",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.DarkGray),
            };

            // Instructions label
            _instructionsLabel = new Label()
            {
                Id = "placeTroopsInstructionsLabel",
                Text = "Select location and troop count to place:",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Troop count label
            _troopCountLabel = new Label()
            {
                Id = "troopCountLabel",
                Text = "Troop Count:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Troop count input
            _troopCountInput = new TextBox()
            {
                Id = "troopCountInput",
                Width = 100,
                Height = 30,
                Text = "1",
            };
            _troopCountInput.TextChanged += (s, a) =>
            {
                if (int.TryParse(_troopCountInput.Text, out int result) && result > 0)
                {
                    _troopCount = result;
                }
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
                Text = "Place Troops",
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
            //_contentPanel.Widgets.Add(_titleLabel);
            //_contentPanel.Widgets.Add(_instructionsLabel);
            //_contentPanel.Widgets.Add(_troopCountLabel);
            //_contentPanel.Widgets.Add(_troopCountInput);
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

        public void BringToFront()
        {
            _windowPanel.BringToFront();
        }

        public void Show()
        {
            Show(600, 0);
        }

        public void Show(int x, int y)
        {
            if (! IsVisible)
            {
                IsVisible = true;
                // Populate location list when showing the modal
                //_window.ShowModal(_desktop);
                //_window.Show(_desktop);
                _windowPanel = new WindowPanel(_rurikMonoGame, _desktop, _panel, "Place Troops", 175, 0, x, y);
                _windowPanel.Show();
            }
        }

        private void PopulateLocationList(GameMap map)
        {
            // Clear existing location widgets
            if (_locationSelect != null && _locationSelect.Widgets != null)
            {
                _locationSelect.Widgets.Clear();
            }

            // Add location options to combo box.
            var locationNames = new List<string>();
            if (map != null)
            {
                locationNames = map.GetLocationsForGameNames();
                Globals.Log("PopulateLocationList(): locations=" + locationNames.Count);
            }
            else
            {
                Globals.Log("PopulateLocationList(): map is null, using default locationNames.");
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
            IsVisible = false;
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
            Globals.Log("UpdateGameInfo(): GameMap=" + map);
            _game = game;
            PopulateLocationList(map);
        }

        private void OnPlaceButtonClicked()
        {
            Globals.Log("PlaceTroops.OnPlaceButtonClicked(): enter");
            if (_game == null || _locationSelect.SelectedIndex == null || _locationSelect.SelectedIndex < 0)
                return;
            Globals.Log("PlaceTroops.OnPlaceButtonClicked(): get selectedItem");


            // Get the selected location
            var selectedItem = _locationSelect.Widgets[(int)_locationSelect.SelectedIndex] as Label;
            if (selectedItem == null)
                return;
            Globals.Log("PlaceTroops.OnPlaceButtonClicked(): selectedItem=" + selectedItem);
            _selectedLocation = selectedItem.Text;

            // Validate troop count
            if (_troopCount <= 0)
            {
                Globals.Log("PlaceTroops.OnPlaceButtonClicked(): Invalid troop count: " + _troopCount);
                return;
            }

            // Check if player has enough troops to deploy
            if (_game.ClientPlayer == null)
            {
                Globals.Log("PlaceTroops.OnPlaceButtonClicked(): Client player not found");
                return;
            }

            if (_game.ClientPlayer.TroopsToDeploy < _troopCount)
            {
                Globals.Log("PlaceTroops.OnPlaceButtonClicked(): Not enough troops to deploy. Available: " + _game.ClientPlayer.TroopsToDeploy + ", Requested: " + _troopCount);
                return;
            }

            // Create and send the action
            PlaceTroopsAction action = new PlaceTroopsAction(_rurikMonoGame.Client.ClientIdentifier);
            PlaceTroopsValues values = new PlaceTroopsValues
            {
                GameId = _game.Id,
                PlayerColor = _game.ClientPlayer.Color,
                TroopCount = _troopCount,
                TargetLocation = _selectedLocation
            };
            action.PlaceTroopsValues = values;
            _rurikMonoGame.Client.SendAction(action);

            //Hide();
            _windowPanel.Close();
        }
    }
}
