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
    public class ChooseLeaderModal : IGameScreen
    {
        private Window _window = new Window();
        private Panel _panel;
        private Grid _grid;
        private VerticalStackPanel _contentPanel;
        
        private Label _titleLabel;
        private Label _instructionsLabel;
        private VerticalStackPanel _leaderListPanel;
        private Button _closeButton;
        
        private bool _isVisible = false;
        private GameStatus _game;
        private readonly Desktop _desktop;
        private readonly RurikMonoGame _rurikMonoGame;
        private string _currentPlayerColor = "";
        private List<string> _availableLeaderNames = new List<string>();

        public ChooseLeaderModal(RurikMonoGame game, Desktop desktop)
        {
            _rurikMonoGame = game;
            _desktop = desktop;
            Initialize();
        }

        public void Initialize()
        {
            _window.Title = "Choose Leader";
            _window.Width = 500;
            _window.Height = 400;
            _window.CloseButton.Visible = false;

            // Main panel
            _panel = new Panel()
            {
                Id = "chooseLeaderPanel",
                Background = new SolidBrush(Color.Black),
                Width = 500,
                Height = 400,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _grid = new Grid()
            {
                Id = "chooseLeaderGrid",
                Background = new SolidBrush(Color.Black),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Title
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Instructions
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Leader List
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
                Id = "chooseLeaderContentPanel",
                Background = new SolidBrush(Color.Gray),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Title label
            _titleLabel = new Label()
            {
                Id = "chooseLeaderTitleLabel",
                Text = "Select Leader",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.DarkGray),
            };

            // Instructions label
            _instructionsLabel = new Label()
            {
                Id = "chooseLeaderInstructionsLabel",
                Text = "Choose your leader for the game:",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Leader list panel
            _leaderListPanel = new VerticalStackPanel()
            {
                Id = "leaderListPanel",
                Background = new SolidBrush(Color.LightGray),
                Padding = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

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
            _contentPanel.Widgets.Add(_leaderListPanel);
            _contentPanel.Widgets.Add(_closeButton);
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

            // Clear existing leader widgets
            for (int i = _leaderListPanel.Widgets.Count - 1; i >= 0; i--)
            {
                _leaderListPanel.Widgets.RemoveAt(i);
            }

            // Get available leaders from the game status
            _availableLeaderNames.Clear();
            if (_game != null && _game.AvailableLeaders != null)
            {
                // Get leaders that are still available (not yet chosen by any player)
                foreach (var leaderKey in _game.AvailableLeaders.GetAvailableLeaderNames())
                {
                    bool leaderChosen = false;
                    if (_game.Players != null && _game.Players.players != null)
                    {
                        foreach (var player in _game.Players.players)
                        {
                            if (player.leader != null && player.leader.name == leaderKey)
                            {
                                leaderChosen = true;
                                break;
                            }
                        }
                    }
                    if (!leaderChosen)
                    {
                        _availableLeaderNames.Add(leaderKey);
                    }
                }
            }

            // Add leader options to the list
            if (_game != null && _game.AvailableLeaders != null)
            {
                foreach (var leaderName in _availableLeaderNames)
                {
                    Leader leader = _game.AvailableLeaders.GetLeaderByName(leaderName);
                    if (leader != null)
                    {
                        Button leaderButton = new Button()
                        {
                            Id = "leaderButton_" + leaderName,
                            Width = 400,
                            Height = 60,
                            Border = new SolidBrush("#808000FF"),
                            BorderThickness = new Thickness(2),
                        };
                        leaderButton.Content = new Label
                        {
                            Text = leader.name,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                        };
                        leaderButton.Click += (s, a) => OnLeaderButtonClicked(leaderName);

                        _leaderListPanel.Widgets.Add(leaderButton);
                    }
                }
            }
        }

        private void OnLeaderButtonClicked(string leaderName)
        {
            if (_game == null)
                return;

            // Get the current player's color from the game status
            if (!string.IsNullOrEmpty(_game.CurrentPlayerName))
            {
                _currentPlayerColor = _game.CurrentPlayerName;
            }

            if (string.IsNullOrEmpty(_currentPlayerColor))
                return;

            // Create and send the action
            ChooseLeaderAction action = new ChooseLeaderAction(_rurikMonoGame.Client.ClientIdentifier);
            ChooseLeaderValues values = new ChooseLeaderValues(_game.Id, _currentPlayerColor, leaderName);
            action.ChooseLeaderValues = values;
            _rurikMonoGame.Client.SendAction(action);

            Hide();
        }
    }
}
