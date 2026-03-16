using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using rurik;

namespace rurik.UI
{
    /// <summary>
    /// UI Panel for displaying player information including:
    /// - Supply troops count
    /// - Leader status (in supply or deployed)
    /// - Captured rebels count
    /// - First player token (bear.png)
    /// - Buildings in supply
    /// - Card counts (deed, scheme, secret agenda)
    /// </summary>
    public class PlayerPanel : Panel
    {

        private readonly Desktop _desktop;
        private Player _player;
        private readonly Textures _textures;

        private Panel _mainPanel;
        private Grid _mainGrid;

        private Panel _playerPanel;
        //private Grid _playerGrid;

        private BoatPanel _boatPanel;

        VerticalStackPanel verticalPanel = new VerticalStackPanel();
        HorizontalStackPanel horizontalPanel1 = new HorizontalStackPanel();
        HorizontalStackPanel horizontalPanel2 = new HorizontalStackPanel();
        //HorizontalStackPanel horizontalPanel3 = new HorizontalStackPanel();


        // Color mapping for factions
        private static readonly Dictionary<string, Color> DefaultFactionColors = new Dictionary<string, Color>
        {
            {"red", Color.Red},
            {"blue", Color.Blue},
            {"white", Color.White},
            {"yellow", Color.Yellow}
        };

        // Building configuration
        private static readonly List<string> BuildingOrder = new List<string>
        {
            "church", "stronghold", "market", "tavern", "stable"
        };

        private static readonly Dictionary<string, string> BuildingImageNames = new Dictionary<string, string>
        {
            {"church", "church"},
            {"stronghold", "stronghold"},
            {"market", "market"},
            {"tavern", "tavern"},
            {"stable", "stable"}
        };

        public PlayerPanel(Desktop desktop, Player player, Textures textures)
            : base()
        {
            _desktop = desktop;
            _player = player;
            _textures = textures;

            Initialize();
        }

        private void Initialize()
        {
            // Set base panel properties
            this.Id = "playerPanel";
            this.HorizontalAlignment = HorizontalAlignment.Center;
            this.VerticalAlignment = VerticalAlignment.Center;

            // Create main container grid
            _playerPanel = new Panel()
            {
                Id = "playerPanelMain",
                Background = new SolidBrush(new Color(80, 80, 80)),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            verticalPanel.Widgets.Add(horizontalPanel1);
            verticalPanel.Widgets.Add(horizontalPanel2);
            //verticalPanel.Widgets.Add(horizontalPanel3);

            // Add main grid to panel
            _playerPanel.Widgets.Add(verticalPanel);

            // Add panel to this container

            _mainPanel = new Panel();
            _mainGrid = new Grid();
            _mainPanel.Widgets.Add(_mainGrid);
            _mainGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // player panel
            _mainGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // boat panel
            this.Widgets.Add(_mainPanel);
        }

        private void AddTroopsSection()
        {

            // Troops count
            var troopsTexture = _textures.GetTexture("troop-" + _player.Color);
            if (troopsTexture != null)
            {
                var textureRegion = new TextureRegion(troopsTexture);
                var troopsImage = new Image()
                {
                    Id = "supplyTroopsImage",
                    Renderable = textureRegion,
                    Width = 30,
                    Height = 40,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                horizontalPanel1.Widgets.Add(troopsImage);
            }

        }


        private void AddLeaderSection()
        {
            var leaderTexture = _textures.GetTexture("rurik-leader");
            if (leaderTexture != null)
            {
                var textureRegion = new TextureRegion(leaderTexture);
                var leaderImage = new Image()
                {
                    Id = "supplyLeaderImage",
                    Renderable = textureRegion,
                    Width = 30,
                    Height = 40,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                horizontalPanel1.Widgets.Add(leaderImage);
            }

        }

        private void AddFirstPlayerTokenSection()
        {

            // First player token (bear.png)
            var bearTexture = _textures.GetTexture("bear");
            if (bearTexture != null)
            {
                var textureRegion = new TextureRegion(bearTexture);
                var bearImage = new Image()
                {
                    Id = "firstPlayerToken",
                    Renderable = textureRegion,
                    Width = 50,
                    Height = 30,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                horizontalPanel1.Widgets.Add(bearImage);
            }

        }

        private void AddCapturedRebelsSection()
        {

            // Captured rebels count
            var rebelTexture = _textures.GetTexture("rebel");
            if (rebelTexture != null)
            {
                var textureRegion = new TextureRegion(rebelTexture);
                var rebelImage = new Image()
                {
                    Id = "capturedRebelsImage",
                    Renderable = textureRegion,
                    Width = 30,
                    Height = 40,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                horizontalPanel1.Widgets.Add(rebelImage);
            }

       }

        private void AddBuildingsSection()
        {

            // Add buildings in a grid layout
            int buildingCol = 0;
            foreach (var building in BuildingOrder)
            {
                // Get building count
                int count = _player.buildings.ContainsKey(building) ? _player.buildings[building] : 0;

                // Get building image
                var imageName = BuildingImageNames[building];
                var texture = _textures.GetTexture(imageName + "-" + _player.Color);
                if (texture != null)
                {
                    // Create a panel for this building type
                    var buildingPanel = new Panel()
                    {
                        Id = $"building_{building}",
                        Width = 50,
                        Height = 50,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Background = new SolidBrush(new Color(100, 100, 100, 100)),
                    };

                    // Add building image
                    var textureRegion = new TextureRegion(texture);
                    var buildingImage = new Image()
                    {
                        Id = $"building_{building}_image",
                        Renderable = textureRegion,
                        Width = 35,
                        Height = 35,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    horizontalPanel2.Widgets.Add(buildingImage);

                }

                buildingCol++;
            }
        }

        private void AddCardsSection()
        {
            // Deed cards
            AddCardType("DEED", "scheme-deedCard", _player.deedCards.Count, 1);

            // Scheme cards
            AddCardType("SCHEME", "scheme-card-back", _player.schemeCards.Count, 2);

            // Secret agenda cards
            AddCardType("SECRET", "scheme-card-back", _player.secretAgenda.Count, 3);
        }

        private void AddCardType(string cardTypeName, string imageName, int count, int row)
        {

            // Card count
            var cardCountLabel = new Label()
            {
                Id = $"cardCount_{cardTypeName.ToLower()}",
                Text = count.ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
            //Grid.SetColumn(cardCountLabel, 3);
            //Grid.SetRow(cardCountLabel, row + 1);
            //_playerGrid.Widgets.Add(cardCountLabel);

            // Card image (if available)
            var texture = _textures.GetTexture(imageName);
            if (texture != null)
            {
                var textureRegion = new TextureRegion(texture);
                var cardImage = new Image()
                {
                    Id = $"cardImage_{cardTypeName.ToLower()}",
                    Renderable = textureRegion,
                    Width = 30,
                    Height = 30,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                //Grid.SetColumn(cardImage, 3);
                //Grid.SetRow(cardImage, row);
                //_playerGrid.Widgets.Add(cardImage);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdatePanel()
        {

            horizontalPanel1.Widgets.Clear();
            AddFirstPlayerTokenSection();
            AddLeaderSection();
            AddTroopsSection();
            AddCapturedRebelsSection();
            horizontalPanel2.Widgets.Clear();
            AddBuildingsSection();

            _mainGrid.Widgets.Clear();
            _boatPanel = new BoatPanel(_desktop, _player, _textures);
            _boatPanel.UpdatePanel();
            Grid.SetRow(_playerPanel, 0);
            _mainGrid.Widgets.Add(_playerPanel);
            Grid.SetRow(_boatPanel, 1);
            _mainGrid.Widgets.Add(_boatPanel);

        }

        public void SetPlayer(Player player)
        {
            _player = player;
            UpdatePanel();
        }
    }
}
