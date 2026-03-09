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
    public class ChooseFirstPlayerModal : IGameScreen
    {
        private Window _window = new Window();
        private Panel _panel;
        private Grid _grid;
        private VerticalStackPanel _contentPanel;
        
        private Label _titleLabel;
        private Label _instructionsLabel;
        private ComboView _playerSelect;
        private Button _chooseButton;
        private Button _randomButton;
        private Button _closeButton;
        
        private bool _isVisible = false;
        private GameStatus _game;
        private readonly Desktop _desktop;
        private readonly RurikMonoGame _rurikMonoGame;
        private string _currentPlayerColor = "";

        public ChooseFirstPlayerModal(RurikMonoGame game, Desktop desktop)
        {
            _rurikMonoGame = game;
            _desktop = desktop;
            Initialize();
        }

        public void Initialize()
        {
            _window.Title = "Choose First Player";
            _window.Width = 400;
            _window.Height = 300;
            _window.CloseButton.Visible = false;

            // Main panel
            _panel = new Panel()
            {
                Id = "chooseFirstPlayerPanel",
                Background = new SolidBrush(Color.Black),
                Width = 400,
                Height = 300,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _grid = new Grid()
            {
                Id = "chooseFirstPlayerGrid",
                Background = new SolidBrush(Color.Black),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Title
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Instructions
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Player Select
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Buttons
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Random Button

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
                Id = "chooseFirstPlayerContentPanel",
                Background = new SolidBrush(Color.Gray),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Title label
            _titleLabel = new Label()
            {
                Id = "chooseFirstPlayerTitleLabel",
                Text = "Select First Player",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.DarkGray),
            };

            // Instructions label
            _instructionsLabel = new Label()
            {
                Id = "chooseFirstPlayerInstructionsLabel",
                Text = "Choose who goes first in the game:",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Player selection combo box
            _playerSelect = new ComboView()
            {
                Id = "playerSelect",
                Width = 200,
                Height = 30,
            };

            // Choose button
            _chooseButton = new Button()
            {
                Id = "chooseButton",
                Width = 150,
                Height = 30,
                Border = new SolidBrush("#808000FF"),
                BorderThickness = new Thickness(2),
            };
            _chooseButton.Content = new Label
            {
                Text = "Selected Player",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _chooseButton.Click += (s, a) => OnChooseButtonClicked();

            // Random button
            _randomButton = new Button()
            {
                Id = "randomButton",
                Width = 150,
                Height = 30,
                Border = new SolidBrush("#808000FF"),
                BorderThickness = new Thickness(2),
            };
            _randomButton.Content = new Label
            {
                Text = "Random Player",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _randomButton.Click += (s, a) => OnRandomButtonClicked();

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
            _contentPanel.Widgets.Add(_playerSelect);
            _contentPanel.Widgets.Add(_chooseButton);
            _contentPanel.Widgets.Add(_randomButton);
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
            _window.ShowModal(_desktop);
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
            _game = game;

            // Clear existing player widgets
            for (int i = _playerSelect.Widgets.Count - 1; i >= 0; i--)
            {   
                if (_playerSelect !=  null && _playerSelect.Widgets != null && _playerSelect.Widgets.Count >  i && i >0)
                    _playerSelect.Widgets.RemoveAt(i);
            }

            // Add player options to combo box
            if (_game != null && _game.Players != null)
            {
                foreach (var player in _game.Players.players)
                {
                    _playerSelect.Widgets.Add(new Label() { Text = player.Color + ": " + player.name });
                }

                // Select the first player by default
                if (_playerSelect.Widgets.Count > 0)
                {
                    _playerSelect.SelectedIndex = 0;
                }
            }
        }

        private void OnChooseButtonClicked()
        {
            Globals.Log("ChooseFirstPlayerModal.OnChooseButtonClicked(): enter");
            if (_game == null || _playerSelect.SelectedIndex == null || _playerSelect.SelectedIndex < 0)
                return;

            // Get the selected player color
            var selectedItem = _playerSelect.Widgets[(int)_playerSelect.SelectedIndex] as Label;
            if (selectedItem == null)
                return;

            // Extract color from "color: name" format
            string selectedText = selectedItem.Text;
            int colonIndex = selectedText.IndexOf(':');
            if (colonIndex > 0)
            {
                _currentPlayerColor = selectedText.Substring(0, colonIndex).Trim();
            }

            // Create and send the action
            ChooseFirstPlayerAction action = new ChooseFirstPlayerAction(_rurikMonoGame.Client.ClientIdentifier);
            ChooseFirstPlayerValues values = new ChooseFirstPlayerValues(_game.Id, _currentPlayerColor);
            action.ChooseFirstPlayerValues = values;
            _rurikMonoGame.Client.SendAction(action);

            Hide();
        }

        private void OnRandomButtonClicked()
        {
            if (_game == null)
                return;

            // Create and send the action with null player color for random selection
            ChooseFirstPlayerAction action = new ChooseFirstPlayerAction(_rurikMonoGame.Client.ClientIdentifier);
            ChooseFirstPlayerValues values = new ChooseFirstPlayerValues(_game.Id, null);
            action.ChooseFirstPlayerValues = values;
            _rurikMonoGame.Client.SendAction(action);

            Hide();
        }
    }
}
