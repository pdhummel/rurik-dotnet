using Myra.Graphics2D.UI;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace rurik.UI
{
    public class LocationItemsPanel
    {
        private Panel _panel;
        private Grid _mainGrid;
        private Panel _leftColumnPanel;
        private Panel _rightColumnPanel;
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

        public LocationItemsPanel(Location location)
        {
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
                Width = 150,
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
                Padding = new Thickness(5),
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
            Grid leftColumnGrid = new Grid()
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
            //leftColumnGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            //leftColumnGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));

            // Add colored squares (16x16px) in the first column of the 2x4 grid
            // Row 0, Column 0
            Panel square0 = CreateColoredSquare(FactionColors["red"]);
            Grid.SetRow(square0, 0);
            Grid.SetColumn(square0, 0);
            leftColumnGrid.Widgets.Add(square0);

            // Row 1, Column 0
            Panel square1 = CreateColoredSquare(FactionColors["blue"]);
            Grid.SetRow(square1, 1);
            Grid.SetColumn(square1, 0);
            leftColumnGrid.Widgets.Add(square1);

            Panel square2 = CreateColoredSquare(FactionColors["white"]);
            Grid.SetRow(square2, 2);
            Grid.SetColumn(square2, 0);
            leftColumnGrid.Widgets.Add(square2);

            Panel square3 = CreateColoredSquare(FactionColors["yellow"]);
            Grid.SetRow(square3, 3);
            Grid.SetColumn(square3, 0);
            leftColumnGrid.Widgets.Add(square3);

            // Add colored circles (16px diameter) in the second column of the 2x4 grid
            // Row 0, Column 1
            Panel circle0 = CreateColoredCircle(FactionColors["red"]);
            Grid.SetRow(circle0, 0);
            Grid.SetColumn(circle0, 1);
            leftColumnGrid.Widgets.Add(circle0);

            // Row 1, Column 1
            Panel circle1 = CreateColoredCircle(FactionColors["blue"]);
            Grid.SetRow(circle1, 1);
            Grid.SetColumn(circle1, 1);
            leftColumnGrid.Widgets.Add(circle1);

            Panel circle2 = CreateColoredCircle(FactionColors["white"]);
            Grid.SetRow(circle2, 2);
            Grid.SetColumn(circle2, 1);
            leftColumnGrid.Widgets.Add(circle2);


            Panel circle3 = CreateColoredCircle(FactionColors["yellow"]);
            Grid.SetRow(circle3, 3);
            Grid.SetColumn(circle3, 1);
            leftColumnGrid.Widgets.Add(circle3);

            // Add left column grid to left column panel
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
            Grid rightColumnGrid = new Grid()
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

            // Add three 50x50px spaces
            Panel space0 = CreateBuildingSpace();
            Grid.SetRow(space0, 0);
            Grid.SetColumn(space0, 0);
            rightColumnGrid.Widgets.Add(space0);

            Panel space1 = CreateBuildingSpace();
            Grid.SetRow(space1, 1);
            Grid.SetColumn(space1, 0);
            rightColumnGrid.Widgets.Add(space1);

            Panel space2 = CreateBuildingSpace();
            Grid.SetRow(space2, 2);
            Grid.SetColumn(space2, 0);
            rightColumnGrid.Widgets.Add(space2);

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

        private Panel CreateColoredSquare(Color color)
        {
            return new Panel()
            {
                Id = $"coloredSquare_{GetColorName(color)}",
                Background = new SolidBrush(color with { A = 128 }),
                Width = 20,
                Height = 20,
                Margin = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
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

        private Panel CreateBuildingSpace()
        {
            return new Panel()
            {
                Id = "buildingSpace",
                Background = new SolidBrush(Color.Black with { A = 128 }),
                Width = 25,
                Height = 25,
                Margin = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
        }
    }
}
