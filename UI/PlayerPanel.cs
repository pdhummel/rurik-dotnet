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
            _mainPanel = new Panel()
            {
                Id = "playerPanelMain",
                Background = new SolidBrush(new Color(80, 80, 80)),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Create main grid with columns for different sections
            _mainGrid = new Grid()
            {
                Id = "playerPanelMainGrid",
                Background = new SolidBrush(new Color(60, 60, 60)),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Set up grid columns: troops/leader (left), first player token (center-left), 
            // buildings (center), cards (right)
            _mainGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // troops/leader
            _mainGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // first player token
            _mainGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // buildings
            _mainGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // cards

            // Set up grid rows
            _mainGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // troops row
            _mainGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // leader row
            _mainGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // rebels row
            _mainGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // empty row
            _mainGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // buildings row
            _mainGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // cards row

            // Add troops section
            AddTroopsSection();

            // Add leader section
            AddLeaderSection();

            // Add first player token section
            AddFirstPlayerTokenSection();

            // Add captured rebels section
            AddCapturedRebelsSection();

            // Add buildings section
            AddBuildingsSection();

            // Add cards section
            AddCardsSection();

            // Add main grid to panel
            _mainPanel.Widgets.Add(_mainGrid);

            // Add panel to this container
            this.Widgets.Add(_mainPanel);
        }

        private void AddTroopsSection()
        {
            // Troops header
            var troopsHeader = new Label()
            {
                Id = "troopsHeader",
                Text = "SUPPLY",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.DarkGray),
            };
            Grid.SetColumn(troopsHeader, 0);
            Grid.SetRow(troopsHeader, 0);
            _mainGrid.Widgets.Add(troopsHeader);

            // Troops count
            var troopsTexture = _textures.GetTexture("rebel");
            if (troopsTexture != null)
            {
                var textureRegion = new TextureRegion(troopsTexture);
                var troopsImage = new Image()
                {
                    Id = "supplyTroopsImage",
                    Renderable = textureRegion,
                    Width = 30,
                    Height = 30,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Grid.SetColumn(troopsImage, 0);
                Grid.SetRow(troopsImage, 1);
                _mainGrid.Widgets.Add(troopsImage);
            }

            var troopsCountLabel = new Label()
            {
                Id = "supplyTroopsCount",
                Text = _player.supplyTroops.ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
            Grid.SetColumn(troopsCountLabel, 0);
            Grid.SetRow(troopsCountLabel, 1);
            _mainGrid.Widgets.Add(troopsCountLabel);
        }

        private void AddLeaderSection()
        {
            // Leader header
            var leaderHeader = new Label()
            {
                Id = "leaderHeader",
                Text = "LEADER",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.DarkGray),
            };
            Grid.SetColumn(leaderHeader, 0);
            Grid.SetRow(leaderHeader, 2);
            _mainGrid.Widgets.Add(leaderHeader);

            // Leader status (in supply or deployed)
            var leaderStatusLabel = new Label()
            {
                Id = "leaderStatus",
                Text = _player.supplyLeader > 0 ? "In Supply" : "Deployed",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
            Grid.SetColumn(leaderStatusLabel, 0);
            Grid.SetRow(leaderStatusLabel, 3);
            _mainGrid.Widgets.Add(leaderStatusLabel);
        }

        private void AddFirstPlayerTokenSection()
        {
            // First player token header
            var firstPlayerHeader = new Label()
            {
                Id = "firstPlayerHeader",
                Text = "FIRST",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.DarkGray),
            };
            Grid.SetColumn(firstPlayerHeader, 1);
            Grid.SetRow(firstPlayerHeader, 0);
            _mainGrid.Widgets.Add(firstPlayerHeader);

            // First player token (bear.png)
            var bearTexture = _textures.GetTexture("bear");
            if (bearTexture != null)
            {
                var textureRegion = new TextureRegion(bearTexture);
                var bearImage = new Image()
                {
                    Id = "firstPlayerToken",
                    Renderable = textureRegion,
                    Width = 40,
                    Height = 40,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Grid.SetColumn(bearImage, 1);
                Grid.SetRow(bearImage, 1);
                _mainGrid.Widgets.Add(bearImage);
            }

            // First player status label
            var firstPlayerStatusLabel = new Label()
            {
                Id = "firstPlayerStatus",
                Text = _player.isFirstPlayer ? "YES" : "NO",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
            Grid.SetColumn(firstPlayerStatusLabel, 1);
            Grid.SetRow(firstPlayerStatusLabel, 2);
            _mainGrid.Widgets.Add(firstPlayerStatusLabel);
        }

        private void AddCapturedRebelsSection()
        {
            // Captured rebels header
            var rebelsHeader = new Label()
            {
                Id = "rebelsHeader",
                Text = "REBELS",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.DarkGray),
            };
            Grid.SetColumn(rebelsHeader, 1);
            Grid.SetRow(rebelsHeader, 3);
            _mainGrid.Widgets.Add(rebelsHeader);

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
                    Height = 30,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Grid.SetColumn(rebelImage, 1);
                Grid.SetRow(rebelImage, 4);
                _mainGrid.Widgets.Add(rebelImage);
            }

            // Note: capturedRebels property is commented out in Player.cs
            // For now, we'll show 0 or you can add this property back to Player.cs
            var capturedRebelsCountLabel = new Label()
            {
                Id = "capturedRebelsCount",
                Text = "0", // _player.capturedRebels.ToString() if property is added
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
            Grid.SetColumn(capturedRebelsCountLabel, 1);
            Grid.SetRow(capturedRebelsCountLabel, 5);
            _mainGrid.Widgets.Add(capturedRebelsCountLabel);
        }

        private void AddBuildingsSection()
        {
            // Buildings header
            var buildingsHeader = new Label()
            {
                Id = "buildingsHeader",
                Text = "BUILDINGS",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.DarkGray),
            };
            Grid.SetColumn(buildingsHeader, 2);
            Grid.SetRow(buildingsHeader, 0);
            _mainGrid.Widgets.Add(buildingsHeader);

            // Add buildings in a grid layout
            int buildingCol = 0;
            foreach (var building in BuildingOrder)
            {
                // Get building count
                int count = _player.buildings.ContainsKey(building) ? _player.buildings[building] : 0;

                // Get building image
                var imageName = BuildingImageNames[building];
                var texture = _textures.GetTexture(imageName);
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
                    Grid.SetColumn(buildingImage, 2);
                    Grid.SetRow(buildingImage, 4);
                    _mainGrid.Widgets.Add(buildingImage);

                    // Add count label
                    var countLabel = new Label()
                    {
                        Id = $"building_{building}_count",
                        Text = count.ToString(),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Background = new SolidBrush(Color.Transparent),
                    };
                    Grid.SetColumn(countLabel, 2);
                    Grid.SetRow(countLabel, 5);
                    _mainGrid.Widgets.Add(countLabel);
                }

                buildingCol++;
            }
        }

        private void AddCardsSection()
        {
            // Cards header
            var cardsHeader = new Label()
            {
                Id = "cardsHeader",
                Text = "CARDS",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.DarkGray),
            };
            Grid.SetColumn(cardsHeader, 3);
            Grid.SetRow(cardsHeader, 0);
            _mainGrid.Widgets.Add(cardsHeader);

            // Deed cards
            AddCardType("DEED", "scheme-deedCard", _player.deedCards.Count, 1);

            // Scheme cards
            AddCardType("SCHEME", "scheme-card-back", _player.schemeCards.Count, 2);

            // Secret agenda cards
            AddCardType("SECRET", "scheme-card-back", _player.secretAgenda.Count, 3);
        }

        private void AddCardType(string cardTypeName, string imageName, int count, int row)
        {
            // Card type header
            var cardTypeLabel = new Label()
            {
                Id = $"cardType_{cardTypeName.ToLower()}",
                Text = cardTypeName,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
            Grid.SetColumn(cardTypeLabel, 3);
            Grid.SetRow(cardTypeLabel, row);
            _mainGrid.Widgets.Add(cardTypeLabel);

            // Card count
            var cardCountLabel = new Label()
            {
                Id = $"cardCount_{cardTypeName.ToLower()}",
                Text = count.ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
            Grid.SetColumn(cardCountLabel, 3);
            Grid.SetRow(cardCountLabel, row + 1);
            _mainGrid.Widgets.Add(cardCountLabel);

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
                Grid.SetColumn(cardImage, 3);
                Grid.SetRow(cardImage, row);
                _mainGrid.Widgets.Add(cardImage);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdatePanel()
        {
            // Update troops count
            var troopsCountLabel = _mainGrid.Widgets.FirstOrDefault(w => w.Id == "supplyTroopsCount") as Label;
            if (troopsCountLabel != null)
            {
                troopsCountLabel.Text = _player.supplyTroops.ToString();
            }

            // Update leader status
            var leaderStatusLabel = _mainGrid.Widgets.FirstOrDefault(w => w.Id == "leaderStatus") as Label;
            if (leaderStatusLabel != null)
            {
                leaderStatusLabel.Text = _player.supplyLeader > 0 ? "In Supply" : "Deployed";
            }

            // Update first player status
            var firstPlayerStatusLabel = _mainGrid.Widgets.FirstOrDefault(w => w.Id == "firstPlayerStatus") as Label;
            if (firstPlayerStatusLabel != null)
            {
                firstPlayerStatusLabel.Text = _player.isFirstPlayer ? "YES" : "NO";
            }

            // Update captured rebels count
            var capturedRebelsCountLabel = _mainGrid.Widgets.FirstOrDefault(w => w.Id == "capturedRebelsCount") as Label;
            if (capturedRebelsCountLabel != null)
            {
                // Note: capturedRebels property is commented out in Player.cs
                // capturedRebelsCountLabel.Text = _player.capturedRebels.ToString();
                capturedRebelsCountLabel.Text = "0";
            }

            // Update building counts
            foreach (var building in BuildingOrder)
            {
                int count = _player.buildings.ContainsKey(building) ? _player.buildings[building] : 0;
                var countLabel = _mainGrid.Widgets.FirstOrDefault(w => w.Id == $"building_{building}_count") as Label;
                if (countLabel != null)
                {
                    countLabel.Text = count.ToString();
                }
            }

            // Update card counts
            var deedCardCountLabel = _mainGrid.Widgets.FirstOrDefault(w => w.Id == "cardCount_deed") as Label;
            if (deedCardCountLabel != null)
            {
                deedCardCountLabel.Text = _player.deedCards.Count.ToString();
            }

            var schemeCardCountLabel = _mainGrid.Widgets.FirstOrDefault(w => w.Id == "cardCount_scheme") as Label;
            if (schemeCardCountLabel != null)
            {
                schemeCardCountLabel.Text = _player.schemeCards.Count.ToString();
            }

            var secretAgendaCardCountLabel = _mainGrid.Widgets.FirstOrDefault(w => w.Id == "cardCount_secret") as Label;
            if (secretAgendaCardCountLabel != null)
            {
                secretAgendaCardCountLabel.Text = _player.secretAgenda.Count.ToString();
            }
        }

        public void SetPlayer(Player player)
        {
            _player = player;
            UpdatePanel();
        }
    }
}
