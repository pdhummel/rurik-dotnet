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
    public class ChooseSecretAgendaModal : IGameScreen
    {
        private Window _window = new Window();
        private Panel _panel;
        private Grid _grid;
        private VerticalStackPanel _contentPanel;
        
        private Label _titleLabel;
        private Label _instructionsLabel;
        private HorizontalStackPanel _cardPanel;
        private Button _card1Button;
        private Button _card2Button;
        private Button _closeButton;
        
        private bool _isVisible = false;
        private GameStatus _game;
        private readonly Desktop _desktop;
        private readonly RurikMonoGame _rurikMonoGame;
        private string _currentPlayerColor = "";
        private List<SecretAgendaCard> _secretAgendaCards = new List<SecretAgendaCard>();

        public ChooseSecretAgendaModal(RurikMonoGame game, Desktop desktop)
        {
            _rurikMonoGame = game;
            _desktop = desktop;
            Initialize();
        }

        public void Initialize()
        {
            _window.Title = "Choose Secret Agenda";
            _window.Width = 600;
            _window.Height = 300;
            _window.CloseButton.Visible = false;

            // Main panel
            _panel = new Panel()
            {
                Id = "chooseSecretAgendaPanel",
                Background = new SolidBrush(Color.Black),
                Width = 600,
                Height = 300,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _grid = new Grid()
            {
                Id = "chooseSecretAgendaGrid",
                Background = new SolidBrush(Color.Black),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Title
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Instructions
            _grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Card Panel
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
                Id = "chooseSecretAgendaContentPanel",
                Background = new SolidBrush(Color.Gray),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Title label
            _titleLabel = new Label()
            {
                Id = "chooseSecretAgendaTitleLabel",
                Text = "Select Secret Agenda",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.DarkGray),
            };

            // Instructions label
            _instructionsLabel = new Label()
            {
                Id = "chooseSecretAgendaInstructionsLabel",
                Text = "You have been dealt 2 secret agenda cards. Select which one to keep:",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Card panel - displays the 2 cards side by side
            _cardPanel = new HorizontalStackPanel()
            {
                Id = "cardPanel",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Card 1 button
            _card1Button = new Button()
            {
                Id = "card1Button",
                Width = 200,
                Height = 100,
                Border = new SolidBrush("#808000FF"),
                BorderThickness = new Thickness(2),
            };
            _card1Button.Click += (s, a) => OnCard1ButtonClicked();

            // Card 2 button
            _card2Button = new Button()
            {
                Id = "card2Button",
                Width = 200,
                Height = 100,
                Border = new SolidBrush("#808000FF"),
                BorderThickness = new Thickness(2),
            };
            _card2Button.Click += (s, a) => OnCard2ButtonClicked();

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
            _contentPanel.Widgets.Add(_cardPanel);
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

            // Clear existing card widgets
            for (int i = _cardPanel.Widgets.Count - 1; i >= 0; i--)
            {
                _cardPanel.Widgets.RemoveAt(i);
            }

            // Get the current player's secret agenda cards
            _secretAgendaCards.Clear();
            if (_game != null && _game.ClientPlayer != null && _game.ClientPlayer.secretAgenda != null)
            {
                _secretAgendaCards = _game.ClientPlayer.secretAgenda.ToList();
            }

            // Update card buttons with card names
            if (_secretAgendaCards.Count >= 2)
            {
                // Card 1
                _card1Button.Content = new Label
                {
                    Text = _secretAgendaCards[0].name,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };

                // Card 2
                _card2Button.Content = new Label
                {
                    Text = _secretAgendaCards[1].name,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };

                // Show both cards
                _cardPanel.Widgets.Add(_card1Button);
                _cardPanel.Widgets.Add(_card2Button);
            }
            else if (_secretAgendaCards.Count == 1)
            {
                // Only one card (user has already selected one)
                _card1Button.Content = new Label
                {
                    Text = _secretAgendaCards[0].name,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };

                // Hide card 2 button
                _cardPanel.Widgets.Add(_card1Button);
            }
            else
            {
                // No cards - shouldn't happen in this state
                _card1Button.Content = new Label
                {
                    Text = "No cards",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                _cardPanel.Widgets.Add(_card1Button);
            }
        }

        private void OnCard1ButtonClicked()
        {
            if (_secretAgendaCards.Count < 1)
                return;

            OnCardButtonClicked(_secretAgendaCards[0].name);
        }

        private void OnCard2ButtonClicked()
        {
            if (_secretAgendaCards.Count < 2)
                return;

            OnCardButtonClicked(_secretAgendaCards[1].name);
        }

        private void OnCardButtonClicked(string cardName)
        {
            if (_game == null || string.IsNullOrEmpty(cardName))
                return;

            // Get the current player's color from the game status
            if (!string.IsNullOrEmpty(_game.CurrentPlayerColor))
            {
                _currentPlayerColor = _game.CurrentPlayerColor;
            }

            if (string.IsNullOrEmpty(_currentPlayerColor))
                return;

            // Create and send the action
            ChooseSecretAgendaAction action = new ChooseSecretAgendaAction(_rurikMonoGame.Client.ClientIdentifier);
            ChooseSecretAgendaValues values = new ChooseSecretAgendaValues(_game.Id, _currentPlayerColor, cardName);
            action.ChooseSecretAgendaValues = values;
            _rurikMonoGame.Client.SendAction(action);

            Hide();
        }
    }
}
