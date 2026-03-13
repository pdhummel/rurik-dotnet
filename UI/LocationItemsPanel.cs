using Myra.Graphics2D.UI;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace rurik.UI
{
    public class LocationItemsPanel
    {
        private Panel _panel;
        private Grid _mainGrid;
        private Panel _leftColumnPanel;
        private Panel _rightColumnPanel;
        private RurikMonoGame _rurikMonoGame;
        public Location Location {get;set;}
        
        // Color mapping for factions
        private static readonly Dictionary<string, Color> FactionColors = new Dictionary<string, Color>
        {
            {"red", Color.Red},
            {"blue", Color.Blue},
            {"white", Color.White},
            {"yellow", Color.Yellow}
        };

        public Window Window {get; set;} = new Window();
        public Panel Panel => _panel;
        public Panel LeftColumnPanel => _leftColumnPanel;
        public Panel RightColumnPanel => _rightColumnPanel;

        private Panel[] troopPanels = new Panel[4];
        private Panel[] leaderPanels = new Panel[4];
        private Panel[] buildingPanels = new Panel[3];
        private Grid leftColumnGrid;
        private Grid rightColumnGrid;

        public LocationItemsPanel(RurikMonoGame rurikMonoGame, Location location)
        {
            _rurikMonoGame = rurikMonoGame;
            Location = location;
            Initialize();
        }

        private void Initialize()
        {
            Window.Title = Location.name;
            // Main panel
            _panel = new Panel()
            {
                Id = "locationItemsPanel",
                Background = new SolidBrush(Color.Transparent),
                Width = 75,
                Height = 100,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };
            Window.Content = _panel;

            // Main grid with 2 columns
            _mainGrid = new Grid()
            {
                Id = "locationItemsGrid",
                Background = new SolidBrush(Color.Transparent),
                Padding = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Set up grid columns: left column (2x4 grid) and right column (3 spaces)
            _mainGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _mainGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));

            // Set up grid rows for left column (2 rows of 4 items each)
            _mainGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            _mainGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            // Create left column panel (contains 2x4 grid)
            _leftColumnPanel = new Panel()
            {
                Id = "leftColumnPanel",
                Background = new SolidBrush(Color.Transparent),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Left column grid: 2 rows x 4 columns
            leftColumnGrid = new Grid()
            {
                Id = "leftColumnGrid",
                Background = new SolidBrush(Color.Transparent),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Set up left column grid: 4 rows, 2 columns
            leftColumnGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            leftColumnGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            leftColumnGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            leftColumnGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            leftColumnGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            leftColumnGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _leftColumnPanel.Widgets.Add(leftColumnGrid);


            // Create right column panel (contains 3 vertical 50x50px spaces)
            _rightColumnPanel = new Panel()
            {
                Id = "rightColumnPanel",
                Background = new SolidBrush(Color.Transparent),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Right column grid: 3 rows, 1 column
            rightColumnGrid = new Grid()
            {
                Id = "rightColumnGrid",
                Background = new SolidBrush(Color.Transparent),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            // Set up right column grid: 3 rows
            rightColumnGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            rightColumnGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            rightColumnGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            rightColumnGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _rightColumnPanel.Widgets.Add(rightColumnGrid);

            UpdateLocation();

            // Add right column grid to right column panel
            _rightColumnPanel.Widgets.Add(rightColumnGrid);

            // Add left and right column panels to main grid
            Grid.SetColumn(_leftColumnPanel, 0);
            Grid.SetRow(_leftColumnPanel, 0);
            _mainGrid.Widgets.Add(_leftColumnPanel);

            Grid.SetColumn(_rightColumnPanel, 1);
            Grid.SetRow(_rightColumnPanel, 0);
            _mainGrid.Widgets.Add(_rightColumnPanel);

            // Add main grid to panel
            _panel.Widgets.Add(_mainGrid);
        }

        private string GetColorName(Color color)
        {
            // Find the color name from the FactionColors dictionary
            foreach (var kvp in FactionColors)
            {
                if (kvp.Value == color)
                {
                    return kvp.Key;
                }
            }
            return "unknown";
        }


        public void UpdateLocation(Location location)
        {
            Location = location;
            UpdateLocation();
        }

        public void UpdateLocation()
        {
            leftColumnGrid.Widgets.Clear();
            int index = 0;
            foreach (string color in FactionColors.Keys)
            {
                int troops = Location.troopsByColor[color];
                int leader = Location.leaderByColor[color];
                if (troops > 0 || leader > 0)
                {
                    if (troops > 0)
                    {
                        //lobals.Log("UpdateLocation(): " + index + " " + color + " " + troops);
                        Panel square = CreateColoredSquare(FactionColors[color], troops);
                        troopPanels[index] = square;
                        Grid.SetRow(square, index);
                        Grid.SetColumn(square, 0);
                        leftColumnGrid.Widgets.Add(square);
                    }
                    if (leader > 0)
                    {
                        Panel circle = CreateColoredCircle(FactionColors[color]);
                        leaderPanels[index] = circle;
                        Grid.SetRow(circle, index);
                        Grid.SetColumn(circle, 1);
                        leftColumnGrid.Widgets.Add(circle);
                    }
                    index += 1;
                }
            }

            rightColumnGrid.Widgets.Clear();
            index = 0;
            //Location.buildings.Clear();
            //Building testBuilding = new Building("yellow", "stronghold");
            //Location.buildings.Add(testBuilding);
            //Location.buildings.Add(testBuilding);
            //Location.buildings.Add(testBuilding);
            List<Building> buildingList = new List<Building>(Location.buildings);
            foreach (Building building in buildingList)
            {
                if (building == null)
                    return;
                Panel buildingSpace = CreateBuildingSpace(building.color, building.name);
                if (index < 3 && index >= 0)
                {
                    buildingPanels[index] = buildingSpace;
                    Grid.SetRow(buildingSpace, index);
                    Grid.SetColumn(buildingSpace, 0);
                    rightColumnGrid.Widgets.Add(buildingSpace);
                    index += 1;
                }    
            }

        }

        private Panel CreateColoredSquare(Color color, int troops = 0)
        {
            var panel = new Panel()
            {
                Id = $"coloredSquare_{GetColorName(color)}",
                Background = new SolidBrush(color with { A = 128 }),
                Width = 20,
                Height = 20,
                Margin = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            
            // Add troop count label in black foreground
            if (troops > 0)
            {
                var label = new Label()
                {
                    Text = troops.ToString(),
                    TextColor =  Color.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                panel.Widgets.Add(label);
            }
            
            return panel;
        }

        private Panel CreateColoredCircle(Color color)
        {
            // Create a panel with a circular shape using a border
            var panel = new Panel()
            {
                Id = $"coloredCircle_{GetColorName(color)}",
                Background = new SolidBrush(color with { A = 128 }),
                Width = 20,
                Height = 20,
                Margin = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var label = new Label()
            {
                Text = "L",
                TextColor =  Color.Black,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            panel.Widgets.Add(label);


            return panel;
        }

        private Panel CreateEmptyCell()
        {
            return new Panel()
            {
                Id = "emptyCell",
                Background = new SolidBrush(Color.Transparent),
                Width = 20,
                Height = 20,
                Margin = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
        }

        private Panel CreateBuildingSpace(string color, string name)
        {
            Texture2D texture = _rurikMonoGame.Textures.GetTexture(name + "-" + color);
            var textureRegion = new Myra.Graphics2D.TextureAtlases.TextureRegion(texture);
            var buildingImage = new Image()
            {
                Renderable = textureRegion,
                Width = 30,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            
            var panel = new Panel()
            {
                Id = "buildingSpace",
                Background = new SolidBrush(Color.Transparent),
                Width = 30,
                Height = 30,
                Margin = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            panel.Widgets.Add(buildingImage);
            return panel;
        }


    }

}
