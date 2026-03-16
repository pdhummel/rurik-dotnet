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
        private static readonly List<string> ResourceOrder = new List<string>
        {
            "wood", "stone", "fur", "honey", "fish", "coin"
        };

        private static readonly Dictionary<string, string> ResourceImageNames = new Dictionary<string, string>
        {
            {"wood", "wood"},
            {"stone", "stone"},
            {"fur", "beaver"},
            {"honey", "honey"},
            {"fish", "fish"},
            {"coin", "coin"}
        };

        // Resource counts on boat (number of placeholders)
        private static readonly Dictionary<string, int> BoatResourceCounts = new Dictionary<string, int>
        {
            {"wood", 3},
            {"stone", 2},
            {"fur", 1},
            {"honey", 2},
            {"fish", 3}
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

            // Set up boat grid columns (resource images and placeholders)
            _boatGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // resource image column
            _boatGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // placeholder 1
            _boatGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // placeholder 2
            _boatGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // placeholder 3
            _boatGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // placeholder 4
            _boatGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // placeholder 5

            // Set up boat grid rows - one row per resource type
            // wood, stone, fur, honey, fish, then wild card tokens
            _boatGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // wood row
            _boatGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // stone row
            _boatGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // fur row
            _boatGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // honey row
            _boatGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // fish row
            _boatGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // wild card row

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
            int row = 0;
            foreach (var resource in ResourceOrder)
            {
                // Skip coin - coins are only shown on the dock
                if (resource == "coin")
                {
                    continue;
                }

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
                    Grid.SetColumn(resourceImage, 0);
                    Grid.SetRow(resourceImage, row);
                    _boatGrid.Widgets.Add(resourceImage);
                }

                // Add placeholders for this resource
                int totalPlaceholders = BoatResourceCounts[resource];

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
                        Grid.SetColumn(img, i + 1);
                        Grid.SetRow(img, row);
                        _boatGrid.Widgets.Add(img);
                    }

                    Grid.SetColumn(placeholderPanel, i + 1);
                    Grid.SetRow(placeholderPanel, row);
                    _boatGrid.Widgets.Add(placeholderPanel);
                }

                row++;
            }

            // Add wild card boon/boom tokens below wood, fish, and honey
            // Wild card tokens are placed in a separate row below the resource rows
            int wildCardRow = row;
            var wildCardTexture = _textures.GetTexture("tradeboom");
            if (wildCardTexture != null)
            {
                var textureRegion = new TextureRegion(wildCardTexture);
                var wildCardImage = new Image()
                {
                    Id = "boat_wildCardImage",
                    Renderable = textureRegion,
                    Width = 30,
                    Height = 30,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Grid.SetColumn(wildCardImage, 0);
                Grid.SetRow(wildCardImage, wildCardRow);
                _boatGrid.Widgets.Add(wildCardImage);
            }

            // Add wild card count label
            int wildCardCount = _player.boat.goodsOnBoat.ContainsKey("tradeBoon") ? _player.boat.goodsOnBoat["tradeBoon"] : 0;
            var wildCardCountLabel = new Label()
            {
                Id = "boat_wildCardCount",
                Text = wildCardCount.ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
            Grid.SetColumn(wildCardCountLabel, 1);
            Grid.SetRow(wildCardCountLabel, wildCardRow);
            _boatGrid.Widgets.Add(wildCardCountLabel);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdatePanel()
        {
            Globals.Log("UpdatePanel(): enter");
            // Update dock resources
            int dockCol = 0;
            foreach (var resource in ResourceOrder)
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

            // Update tradeBoon count
            int tradeBoonCount = _player.boat.goodsOnDock.ContainsKey("tradeBoon") ? _player.boat.goodsOnDock["tradeBoon"] : 0;
            var tradeBoonCountLabel = _dockGrid.Widgets.FirstOrDefault(w => w.Id == "dock_tradeBoonCount") as Label;
            if (tradeBoonCountLabel != null)
            {
                tradeBoonCountLabel.Text = tradeBoonCount.ToString();
            }

            // Update boat resources - cover placeholders based on count
            int row = 0;
            foreach (var resource in ResourceOrder)
            {
                // Skip coin - coins are only shown on the dock
                if (resource == "coin")
                {
                    row++;
                    continue;
                }

                int count = _player.boat.goodsOnBoat.ContainsKey(resource) ? _player.boat.goodsOnBoat[resource] : 0;
                int totalPlaceholders = BoatResourceCounts[resource];

                // Update placeholder visibility based on count
                for (int i = 0; i < totalPlaceholders; i++)
                {
                    // Find the placeholder panel (the circular background)
                    var placeholderPanel = _boatGrid.Widgets.FirstOrDefault(w => w.Id == $"boat_{resource}_placeholder_{i}") as Panel;
                    // Find the resource image inside the placeholder
                    var resourceImg = _boatGrid.Widgets.FirstOrDefault(w => w.Id == $"boat_{resource}_img_{i}") as Image;

                    if (placeholderPanel != null && resourceImg != null)
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
                            Grid.SetColumn(coveredPlaceholder, i + 1);
                            Grid.SetRow(coveredPlaceholder, row);
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
                row++;
            }

            // Update wild card count
            int wildCardCount = _player.boat.goodsOnBoat.ContainsKey("tradeBoon") ? _player.boat.goodsOnBoat["tradeBoon"] : 0;
            var wildCardCountLabel = _boatGrid.Widgets.FirstOrDefault(w => w.Id == "boat_wildCardCount") as Label;
            if (wildCardCountLabel != null)
            {
                wildCardCountLabel.Text = wildCardCount.ToString();
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
