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

namespace rurik.UI
{
    /// <summary>
    /// UI Panel for displaying the Advisor Board (auction board).
    /// Shows the 6 columns with their rows and advisor placements.
    /// </summary>
    public class AdvisorBoardPanel : Panel
    {
        private readonly Desktop _desktop;
        private AuctionBoard _auctionBoard;
        private readonly Dictionary<string, Color> _factionColors;
        private readonly Dictionary<int, string> _advisorNumToText;
        private readonly Textures _textures;

        private Panel _boardPanel;
        private Grid _boardGrid;

        // Color mapping for factions
        private static readonly Dictionary<string, Color> DefaultFactionColors = new Dictionary<string, Color>
        {
            {"red", Color.Red},
            {"blue", Color.Blue},
            {"white", Color.White},
            {"yellow", Color.Yellow}
        };

        public AdvisorBoardPanel(Desktop desktop, AuctionBoard auctionBoard, Textures textures)
            : base()
        {
            _desktop = desktop;
            _auctionBoard = auctionBoard;
            _factionColors = DefaultFactionColors;
            _textures = textures;
      
            // Map advisor numbers to text for image naming
            _advisorNumToText = new Dictionary<int, string>
            {
                {1, "one"},
                {2, "two"},
                {3, "three"},
                {4, "four"},
                {5, "five"}
            };

            Initialize();
        }

        private void Initialize()
        {
            // Set base panel properties
            this.Id = "advisorBoardPanel";
            this.HorizontalAlignment = HorizontalAlignment.Center;
            this.VerticalAlignment = VerticalAlignment.Center;

            // Create the board panel
            _boardPanel = new Panel()
            {
                Id = "boardPanel",
                Background = new SolidBrush(new Color(50, 50, 50)),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Create the grid for the board
            _boardGrid = new Grid()
            {
                Id = "boardGrid",
                Background = new SolidBrush(new Color(80, 80, 80)),
                Padding = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Set up grid columns (6 action columns)
            _boardGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _boardGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _boardGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _boardGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _boardGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            _boardGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));

            // Set up grid rows (3 or 4 rows depending on player count)
            for (int i = 0; i < _auctionBoard.numberOfRows; i++)
            {
                _boardGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            }

            // Add column headers
            string[] columnNames = { "muster", "move", "attack", "tax", "build", "scheme" };
            for (int col = 0; col < columnNames.Length; col++)
            {
                var headerLabel = new Label()
                {
                    Id = $"header_{columnNames[col]}",
                    Text = columnNames[col].ToUpper(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = new SolidBrush(Color.DarkGray),
                };
                Grid.SetColumn(headerLabel, col);
                Grid.SetRow(headerLabel, 0);
                _boardGrid.Widgets.Add(headerLabel);
            }

            // Add rows for each column
            for (int row = 0; row < _auctionBoard.numberOfRows; row++)
            {
                for (int col = 0; col < columnNames.Length; col++)
                {
                    var actionName = columnNames[col];
                    var auctionSpaces = _auctionBoard.Board[actionName];
                    
                    if (row < auctionSpaces.Count)
                    {
                        var space = auctionSpaces[row];
                        var cellPanel = CreateCellPanel(space, row, col);
                        Grid.SetColumn(cellPanel, col);
                        Grid.SetRow(cellPanel, row + 1); // +1 for header row
                        _boardGrid.Widgets.Add(cellPanel);
                    }
                }
            }

            // Add board grid to board panel
            _boardPanel.Widgets.Add(_boardGrid);

            // Add board panel to this container
            this.Widgets.Add(_boardPanel);
        }

        private Panel CreateCellPanel(AuctionSpace space, int row, int col)
        {
            var cellPanel = new Panel()
            {
                Id = $"cell_{space.actionName}_r{row + 1}",
                Background = new SolidBrush(new Color(60, 60, 60)),
                Width = 100,
                Height = 60,
                Padding = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Create a grid for the cell content
            var contentGrid = new Grid()
            {
                Id = $"content_{space.actionName}_r{row + 1}",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Set up rows: action image, advisor image (if placed), bid coins/quantity info
            contentGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            contentGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            contentGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            // Action image display (from Content folder)
            var imageName = GetActionImageName(space);
            var texture = _textures.GetTexture(imageName);
            //Globals.Log($"CreateCellPanel(): actionName={space.actionName}, quantity={space.quantity}, extraCoin={space.extraCoin}, imageName={imageName}, texture={texture != null}");
            if (texture != null)
            {
                var textureRegion = new TextureRegion(texture);
                var actionImage = new Image()
                {
                    Id = $"action_{space.actionName}_r{row + 1}",
                    Renderable = textureRegion,
                    Width = 50,
                    Height = 50,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Grid.SetColumn(actionImage, 0);
                Grid.SetRow(actionImage, 0);
                contentGrid.Widgets.Add(actionImage);
            }

            // Advisor image display (if advisor is placed on this space)
            if (!string.IsNullOrEmpty(space.color) && space.advisor > 0)
            {
                //Globals.Log("CreateCellPanel(): advisor=" + space.advisor);
                var advisorImageName = GetAdvisorImageName(space);
                var advisorTexture = _textures.GetTexture(advisorImageName);
                if (advisorTexture != null)
                {
                    //Globals.Log("CreateCellPanel(): advisor=" + space.advisor);
                    var textureRegion = new TextureRegion(advisorTexture);
                    var advisorImage = new Image()
                    {
                        Id = $"advisor_{space.actionName}_r{row + 1}",
                        Renderable = textureRegion,
                        Width = 40,
                        Height = 40,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    Grid.SetColumn(advisorImage, 0);
                    Grid.SetRow(advisorImage, 0);
                    contentGrid.Widgets.Add(advisorImage);
                }
            }

            // Bid coins display
            if (space.bidCoins > 0)
            {
                var coinLabel = new Label()
                {
                    Id = $"coin_{space.actionName}_r{row + 1}",
                    Text = space.bidCoins.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = new SolidBrush(Color.Transparent),
                };
                //Grid.SetRow(coinLabel, 2);
                //contentGrid.Widgets.Add(coinLabel);
            }

            // Add quantity info (for reference)
            var quantityLabel = new Label()
            {
                Id = $"quantity_{space.actionName}_r{row + 1}",
                Text = $"Qty: {space.quantity}",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
            };
            //Grid.SetRow(quantityLabel, 2);
            //contentGrid.Widgets.Add(quantityLabel);

            cellPanel.Widgets.Add(contentGrid);
            return cellPanel;
        }

        private string GetActionImageName(AuctionSpace space)
        {
            // Build image name based on action name, quantity, and extraCoin
            // Pattern: {actionName}-{quantity}.png or {actionName}-{quantity}-coin.png
            var imageName = $"{space.actionName}-{space.quantity}";
            if (space.extraCoin > 0)
            {
                imageName += "-coin";
            }
            return imageName;
        }

        private string GetAdvisorImageName(AuctionSpace space)
        {
            // Build advisor image name based on advisor number and color
            // Pattern: {advisor}-{color}.png (e.g., one-yellow.png, five-red.png)
            var advisorText = _advisorNumToText.GetValueOrDefault(space.advisor, space.advisor.ToString().ToLower());
            return $"{advisorText}-{space.color}";
        }

        public void UpdateBoard()
        {
            // Clear existing widgets (except header row)
            var widgetsToRemove = new List<Widget>();
            foreach (var widget in _boardGrid.Widgets)
            {
                if (widget is Label label && label.Id.StartsWith("header_"))
                {
                    continue; // Keep headers
                }
                widgetsToRemove.Add(widget);
            }

            foreach (var widget in widgetsToRemove)
            {
                _boardGrid.Widgets.Remove(widget);
            }

            // Re-add all cells
            string[] columnNames = { "muster", "move", "attack", "tax", "build", "scheme" };
            for (int row = 0; row < _auctionBoard.numberOfRows; row++)
            {
                for (int col = 0; col < columnNames.Length; col++)
                {
                    var actionName = columnNames[col];
                    var auctionSpaces = _auctionBoard.Board[actionName];
                    
                    if (row < auctionSpaces.Count)
                    {
                        var space = auctionSpaces[row];
                        var cellPanel = CreateCellPanel(space, row, col);
                        Grid.SetColumn(cellPanel, col);
                        Grid.SetRow(cellPanel, row + 1); // +1 for header row
                        _boardGrid.Widgets.Add(cellPanel);
                    }
                }
            }
        }

        public void SetFactionColor(string color, Color colorValue)
        {
            _factionColors[color] = colorValue;
        }

        public void SetAuctionBoard(AuctionBoard auctionBoard)
        {
            _auctionBoard = auctionBoard;
        }
    }
}

