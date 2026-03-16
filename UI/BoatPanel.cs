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
    /// Dock section is at the top with dock.png background.
    /// Boat section is below with resource placeholders.
    /// </summary>
    public class BoatPanel : Panel
    {
        private readonly Desktop _desktop;
        private Player _player;
        private readonly Textures _textures;

        private Panel _dockPanel;
        public Panel MainBoatPanel;
        private Grid _dockGrid;
        private Grid _boatGrid;

        // Resource configuration
        // For boat: only stone, wood, fish, honey, fur (no coin, no tradeBoom)
        // For dock: all resources including coin and tradeBoom
        private static readonly List<string> ResourceOrder = new List<string>
        {
            "stone", "wood", "fish", "honey", "fur"
        };
        private static readonly List<string> DockResourceOrder = new List<string>
        {
            "stone", "wood", "fish", "honey", "fur", "tradeBoom", "coin"
        };

        private static readonly Dictionary<string, string> ResourceImageNames = new Dictionary<string, string>
        {
            {"wood", "wood"},
            {"stone", "stone"},
            {"fur", "beaver"},
            {"honey", "honey"},
            {"fish", "fish"},
            {"tradeBoom", "tradeboom"},
            {"coin", "coin"}
        };

        // Resource counts on boat (number of placeholders)
        private static readonly Dictionary<string, int> BoatResourceCounts = new Dictionary<string, int>
        {
            {"wood", 3},
            {"stone", 2},
            {"fur", 1},
            {"honey", 2},
            {"fish", 3},
            {"tradeBoom", 0}
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
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Set up grid columns: dock (left), boat (center), money (right)
            mainGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            mainGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            mainGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));

            // Set up grid rows: dock (top), boat (bottom)
            mainGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // dock (row 0)
            mainGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // boat (row 1)

            // Create dock panel
            _dockPanel = new Panel()
            {
                Id = "dockPanel",
                Background = new SolidBrush(new Color(60, 60, 60)),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Set dock background to dock.png texture
            SetDockBackground();

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
            // stone, wood, fish, honey, fur, tradeBoom, coin
            _dockGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // stone
            _dockGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // wood
            _dockGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // fish
            _dockGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // honey
            _dockGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // fur
            _dockGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // tradeBoom
            _dockGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // coin

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

            // Set boat background to boat.png texture
            SetBoatBackground();

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

            // Set up boat grid columns - one column per resource type
            // wood, stone, fur, honey, fish (no wild card column on boat)
            _boatGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // wood column
            _boatGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // stone column
            _boatGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // fur column
            _boatGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // honey column
            _boatGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // fish column

            // Set up boat grid rows: placeholders (row 0), count label (row 1), coin images (row 4), trade boom (row 5)
            _boatGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // placeholders row
            _boatGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // count label row
            _boatGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // empty row 2
            _boatGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // empty row 3
            _boatGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // coin images row
            _boatGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // trade boom row

            // Add boat resources
            AddBoatResources();

            // Add boat widgets to boat panel
            MainBoatPanel.Widgets.Add(_boatGrid);

            // Add all panels to main grid
            Grid.SetColumn(_dockPanel, 0);
            Grid.SetRow(_dockPanel, 0);
            mainGrid.Widgets.Add(_dockPanel);

            Grid.SetColumn(MainBoatPanel, 0);
            Grid.SetRow(MainBoatPanel, 1);
            mainGrid.Widgets.Add(MainBoatPanel);

            // Add main grid to this container
            this.Widgets.Add(mainGrid);
        }

        private void SetDockBackground()
        {
            var dockTexture = _textures.GetTexture("dock");
            if (dockTexture != null)
            {
                var textureRegion = new TextureRegion(dockTexture);
                var backgroundImage = new Image()
                {
                    Id = "dockBackground",
                    Renderable = textureRegion,
                    Width = 300,
                    Height = 150,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                _dockPanel.Background = new SolidBrush(new Color(60, 60, 60));
                // Add background image as first widget (behind other content)
                _dockPanel.Widgets.Insert(0, backgroundImage);
            }
        }

        private void SetBoatBackground()
        {
            var boatTexture = _textures.GetTexture("boat");
            if (boatTexture != null)
            {
                var textureRegion = new TextureRegion(boatTexture);
                var backgroundImage = new Image()
                {
                    Id = "boatBackground",
                    Renderable = textureRegion,
                    Width = 300,
                    Height = 200,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                MainBoatPanel.Background = new SolidBrush(new Color(60, 60, 60));
                // Add background image as first widget (behind other content)
                MainBoatPanel.Widgets.Insert(0, backgroundImage);
            }
        }

        private void AddDockResources()
        {
            int col = 0;
            foreach (var resource in DockResourceOrder)
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
                int count;
                if (resource == "coin")
                {
                    count = _player.boat.money;
                }
                else
                {
                    count = _player.boat.goodsOnDock.ContainsKey(resource) ? _player.boat.goodsOnDock[resource] : 0;
                }
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
        }

        private void AddBoatResources()
        {
            int col = 0;
        
            // Get coin texture for coin indicators
            var coinTexture = _textures.GetTexture("coin");
            // Get trade boom texture
            var tradeBoomTexture = _textures.GetTexture("tradeboom");

            foreach (var resource in ResourceOrder)
            {
                // Skip coin - coins are only shown on the dock
                if (resource == "coin")
                {
                    continue;
                }

                // Add placeholders for this resource (no heading image, just placeholders with resource images inside)
                int totalPlaceholders = BoatResourceCounts[resource];

                // Get resource image for the placeholders
                var imageName = ResourceImageNames[resource];
                var texture = _textures.GetTexture(imageName);

                // Create circular placeholder cells with resource image inside
                for (int i = 0; i < totalPlaceholders; i++)
                {
                    // Create a circular panel for the placeholder
                    var placeholderPanel = new Panel()
                    {
                        Id = $"boat_{resource}_placeholder_{i}",
                        Width = 35,
                        Height = 35,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Background = new SolidBrush(new Color(100, 100, 100, 100)), // Semi-transparent gray background
                    };

                    // Add resource image inside the placeholder
                    if (texture != null)
                    {
                        var textureRegion = new TextureRegion(texture);
                        var img = new Image()
                        {
                            Id = $"boat_{resource}_img_{i}",
                            Renderable = textureRegion,
                            Width = 25,
                            Height = 25,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                        };
                        Grid.SetColumn(img, col);
                        Grid.SetRow(img, i); // Same row as the placeholder
                        _boatGrid.Widgets.Add(img);
                    }

                    Grid.SetColumn(placeholderPanel, col);
                    Grid.SetRow(placeholderPanel, i); // Placeholders in rows within the column
                    _boatGrid.Widgets.Add(placeholderPanel);
                }

                // Add count label in row 1
                int count = _player.boat.goodsOnBoat.ContainsKey(resource) ? _player.boat.goodsOnBoat[resource] : 0;
                var countLabel = new Label()
                {
                    Id = $"boat_{resource}_count",
                    Text = count.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = new SolidBrush(Color.Transparent),
                };
                Grid.SetColumn(countLabel, col);
                Grid.SetRow(countLabel, 1); // Row 1
                _boatGrid.Widgets.Add(countLabel);

                // Add coin images in row 4
                if (coinTexture != null)
                {
                    var coinValue = (count == totalPlaceholders) ? "+1" : "0";
                    var textureRegion = new TextureRegion(coinTexture);
                    var coinImage = new Image()
                    {
                        Id = $"boat_{resource}_coinImage",
                        Renderable = textureRegion,
                        Width = 30,
                        Height = 30,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    Grid.SetColumn(coinImage, col);
                    Grid.SetRow(coinImage, 4); // Row 4 - coin images
                    _boatGrid.Widgets.Add(coinImage);

                    // Add coin count label inside the coin image
                    var coinCountLabel = new Label()
                    {
                        Id = $"boat_{resource}_coinCount",
                        Text = coinValue.ToString(),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Background = new SolidBrush(Color.Transparent),
                    };
                    Grid.SetColumn(coinCountLabel, col);
                    Grid.SetRow(coinCountLabel, 4); // Row 4 - coin images
                    _boatGrid.Widgets.Add(coinCountLabel);
                }

                // Add trade boom images in row 5 if tradeBoon[resource] > 0
                if (tradeBoomTexture != null && _player.boat.tradeBoon.ContainsKey(resource) && _player.boat.tradeBoon[resource] > 0)
                {
                    var textureRegion = new TextureRegion(tradeBoomTexture);
                    var tradeBoomImage = new Image()
                    {
                        Id = $"boat_{resource}_tradeBoomImage",
                        Renderable = textureRegion,
                        Width = 30,
                        Height = 30,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    Grid.SetColumn(tradeBoomImage, col);
                    Grid.SetRow(tradeBoomImage, 5); // Row 5 - trade boom images
                    _boatGrid.Widgets.Add(tradeBoomImage);
                }

                col++;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdatePanel()
        {
            Globals.Log("UpdatePanel(): enter");
            // Update dock resources
            int dockCol = 0;
            foreach (var resource in DockResourceOrder)
            {
                int count;
                if (resource == "coin")
                {
                    count = _player.boat.money;
                }
                else
                {
                    count = _player.boat.goodsOnDock.ContainsKey(resource) ? _player.boat.goodsOnDock[resource] : 0;
                }
                var countLabel = _dockGrid.Widgets.FirstOrDefault(w => w.Id == $"dock_{resource}Count") as Label;
                if (countLabel != null)
                {
                    countLabel.Text = count.ToString();
                }
                dockCol++;
            }

            // Update boat resources - cover placeholders based on count
            int col = 0;
            foreach (var resource in ResourceOrder)
            {
                // Skip coin - coins are only shown on the dock
                if (resource == "coin")
                {
                    col++;
                    continue;
                }

                int count = _player.boat.goodsOnBoat.ContainsKey(resource) ? _player.boat.goodsOnBoat[resource] : 0;
                int totalPlaceholders = BoatResourceCounts[resource];

                // Update placeholder visibility based on count
                for (int i = 0; i < totalPlaceholders; i++)
                {
                    // Find the placeholder panel (the circular background)
                    var placeholderPanel = _boatGrid.Widgets.FirstOrDefault(w => w.Id == $"boat_{resource}_placeholder_{i}") as Panel;

                    if (placeholderPanel != null)
                    {
                        // Cover placeholder if count > i (i.e., this slot is occupied)
                        if (i < count)
                        {
                            // Create a covered placeholder with a semi-transparent overlay
                            var coveredPlaceholder = new Panel()
                            {
                                Id = $"boat_{resource}_covered_{i}",
                                Width = 35,
                                Height = 35,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                                Background = new SolidBrush(new Color(128, 128, 128, 128)), // Semi-transparent gray
                            };
                            Grid.SetColumn(coveredPlaceholder, col);
                            Grid.SetRow(coveredPlaceholder, i); // Same row as the placeholder
                            _boatGrid.Widgets.Add(coveredPlaceholder);
                        }
                        else
                        {
                            // Remove any covered placeholder if it exists
                            var existingCovered = _boatGrid.Widgets.FirstOrDefault(w => w.Id == $"boat_{resource}_covered_{i}");
                            if (existingCovered != null)
                            {
                                _boatGrid.Widgets.Remove(existingCovered);
                            }
                        }
                    }
                }

                // Update count label
                var countLabel = _boatGrid.Widgets.FirstOrDefault(w => w.Id == $"boat_{resource}_count") as Label;
                if (countLabel != null)
                {
                    countLabel.Text = count.ToString();
                }

                // Update coin count label (+1 if column is full, 0 otherwise)
                string coinValue = (count == totalPlaceholders) ? "+1" : "0";
                var coinCountLabel = _boatGrid.Widgets.FirstOrDefault(w => w.Id == $"boat_{resource}_coinCount") as Label;
                if (coinCountLabel != null)
                {
                    coinCountLabel.Text = coinValue;
                }

                // Update trade boom image visibility based on tradeBoon[resource] > 0
                string tradeBoomImageId = $"boat_{resource}_tradeBoomImage";
                var existingTradeBoomImage = _boatGrid.Widgets.FirstOrDefault(w => w.Id == tradeBoomImageId) as Image;
                
                // Check if trade boom should be shown (wood, fish, honey only)
                bool shouldShowTradeBoom = (resource == "wood" || resource == "fish" || resource == "honey") 
                    && _player.boat.tradeBoon.ContainsKey(resource) 
                    && _player.boat.tradeBoon[resource] > 0;

                if (shouldShowTradeBoom)
                {
                    // Create trade boom image if it doesn't exist
                    if (existingTradeBoomImage == null)
                    {
                        var tradeBoomTexture = _textures.GetTexture("tradeboom");
                        if (tradeBoomTexture != null)
                        {
                            var textureRegion = new TextureRegion(tradeBoomTexture);
                            var tradeBoomImage = new Image()
                            {
                                Id = tradeBoomImageId,
                                Renderable = textureRegion,
                                Width = 30,
                                Height = 30,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                            };
                            Grid.SetColumn(tradeBoomImage, col);
                            Grid.SetRow(tradeBoomImage, 5); // Row 5 - trade boom images
                            _boatGrid.Widgets.Add(tradeBoomImage);
                        }
                    }
                }
                else
                {
                    // Remove trade boom image if it exists and shouldn't be shown
                    if (existingTradeBoomImage != null)
                    {
                        _boatGrid.Widgets.Remove(existingTradeBoomImage);
                    }
                }

                col++;
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
