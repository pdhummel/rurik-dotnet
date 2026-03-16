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

namespace rurik.UI
{
    /// <summary>
    /// UI Panel for displaying the Boat and Supply resources.
    /// Shows resources on the dock, resources on the boat, unclaimed bonus wildcard resources (tradeBoon),
    /// and the player's money/coins.
    /// </summary>
    public class BoatPanel : Panel
    {
        private readonly Desktop _desktop;
        private Player _player;
        private readonly Textures _textures;

        private Panel _dockPanel;
        public Panel MainBoatPanel;
        private Panel _moneyPanel;
        private Grid _dockGrid;
        private Grid _boatGrid;

        // Resource configuration
        private static readonly List<string> ResourceOrder = new List<string>
        {
            "wood", "stone", "fur", "honey", "fish"
        };

        private static readonly Dictionary<string, string> ResourceImageNames = new Dictionary<string, string>
        {
            {"wood", "wood"},
            {"stone", "stone"},
            {"fur", "beaver"},
            {"honey", "honey"},
            {"fish", "fish"}
        };

        public BoatPanel(Desktop desktop, Player player, Textures textures)
            : base()
        {
            _desktop = desktop;
            _player = player;
            _textures = textures;

            Initialize();
        }

        private void Initialize()
        {
            Globals.Log("Initialize(): enter");
            // Set base panel properties
            this.Id = "boatPanel";
            this.HorizontalAlignment = HorizontalAlignment.Center;
            this.VerticalAlignment = VerticalAlignment.Center;

            // Create main container grid
            var mainGrid = new Grid()
            {
                Id = "boatPanelMainGrid",
                Background = new SolidBrush(new Color(80, 80, 80)),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Set up grid columns: dock (left), boat (center), money (right)
            mainGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            mainGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            mainGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));

            // Set up grid rows
            mainGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            mainGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            // Create dock panel
            _dockPanel = new Panel()
            {
                Id = "dockPanel",
                Background = new SolidBrush(new Color(60, 60, 60)),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Create dock header
            var dockHeader = new Label()
            {
                Id = "dockHeader",
                Text = "DOCK",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.DarkGray),
            };

            // Create dock grid for resources
            _dockGrid = new Grid()
            {
                Id = "dockGrid",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Set up dock grid columns (resource images and counts)
            _dockGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _dockGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _dockGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _dockGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _dockGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _dockGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // tradeBoon column

            // Set up dock grid rows
            _dockGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // resource row
            _dockGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // count row

            // Add dock resources
            AddDockResources();

            // Add dock widgets to dock panel
            _dockPanel.Widgets.Add(_dockGrid);

            // Create boat panel
            MainBoatPanel = new Panel()
            {
                Id = "boatPanelContent",
                Background = new SolidBrush(new Color(60, 60, 60)),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            

            // Create boat header
            var boatHeader = new Label()
            {
                Id = "boatHeader",
                Text = "BOAT",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.DarkGray),
            };

            // Create boat grid for resources
            _boatGrid = new Grid()
            {
                Id = "boatGrid",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Set up boat grid columns (resource images and counts)
            _boatGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _boatGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _boatGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _boatGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _boatGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));

            // Set up boat grid rows
            _boatGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // resource row
            _boatGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // count row

            // Add boat resources
            AddBoatResources();

            // Add boat widgets to boat panel
            MainBoatPanel.Widgets.Add(_boatGrid);

            // Create money panel
            _moneyPanel = new Panel()
            {
                Id = "moneyPanel",
                Background = new SolidBrush(new Color(60, 60, 60)),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Create money display
            var moneyLabel = new Label()
            {
                Id = "moneyLabel",
                Text = "MONEY",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.DarkGray),
            };

            var coinTexture = _textures.GetTexture("coin");
            Image coinImage = null;
            if (coinTexture != null)
            {
                var textureRegion = new TextureRegion(coinTexture);
                coinImage = new Image()
                {
                    Id = "coinImage",
                    Renderable = textureRegion,
                    Width = 40,
                    Height = 40,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
            }

            var playerMoneyLabel = new Label()
            {
                Id = "playerMoneyLabel",
                Text = _player.boat.money.ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };

            // Add money widgets to money panel
            if (coinImage != null)
            {
                var moneyGrid = new Grid();
                moneyGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
                moneyGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
                Grid.SetColumn(coinImage, 0);
                Grid.SetRow(coinImage, 0);
                moneyGrid.Widgets.Add(coinImage);
                Grid.SetColumn(playerMoneyLabel, 1);
                Grid.SetRow(playerMoneyLabel, 0);
                moneyGrid.Widgets.Add(playerMoneyLabel);
                _moneyPanel.Widgets.Add(moneyGrid);
            }
            else
            {
                _moneyPanel.Widgets.Add(playerMoneyLabel);
            }

            // Add all panels to main grid
            Grid.SetColumn(_dockPanel, 0);
            Grid.SetRow(_dockPanel, 0);
            mainGrid.Widgets.Add(_dockPanel);

            Grid.SetColumn(MainBoatPanel, 1);
            Grid.SetRow(MainBoatPanel, 0);
            mainGrid.Widgets.Add(MainBoatPanel);

            Grid.SetColumn(_moneyPanel, 2);
            Grid.SetRow(_moneyPanel, 0);
            mainGrid.Widgets.Add(_moneyPanel);

            // Add main grid to this container
            this.Widgets.Add(mainGrid);
        }

        private void AddDockResources()
        {
            int col = 0;
            foreach (var resource in ResourceOrder)
            {
                // Get resource image
                var imageName = ResourceImageNames[resource];
                var texture = _textures.GetTexture(imageName);
                Image resourceImage = null;
                if (texture != null)
                {
                    var textureRegion = new TextureRegion(texture);
                    resourceImage = new Image()
                    {
                        Id = $"dock_{resource}Image",
                        Renderable = textureRegion,
                        Width = 30,
                        Height = 30,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    Grid.SetColumn(resourceImage, col);
                    Grid.SetRow(resourceImage, 0);
                    _dockGrid.Widgets.Add(resourceImage);
                }

                // Get resource count
                int count = _player.boat.goodsOnDock.ContainsKey(resource) ? _player.boat.goodsOnDock[resource] : 0;
                var countLabel = new Label()
                {
                    Id = $"dock_{resource}Count",
                    Text = count.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = new SolidBrush(Color.Transparent),
                };
                Grid.SetColumn(countLabel, col);
                Grid.SetRow(countLabel, 1);
                _dockGrid.Widgets.Add(countLabel);

                col++;
            }

            // Add tradeBoon (wildcard bonus) resource
            var tradeBoonTexture = _textures.GetTexture("tradeboom");
            Image tradeBoonImage = null;
            if (tradeBoonTexture != null)
            {
                var textureRegion = new TextureRegion(tradeBoonTexture);
                tradeBoonImage = new Image()
                {
                    Id = "dock_tradeBoonImage",
                    Renderable = textureRegion,
                    Width = 30,
                    Height = 30,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Grid.SetColumn(tradeBoonImage, col);
                Grid.SetRow(tradeBoonImage, 0);
                _dockGrid.Widgets.Add(tradeBoonImage);
            }

            int tradeBoonCount = _player.boat.goodsOnDock.ContainsKey("tradeBoon") ? _player.boat.goodsOnDock["tradeBoon"] : 0;
            var tradeBoonCountLabel = new Label()
            {
                Id = "dock_tradeBoonCount",
                Text = tradeBoonCount.ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
            Grid.SetColumn(tradeBoonCountLabel, col);
            Grid.SetRow(tradeBoonCountLabel, 1);
            _dockGrid.Widgets.Add(tradeBoonCountLabel);
        }

        private void AddBoatResources()
        {
            int col = 0;
            foreach (var resource in ResourceOrder)
            {
                // Get resource image
                var imageName = ResourceImageNames[resource];
                var texture = _textures.GetTexture(imageName);
                Image resourceImage = null;
                if (texture != null)
                {
                    var textureRegion = new TextureRegion(texture);
                    resourceImage = new Image()
                    {
                        Id = $"boat_{resource}Image",
                        Renderable = textureRegion,
                        Width = 30,
                        Height = 30,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    Grid.SetColumn(resourceImage, col);
                    Grid.SetRow(resourceImage, 0);
                    _boatGrid.Widgets.Add(resourceImage);
                }

                // Get resource count
                int count = _player.boat.goodsOnBoat.ContainsKey(resource) ? _player.boat.goodsOnBoat[resource] : 0;
                var countLabel = new Label()
                {
                    Id = $"boat_{resource}Count",
                    Text = count.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = new SolidBrush(Color.Transparent),
                };
                Grid.SetColumn(countLabel, col);
                Grid.SetRow(countLabel, 1);
                _boatGrid.Widgets.Add(countLabel);

                col++;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdatePanel()
        {
            Globals.Log("UpdatePanel(): enter");
            // Update dock resources
            int dockCol = 0;
            foreach (var resource in ResourceOrder)
            {
                int count = _player.boat.goodsOnDock.ContainsKey(resource) ? _player.boat.goodsOnDock[resource] : 0;
                var countLabel = _dockGrid.Widgets.FirstOrDefault(w => w.Id == $"dock_{resource}Count") as Label;
                if (countLabel != null)
                {
                    countLabel.Text = count.ToString();
                }
                dockCol++;
            }

            // Update tradeBoon count
            int tradeBoonCount = _player.boat.goodsOnDock.ContainsKey("tradeBoon") ? _player.boat.goodsOnDock["tradeBoon"] : 0;
            var tradeBoonCountLabel = _dockGrid.Widgets.FirstOrDefault(w => w.Id == "dock_tradeBoonCount") as Label;
            if (tradeBoonCountLabel != null)
            {
                tradeBoonCountLabel.Text = tradeBoonCount.ToString();
            }

            // Update boat resources
            int boatCol = 0;
            foreach (var resource in ResourceOrder)
            {
                int count = _player.boat.goodsOnBoat.ContainsKey(resource) ? _player.boat.goodsOnBoat[resource] : 0;
                var countLabel = _boatGrid.Widgets.FirstOrDefault(w => w.Id == $"boat_{resource}Count") as Label;
                if (countLabel != null)
                {
                    countLabel.Text = count.ToString();
                }
                boatCol++;
            }

            // Update money
            var playerMoneyLabel = _moneyPanel.Widgets.FirstOrDefault(w => w.Id == "playerMoneyLabel") as Label;
            if (playerMoneyLabel != null)
            {
                playerMoneyLabel.Text = _player.boat.money.ToString();
            }
        }

        public void SetPlayer(Player player)
        {
            Globals.Log("SetPlayer(): enter");
            _player = player;
            UpdatePanel();
        }
    }
}
