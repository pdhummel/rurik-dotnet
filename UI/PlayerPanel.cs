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

        Grid playerPanelGrid = new Grid();


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


            // Add main grid to panel
            //_playerPanel.Widgets.Add(verticalPanel);
            _playerPanel.Widgets.Add(playerPanelGrid);

            // Add panel to this container

            _mainPanel = new Panel();
            _mainGrid = new Grid();
            _mainPanel.Widgets.Add(_mainGrid);
            _mainGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // player panel
            _mainGrid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // boat panel
            this.Widgets.Add(_mainPanel);
        }

        private void AddFirstPlayerTokenSection()
        {
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
                if (_player.isFirstPlayer)
                {
                    Grid.SetColumn(bearImage, 0);
                    Grid.SetRow(bearImage, 0);
                    playerPanelGrid.Widgets.Add(bearImage);
                }

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
                if (_player.supplyLeader > 0)
                {
                    Grid.SetColumn(leaderImage, 1);
                    Grid.SetRow(leaderImage, 0);
                    playerPanelGrid.Widgets.Add(leaderImage);
                    Label label = new Label()
                    { 
                        Text="" + _player.supplyLeader,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    Grid.SetColumn(label, 1);
                    Grid.SetRow(label, 0);
                    playerPanelGrid.Widgets.Add(label);
                }
            }
        }

        private void AddTroopsSection()
        {
            Panel troopPanel = new Panel();
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
                if (_player.supplyTroops > 0)
                {
                    Grid.SetColumn(troopsImage, 2);
                    Grid.SetRow(troopsImage, 0);
                    playerPanelGrid.Widgets.Add(troopsImage);
                    Label label = new Label()
                    { 
                        Text="" + _player.supplyTroops,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    Grid.SetColumn(label, 2);
                    Grid.SetRow(label, 0);
                    playerPanelGrid.Widgets.Add(label);
                }
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
                if (_player.boat.capturedRebels >= 0)
                {
                    Grid.SetColumn(rebelImage, 3);
                    Grid.SetRow(rebelImage, 0);
                    playerPanelGrid.Widgets.Add(rebelImage);
                    Label label = new Label()
                    { 
                        Text="" + _player.boat.capturedRebels,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    Grid.SetColumn(label, 3);
                    Grid.SetRow(label, 0);
                    playerPanelGrid.Widgets.Add(label);
                }
            }

            // Card counts on the first row next to captured rebels
            int cardCol = 4;

            // Scheme cards
            var schemeTexture = _textures.GetTexture("scheme-card");
            if (schemeTexture != null)
            {
                var schemePanel = new Panel()
                {
                    Id = "schemeCardPanel",
                    Width = 35,
                    Height = 50,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = new SolidBrush(Color.Transparent),
                };

                var textureRegion = new TextureRegion(schemeTexture);
                var schemeImage = new Image()
                {
                    Id = "schemeCardImage",
                    Renderable = textureRegion,
                    Width = 25,
                    Height = 35,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Grid.SetColumn(schemeImage, 0);
                Grid.SetRow(schemeImage, 1);
                schemePanel.Widgets.Add(schemeImage);

                Label schemeLabel = new Label()
                {
                    Text = _player.schemeCards.Count.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Grid.SetColumn(schemeLabel, 0);
                Grid.SetRow(schemeLabel, 0);
                schemePanel.Widgets.Add(schemeLabel);

                Grid.SetColumn(schemePanel, cardCol);
                Grid.SetRow(schemePanel, 0);
                playerPanelGrid.Widgets.Add(schemePanel);
                cardCol += 1;
            }

            // Secret agenda cards
            var secretAgendaTexture = _textures.GetTexture("secret-agenda");
            if (secretAgendaTexture != null)
            {
                var secretAgendaPanel = new Panel()
                {
                    Id = "secretAgendaCardPanel",
                    Width = 35,
                    Height = 50,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = new SolidBrush(Color.Transparent),
                };

                var textureRegion = new TextureRegion(secretAgendaTexture);
                var secretAgendaImage = new Image()
                {
                    Id = "secretAgendaCardImage",
                    Renderable = textureRegion,
                    Width = 25,
                    Height = 35,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Grid.SetColumn(secretAgendaImage, 0);
                Grid.SetRow(secretAgendaImage, 1);
                secretAgendaPanel.Widgets.Add(secretAgendaImage);

                Label secretAgendaLabel = new Label()
                {
                    Text = _player.secretAgenda.Count.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Grid.SetColumn(secretAgendaLabel, 0);
                Grid.SetRow(secretAgendaLabel, 0);
                secretAgendaPanel.Widgets.Add(secretAgendaLabel);

                Grid.SetColumn(secretAgendaPanel, cardCol);
                Grid.SetRow(secretAgendaPanel, 0);
                playerPanelGrid.Widgets.Add(secretAgendaPanel);
                cardCol += 1;
            }

            // Deed cards
            var deedTexture = _textures.GetTexture("deed-card");
            if (deedTexture != null)
            {
                var deedPanel = new Panel()
                {
                    Id = "deedCardPanel",
                    Width = 35,
                    Height = 50,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = new SolidBrush(Color.Transparent),
                };

                var textureRegion = new TextureRegion(deedTexture);
                var deedImage = new Image()
                {
                    Id = "deedCardImage",
                    Renderable = textureRegion,
                    Width = 25,
                    Height = 35,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Grid.SetColumn(deedImage, 0);
                Grid.SetRow(deedImage, 1);
                deedPanel.Widgets.Add(deedImage);

                Label deedLabel = new Label()
                {
                    Text = _player.deedCards.Count.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Grid.SetColumn(deedLabel, 0);
                Grid.SetRow(deedLabel, 0);
                deedPanel.Widgets.Add(deedLabel);

                Grid.SetColumn(deedPanel, cardCol);
                Grid.SetRow(deedPanel, 0);
                playerPanelGrid.Widgets.Add(deedPanel);
                cardCol += 1;
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
                    if (_player.buildings[building] > 0)
                    {
                        Grid.SetColumn(buildingImage, buildingCol);
                        Grid.SetRow(buildingImage, 1);
                        playerPanelGrid.Widgets.Add(buildingImage);
                        Label label = new Label()
                        { 
                            Text="" + _player.buildings[building],
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                        };
                        Grid.SetColumn(label, buildingCol);
                        Grid.SetRow(label, 1);
                        playerPanelGrid.Widgets.Add(label);
                    }
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
        public void UpdatePanel(Player player)
        {
            Globals.Log("UpdatePanel(): enter");
            if (player != null)
                _player = player;
            playerPanelGrid.Widgets.Clear();
            AddFirstPlayerTokenSection();
            AddLeaderSection();
            AddTroopsSection();
            AddCapturedRebelsSection();
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
            Globals.Log("SetPlayer(): enter");
            _player = player;
            UpdatePanel(player);
        }
    }
}
