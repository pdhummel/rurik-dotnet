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
    public class PlayAdvisorModal : IGameScreen
    {
        private Window _window = new Window();
        private Panel _panel;
        private Grid _grid;
        private VerticalStackPanel _contentPanel;
        
        private Label _titleLabel;
        private Label _instructionsLabel;
        private Label _advisorLabel;
        private ComboView _advisorSelect;
        private Label _columnLabel;
        private ComboView _columnSelect;
        private Label _bidLabel;
        private TextBox _bidInput;
        private Button _playButton;
        private Button _closeButton;
        
        public bool IsVisible {get; set;} = false;
        private GameStatus _game;
        private readonly Desktop _desktop;
        private readonly RurikMonoGame _rurikMonoGame;
        private string _selectedAdvisor = "";
        private string _selectedColumn = "";
        private int _bidAmount = 0;

        public PlayAdvisorModal(RurikMonoGame game, Desktop desktop)
        {
            _rurikMonoGame = game;
            _desktop = desktop;
            Initialize();
        }

        public void Initialize()
        {
            _window.Title = "Play Advisor";
            _window.Width = 400;
            _window.Height = 350;
            _window.CloseButton.Visible = false;

            // Main panel
            _panel = new Panel()
            {
                Id = "playAdvisorPanel",
                Background = new SolidBrush(Color.Black),
                Width = 400,
                Height = 350,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _grid = new Grid()
            {
                Id = "playAdvisorGrid",
                Background = new SolidBrush(Color.Black),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Title
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Instructions
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Advisor Select
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Column Select
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Bid Input
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Buttons

            _grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));

            setupContentPanel();

            _grid.Widgets.Add(_contentPanel);
            Grid.SetRow(_contentPanel, 5);

            _panel.Widgets.Add(_grid);
            _window.Content = _panel;
        }

        private void setupContentPanel()
        {
            _contentPanel = new VerticalStackPanel()
            {
                Id = "playAdvisorContentPanel",
                Background = new SolidBrush(Color.Gray),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Title label
            _titleLabel = new Label()
            {
                Id = "playAdvisorTitleLabel",
                Text = "Play Advisor",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.DarkGray),
            };

            // Instructions label
            _instructionsLabel = new Label()
            {
                Id = "playAdvisorInstructionsLabel",
                Text = "Select advisor, column, and bid amount:",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Advisor label
            _advisorLabel = new Label()
            {
                Id = "advisorLabel",
                Text = "Advisor:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Advisor selection combo box
            _advisorSelect = new ComboView()
            {
                Id = "advisorSelect",
                Width = 200,
                Height = 30,
            };

            // Column label
            _columnLabel = new Label()
            {
                Id = "columnLabel",
                Text = "Column:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Column selection combo box
            _columnSelect = new ComboView()
            {
                Id = "columnSelect",
                Width = 200,
                Height = 30,
            };

            // Bid label
            _bidLabel = new Label()
            {
                Id = "bidLabel",
                Text = "Bid Coins:",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Bid input
            _bidInput = new TextBox()
            {
                Id = "bidInput",
                Width = 100,
                Height = 30,
                Text = "0",
            };
            _bidInput.TextChanged += (s, a) =>
            {
                if (int.TryParse(_bidInput.Text, out int result) && result >= 0)
                {
                    _bidAmount = result;
                }
            };

            // Play button
            _playButton = new Button()
            {
                Id = "playButton",
                Width = 150,
                Height = 30,
                Border = new SolidBrush("#808000FF"),
                BorderThickness = new Thickness(2),
            };
            _playButton.Content = new Label
            {
                Text = "Play Advisor",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _playButton.Click += (s, a) => OnPlayButtonClicked();

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
            _contentPanel.Widgets.Add(_advisorLabel);
            _contentPanel.Widgets.Add(_advisorSelect);
            _contentPanel.Widgets.Add(_columnLabel);
            _contentPanel.Widgets.Add(_columnSelect);
            _contentPanel.Widgets.Add(_bidLabel);
            _contentPanel.Widgets.Add(_bidInput);
            _contentPanel.Widgets.Add(_playButton);
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
            IsVisible = true;
            // Populate advisor and column lists when showing the modal
            PopulateAdvisorList();
            PopulateColumnList();
            _window.ShowModal(_desktop);
        }

        private void PopulateAdvisorList()
        {
            _advisorSelect.Widgets.Clear();

            // Add advisor options to combo box
            var availableAdvisors = new List<string>();
            if (_game != null && _game.ClientPlayer != null)
            {
                availableAdvisors = _game.ClientPlayer.Advisors;
            }

            foreach (var advisorName in availableAdvisors)
            {
                _advisorSelect.Widgets.Add(new Label() { Text = advisorName });
            }

            // Select the first advisor by default if available
            if (_advisorSelect.Widgets.Count > 0)
            {
                _advisorSelect.SelectedIndex = 0;
                _selectedAdvisor = availableAdvisors.Count > 0 ? availableAdvisors[0] : "";
            }
        }

        private void PopulateColumnList()
        {
            _columnSelect.Widgets.Clear();

            // Add column options to combo box
            var columnNames = new List<string>();
            if (_game != null && _game.AuctionBoard != null)
            {
                columnNames = _game.AuctionBoard.board.Keys.ToList();
            }
            else
            {
                // Default column names if auction board is not available
                columnNames = new List<string>
                {
                    "scheme", "muster", "move", "attack", "tax", "build"
                };
            }

            foreach (var columnName in columnNames)
            {
                _columnSelect.Widgets.Add(new Label() { Text = columnName });
            }

            // Select the first column by default if available
            if (_columnSelect.Widgets.Count > 0)
            {
                _columnSelect.SelectedIndex = 0;
                _selectedColumn = columnNames.Count > 0 ? columnNames[0] : "";
            }
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

        public void UpdateGameInfo(GameStatus game)
        {
            _game = game;
        }

        private void OnPlayButtonClicked()
        {
            Globals.Log("PlayAdvisorModal.OnPlayButtonClicked(): enter");
            if (_game == null)
                return;

            // Get the selected advisor
            if (_advisorSelect.SelectedIndex == null || _advisorSelect.SelectedIndex < 0)
            {
                Globals.Log("PlayAdvisorModal.OnPlayButtonClicked(): No advisor selected");
                return;
            }
            var advisorItem = _advisorSelect.Widgets[(int)_advisorSelect.SelectedIndex] as Label;
            if (advisorItem == null)
            {
                Globals.Log("PlayAdvisorModal.OnPlayButtonClicked(): Advisor item not found");
                return;
            }
            _selectedAdvisor = advisorItem.Text;

            // Get the selected column
            if (_columnSelect.SelectedIndex == null || _columnSelect.SelectedIndex < 0)
            {
                Globals.Log("PlayAdvisorModal.OnPlayButtonClicked(): No column selected");
                return;
            }
            var columnItem = _columnSelect.Widgets[(int)_columnSelect.SelectedIndex] as Label;
            if (columnItem == null)
            {
                Globals.Log("PlayAdvisorModal.OnPlayButtonClicked(): Column item not found");
                return;
            }
            _selectedColumn = columnItem.Text;

            // Get the current player's color from the game status
            string playerColor = "";
            if (!string.IsNullOrEmpty(_game.CurrentPlayerColor))
            {
                playerColor = _game.CurrentPlayerColor;
            }

            if (string.IsNullOrEmpty(playerColor))
            {
                Globals.Log("PlayAdvisorModal.OnPlayButtonClicked(): Current player color not found");
                return;
            }

            // Create and send the action
            PlaceAdvisorAction action = new PlaceAdvisorAction(_rurikMonoGame.Client.ClientIdentifier);
            PlaceAdvisorValues values = new PlaceAdvisorValues
            {
                GameId = _game.Id,
                PlayerColor = playerColor,
                Advisor = _selectedAdvisor,
                ActionColumn = _selectedColumn,
                BidCoins = _bidAmount
            };
            action.PlaceAdvisorValues = values;
            _rurikMonoGame.Client.SendAction(action);

            Hide();
        }
    }
}
