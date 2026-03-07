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
    /// Panel for rendering Scheme cards.
    /// </summary>
    public class SchemeCardPanel : CardPanel
    {
        private Label _rewardsLabel;
        private Label _coinCostLabel;
        private Label _deathsLabel;
        
        public SchemeCardPanel(Desktop desktop, SchemeCard card, bool isFaceUp = false)
            : base(desktop, new CardData(card.id, "Scheme", "Scheme"), isFaceUp)
        {
        }
        
        protected override void SetupDataVisualization()
        {
            // Create a grid layout for the card data
            var grid = new Grid()
            {
                Id = "schemeCardGrid",
                Background = new SolidBrush(new Color(40, 40, 50)),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
        
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Rewards
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Coin Cost
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Deaths
        
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
        
            // Rewards label
            _rewardsLabel = new Label()
            {
                Id = "schemeCardRewards",
                Text = "Rewards: ",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
        
            // Coin Cost label
            _coinCostLabel = new Label()
            {
                Id = "schemeCardCoinCost",
                Text = "Coin Cost: 0",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
        
            // Deaths label
            _deathsLabel = new Label()
            {
                Id = "schemeCardDeaths",
                Text = "Deaths: 0",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
        
            grid.Widgets.Add(_rewardsLabel);
            Grid.SetRow(_rewardsLabel, 0);
        
            grid.Widgets.Add(_coinCostLabel);
            Grid.SetRow(_coinCostLabel, 1);
        
            grid.Widgets.Add(_deathsLabel);
            Grid.SetRow(_deathsLabel, 2);
        
            _backPanel.Widgets.Add(grid);
        }
        
        public void UpdateCardData(SchemeCard card)
        {
            _rewardsLabel.Text = $"Rewards: {string.Join(", ", card.rewards)}";
            _coinCostLabel.Text = $"Coin Cost: {card.rewardCoinCost}";
            _deathsLabel.Text = $"Deaths: {card.deaths}";
        }
    }
}
