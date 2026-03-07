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
    /// Base class for card panels that render card data with a two-sided design.
    /// One side displays an image (card back), the other side visualizes the card data.
    /// </summary>
    public abstract class CardPanel : Panel
    {
        protected readonly Desktop _desktop;
        protected readonly CardData _cardData;
        protected bool _isFaceUp;
        
        protected Panel _frontPanel;
        protected Panel _backPanel;
        
        public CardPanel(Desktop desktop, CardData cardData, bool isFaceUp = false)
            : base()
        {
            _desktop = desktop;
            _cardData = cardData;
            _isFaceUp = isFaceUp;
      
            Initialize();
        }
        
        protected virtual void Initialize()
        {
            // Set base panel properties
            this.Id = $"cardPanel_{_cardData.Id}";
            this.Width = 200;
            this.Height = 280;
            this.HorizontalAlignment = HorizontalAlignment.Center;
            this.VerticalAlignment = VerticalAlignment.Center;
      
            // Create front panel (image side - card back)
            _frontPanel = new Panel()
            {
                Id = "frontPanel",
                Background = new SolidBrush(new Color(100, 100, 100)),
                Width = this.Width,
                Height = this.Height,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
      
            // Create back panel (data visualization side)
            _backPanel = new Panel()
            {
                Id = "backPanel",
                Background = new SolidBrush(new Color(60, 60, 60)),
                Width = this.Width,
                Height = this.Height,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
      
            // Setup data visualization
            SetupDataVisualization();
      
            // Add panels to this container
            this.Widgets.Add(_frontPanel);
            this.Widgets.Add(_backPanel);
      
            // Initial visibility based on face state
            UpdateVisibility();
        }
        
        protected virtual void SetupDataVisualization()
        {
            // Override in subclasses to visualize specific card data
        }
        
        protected void UpdateVisibility()
        {
            if (_isFaceUp)
            {
                _frontPanel.Visible = false;
                _backPanel.Visible = true;
            }
            else
            {
                _frontPanel.Visible = true;
                _backPanel.Visible = false;
            }
        }
        
        public void Flip()
        {
            _isFaceUp = !_isFaceUp;
            UpdateVisibility();
        }
        
        public void SetFaceUp(bool isFaceUp)
        {
            _isFaceUp = isFaceUp;
            UpdateVisibility();
        }
        
        public bool IsFaceUp => _isFaceUp;
    }
    
    /// <summary>
    /// Data class to hold common card information.
    /// </summary>
    public class CardData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        
        public CardData(string id, string name, string type)
        {
            Id = id;
            Name = name;
            Type = type;
        }
    }
}
