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
    /// Panel for rendering Deed cards.
    /// </summary>
    public class DeedCardPanel : CardPanel
    {
        private Label _nameLabel;
        private Label _victoryPointsLabel;
        private Label _requirementLabel;
        private Label _rewardsLabel;
        private Label _costsLabel;
        private Label _accomplishedLabel;
        
        public DeedCardPanel(Desktop desktop, DeedCard card, bool isFaceUp = false)
            : base(desktop, new CardData(card.name, card.name, "Deed"), isFaceUp)
        {
        }
        
        protected override void SetupDataVisualization()
        {
            // Create a grid layout for the card data
            var grid = new Grid()
            {
                Id = "deedCardGrid",
                Background = new SolidBrush(new Color(50, 40, 40)),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
        
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Name
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Victory Points
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Requirement
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Rewards
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Costs
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // Accomplished
        
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
        
            // Name label
            _nameLabel = new Label()
            {
                Id = "deedCardName",
                Text = _cardData.Name,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
        
            // Victory Points label
            _victoryPointsLabel = new Label()
            {
                Id = "deedCardVictoryPoints",
                Text = "Victory Points: 0",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
        
            // Requirement label
            _requirementLabel = new Label()
            {
                Id = "deedCardRequirement",
                Text = "Requirement: ",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
        
            // Rewards label
            _rewardsLabel = new Label()
            {
                Id = "deedCardRewards",
                Text = "Rewards: ",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
        
            // Costs label
            _costsLabel = new Label()
            {
                Id = "deedCardCosts",
                Text = "Costs: ",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
        
            // Accomplished label
            _accomplishedLabel = new Label()
            {
                Id = "deedCardAccomplished",
                Text = "Accomplished: No",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
        
            grid.Widgets.Add(_nameLabel);
            Grid.SetRow(_nameLabel, 0);
        
            grid.Widgets.Add(_victoryPointsLabel);
            Grid.SetRow(_victoryPointsLabel, 1);
        
            grid.Widgets.Add(_requirementLabel);
            Grid.SetRow(_requirementLabel, 2);
        
            grid.Widgets.Add(_rewardsLabel);
            Grid.SetRow(_rewardsLabel, 3);
        
            grid.Widgets.Add(_costsLabel);
            Grid.SetRow(_costsLabel, 4);
        
            grid.Widgets.Add(_accomplishedLabel);
            Grid.SetRow(_accomplishedLabel, 5);
        
            _backPanel.Widgets.Add(grid);
        }
        
        public void UpdateCardData(DeedCard card)
        {
            _nameLabel.Text = card.name;
            _victoryPointsLabel.Text = $"Victory Points: {card.victoryPoints}";
            _requirementLabel.Text = $"Requirement: {card.requirementText}";
            _rewardsLabel.Text = $"Rewards: {string.Join(", ", card.rewards)}";
            _costsLabel.Text = $"Costs: {string.Join(", ", card.costs)}";
            _accomplishedLabel.Text = $"Accomplished: {(card.accomplished ? "Yes" : "No")}";
        }
    }
}
