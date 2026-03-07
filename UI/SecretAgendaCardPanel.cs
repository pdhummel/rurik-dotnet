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
    /// <summary>
    /// Panel for rendering Secret Agenda cards.
    /// </summary>
    public class SecretAgendaCardPanel : CardPanel
    {
        private Label _nameLabel;
        private Label _textLabel;
        private Label _pointsLabel;
        private Label _accomplishedLabel;
        
        public SecretAgendaCardPanel(Desktop desktop, SecretAgendaCard card, bool isFaceUp = false)
            : base(desktop, new CardData(card.name, card.name, "SecretAgenda"), isFaceUp)
        {
        }
        
        protected override void SetupDataVisualization()
        {
            // Create a grid layout for the card data
            var grid = new Grid()
            {
                Id = "secretAgendaGrid",
                Background = new SolidBrush(new Color(40, 40, 60)),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
        
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Name
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Text
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Points
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Accomplished
        
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
        
            // Name label
            _nameLabel = new Label()
            {
                Id = "secretAgendaName",
                Text = _cardData.Name,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
        
            // Text label
            _textLabel = new Label()
            {
                Id = "secretAgendaText",
                Text = "",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
        
            // Points label
            _pointsLabel = new Label()
            {
                Id = "secretAgendaPoints",
                Text = "Points: 0",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
        
            // Accomplished label
            _accomplishedLabel = new Label()
            {
                Id = "secretAgendaAccomplished",
                Text = "Accomplished: No",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
        
            grid.Widgets.Add(_nameLabel);
            Grid.SetRow(_nameLabel, 0);
        
            grid.Widgets.Add(_textLabel);
            Grid.SetRow(_textLabel, 1);
        
            grid.Widgets.Add(_pointsLabel);
            Grid.SetRow(_pointsLabel, 2);
        
            grid.Widgets.Add(_accomplishedLabel);
            Grid.SetRow(_accomplishedLabel, 3);
        
            _backPanel.Widgets.Add(grid);
        }
        
        public void UpdateCardData(SecretAgendaCard card)
        {
            _nameLabel.Text = card.name;
            _textLabel.Text = card.text;
            _pointsLabel.Text = $"Points: {card.points}";
            _accomplishedLabel.Text = $"Accomplished: {(card.accomplished ? "Yes" : "No")}";
        }
    }
}
