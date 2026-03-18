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
using System.Runtime.CompilerServices;

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
        private ComboView _leaderComboView;
        private Label _descriptionLabel;
        private Button _selectButton;
        private Button _closeButton;
        
        public bool IsVisible = false;
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
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Leader Combo
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Description
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Buttons

            _grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));

            setupContentPanel();

            _grid.Widgets.Add(_contentPanel);
            Grid.SetRow(_contentPanel, 4);

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

            // Leader combo view
            _leaderComboView = new ComboView()
            {
                Id = "leaderComboView",
                Width = 400,
                Height = 30,
            };
            _leaderComboView.SelectedIndexChanged += (s, a) => OnLeaderSelectionChanged();

            // Description label
            _descriptionLabel = new Label()
            {
                Id = "descriptionLabel",
                Text = "",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Wrap = true,
                MaxWidth = 450,
            };

            // Select button
            _selectButton = new Button()
            {
                Id = "selectButton",
                Width = 100,
                Height = 30,
                Border = new SolidBrush("#808000FF"),
                BorderThickness = new Thickness(2),
            };
            _selectButton.Content = new Label
            {
                Text = "Select",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _selectButton.Click += (s, a) => OnSelectButtonClicked();

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
            _contentPanel.Widgets.Add(_instructionsLabel);
            _contentPanel.Widgets.Add(_leaderComboView);
            _contentPanel.Widgets.Add(_descriptionLabel);
            _contentPanel.Widgets.Add(_selectButton);
            //_contentPanel.Widgets.Add(_closeButton);
        }

        private void OnLeaderSelectionChanged()
        {
            if (_leaderComboView.SelectedIndex != null && _leaderComboView.SelectedIndex >= 0 && _leaderComboView.SelectedIndex < _availableLeaderNames.Count)
            {
                string selectedLeaderName = _availableLeaderNames[(int)_leaderComboView.SelectedIndex];
                Leader leader = _game?.AvailableLeaders?.GetLeaderByName(selectedLeaderName);
                if (leader != null)
                {
                    _descriptionLabel.Text = leader.description;
                }
            }
            else
            {
                _descriptionLabel.Text = "";
            }
        }

        private void OnSelectButtonClicked()
        {
            if (_leaderComboView.SelectedIndex != null && _leaderComboView.SelectedIndex >= 0 && _leaderComboView.SelectedIndex < _availableLeaderNames.Count)
            {
                string selectedLeaderName = _availableLeaderNames[(int)_leaderComboView.SelectedIndex];
                OnLeaderButtonClicked(selectedLeaderName);
            }
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
            IsVisible = true;
            _window.ShowModal(_desktop);
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

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateGameInfo(GameStatus game)
        {
            _game = game;

            // Clear existing items in combo view
            _leaderComboView.Widgets.Clear();
            _availableLeaderNames.Clear();

            // Get available leaders from the game status
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

            // Add leader options to the combo view
            List<string> leaderNames = new List<string>();
            try
            {
                leaderNames = new List<string>(_availableLeaderNames);    
            }
            catch(Exception ex)
            {
                leaderNames = new List<string>(_availableLeaderNames);
            }
            foreach (var leaderName in leaderNames)
            {
                _leaderComboView.Widgets.Add(new Label() { Text = leaderName });
            }

            // Select first item by default if available
            if (_leaderComboView.Widgets.Count > 0)
            {
                _leaderComboView.SelectedIndex = 0;
            }

            // Clear description if no leaders available
            if (_availableLeaderNames.Count == 0)
            {
                _descriptionLabel.Text = "";
            }
        }

        private void OnLeaderButtonClicked(string leaderName)
        {
            if (_game == null)
                return;

            // Get the current player's color from the game status
            if (!string.IsNullOrEmpty(_game.CurrentPlayerColor))
            {
                _currentPlayerColor = _game.CurrentPlayerColor;
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
