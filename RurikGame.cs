using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;

namespace rurik
{
    public class RurikGame
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public int CurrentRound { get; set; }
        public int TargetNumberOfPlayers { get; set; }
        public string Password { get; set; }
        public GameMap GameMap { get; set; }
        public AuctionBoard AuctionBoard { get; set; }
        public Cards Cards { get; set; }
        public ClaimBoard ClaimBoard { get; set; }
        public GamePlayers Players { get; set; }
        public AvailableLeaders AvailableLeaders { get; set; }
        public GameStates GameStates { get; set; }
        public Ai Ai { get; set; }
        public GameLog Log { get; set; }
        public string DeedCardToVerify { get; set; }

        public RurikGame(string gameName, string owner, int targetNumberOfPlayers = 4, string password = "")
        {
            Log = new GameLog(this);
            Owner = owner;
            CurrentRound = 1;
            AuctionBoard = null;
            GameMap = new GameMap();
            Ai = new Ai();
            Cards = new Cards();
            ClaimBoard = new ClaimBoard();
            TargetNumberOfPlayers = targetNumberOfPlayers;
            Players = new GamePlayers(targetNumberOfPlayers);
            AvailableLeaders = new AvailableLeaders();
            GameStates = new GameStates();
            // TODO: generate id with uuid
            Id = DateTime.Now.Ticks.ToString();
            // TODO: formatted creation date
            Name = gameName;
            Password = password;
            DeedCardToVerify = null;
        }

        public void AiEvaluateGame()
        {
            var thisGame = this;
            Ai.EvaluateGame(thisGame);
        }

        public void ChangePlayerAndOrState(string player, string gameStateName)
        {
            if (string.IsNullOrEmpty(player))
            {
                // If player is null or empty, just change the state
                GameStates.ChangeState(gameStateName);
            }
            else
            {
                // Change both player and state
                Players.setCurrentPlayerByColor(player);
                GameStates.ChangeState(gameStateName);
            }
        }

        public void JoinGame(string name, string color, int position, bool isPlayerAi = false, string password = null)
        {
            if (Players.getNumberOfPlayers() >= TargetNumberOfPlayers)
            {
                ThrowError("Game is full", "JoinGame");
                return;
            }

            if (Players.getPlayerByColor(color) != null)
            {
                ThrowError("Player already joined", "JoinGame");
                return;
            }

            if (!string.IsNullOrEmpty(Password) && Password != password)
            {
                ThrowError("Incorrect password", "JoinGame");
                return;
            }

            Player player = new Player(name, color, "N", isPlayerAi); // Added default position "N" to match Player constructor
            Players.addPlayer(name, color, "N", isPlayerAi, Cards); // Using the proper method from GamePlayers
            Log.AddEntry("Player " + name + " joined the game");
        }

        public void StartGame()
        {
            if (Players.getNumberOfPlayers() < 2)
            {
                ThrowError("Not enough players to start the game", "StartGame");
                return;
            }

            // Initialize game state and start the first round
            GameStates.ChangeState("SetupPhase");
            Log.AddLogEntry("Game started with " + Players.getNumberOfPlayers() + " players");
        }

        public void SelectFirstPlayer(string color)
        {
            if (Players.getPlayerByColor(color) == null)
            {
                ThrowError("Player not found", "SelectFirstPlayer");
                return;
            }

            Players.setCurrentPlayerByColor(color);
            Log.AddLogEntry("First player selected: " + color);
        }

        public void SelectRandomFirstPlayer()
        {
            if (Players.getNumberOfPlayers() == 0)
            {
                ThrowError("No players in the game", "SelectRandomFirstPlayer");
                return;
            }

            Random rand = new Random();
            int randomIndex = rand.Next(Players.getNumberOfPlayers());
            string randomPlayer = Players.players[randomIndex].Color;
            Players.setCurrentPlayerByColor(randomPlayer);
            Log.AddLogEntry("Random first player selected: " + randomPlayer);
        }

        public void ChooseLeader(string color, string leaderName)
        {
            Player player = ValidateCurrentPlayer(color, "ChooseLeader");
            if (player == null)
                return;

            if (!AvailableLeaders.IsLeaderAvailable(leaderName)) // Using IsLeaderAvailable instead of Contains
            {
                ThrowError("Leader not available", "ChooseLeader");
                return;
            }

            // Set leader using the proper method
            // We need to get the leader from AvailableLeaders
            Leader leader = AvailableLeaders.GetLeaderByName(leaderName);
            if (leader == null)
            {
                ThrowError("Leader not found", "ChooseLeader");
                return;
            }
            player.setLeader(leader); // Using the setLeader method we defined in Player.cs
            Log.AddLogEntry(color + " chose leader " + leaderName);
        }

        public void SelectSecretAgenda(string color, string cardName)
        {
            Player player = ValidateCurrentPlayer(color, "SelectSecretAgenda");
            if (player == null)
                return;

            if (player.secretAgenda != null)
            {
                ThrowError("Secret agenda already selected", "SelectSecretAgenda");
                return;
            }

            // Assuming there's a method to validate the card name or a list of valid agendas
            // For now, we'll just assign it
            player.secretAgenda = new List<SecretAgendaCard> { new SecretAgendaCard(cardName, "", 0) };
            Log.AddLogEntry(color + " selected secret agenda " + cardName);
        }

        public void PlaceInitialTroop(string color, string locationName)
        {
            Player player = ValidateCurrentPlayer(color, "PlaceInitialTroop");
            if (player == null)
                return;

            // Add troop to the location - need to access the actual location in the map
            // Since we don't know the exact structure of GameMap, we'll use a different approach
            // This is a placeholder - in a real implementation, you'd need to implement the correct GameMap structure
            // For now, we'll just call the method but handle the error appropriately
            try
            {
                // This will be implemented properly in the real GameMap implementation
                Log.AddLogEntry(color + " placed initial troop at " + locationName);
            }
            catch (Exception)
            {
                ThrowError("Location not found: " + locationName, "PlaceInitialTroop");
                return;
            }
            
            player.TroopsToDeploy--;
            Log.AddLogEntry(color + " placed initial troop at " + locationName);
            
            // Check if all players have placed their initial troops
            Player nextPlayer = Players.GetNextPlayer(player);
            if (nextPlayer.TroopsToDeploy > 0)
            {
                Players.setCurrentPlayerByColor(nextPlayer.Color);
            }
            else
            {
                Players.setCurrentPlayerByColor(Players.getFirstPlayer().Color);
                GameStates.ChangeState("waitingForLeaderPlacement");
                Players.setTroopsToDeploy(1);
            }
            AiEvaluateGame();
        }

        public void PlaceLeader(string color, string locationName)
        {
            Player player = ValidateCurrentPlayer(color, "PlaceLeader");
            if (player == null)
                return;

            // Add leader to the location - need to access the actual location in the map
            try
            {
                // This will be implemented properly in the real GameMap implementation
                Log.AddLogEntry(color + " placed leader at " + locationName);
            }
            catch (Exception)
            {
                ThrowError("Location not found: " + locationName, "PlaceLeader");
                return;
            }
            
            player.TroopsToDeploy--;
            Log.AddLogEntry(color + " placed leader at " + locationName);
            
            // Check if all players have placed their leaders
            Player nextPlayer = Players.GetNextPlayer(player);
            if (nextPlayer.TroopsToDeploy > 0)
            {
                Players.setCurrentPlayerByColor(nextPlayer.Color);
            }
            else
            {
                // Move to next game phase
                GameStates.ChangeState("strategyPhase");
                AiEvaluateGame();
            }
        }

        public void PlayAdvisor(string color, string columnName, string advisor, int bidCoins = 0)
        {
            Player player = ValidateCurrentPlayer(color, "PlayAdvisor");
            if (player == null)
                return;

            if (!player.IsAdvisorAvailable(advisor))
            {
                ThrowError("No advisor=" + advisor + " is available.", "PlayAdvisor");
                return;
            }

            if (player.Coins < bidCoins)
            {
                ThrowError("Bid exceeded money available.", "PlayAdvisor");
                return;
            }

            // You may only place an advisor in a column that already contains any of your own
            // advisors after you have placed advisors in three or more different columns.
            if (AuctionBoard.IsPlayerAlreadyInColumn(columnName, color))
            {
                if (!AuctionBoard.IsPlayerIn3Columns(color))
                {
                    ThrowError("You cannot place an advisor in the same column, " + columnName + ", until you are present in 3 or more different columns.", "PlayAdvisor");
                    return;
                }
            }

            // Place the bid on the advisor
            // AuctionBoard.PlaceBid(columnName, color, advisor, bidCoins); // This method doesn't exist
            // Since Coins is read-only, we need to modify the boat's money directly
            player.boat.money -= bidCoins;
            player.UseAdvisor(advisor);
            
            Log.AddLogEntry(color + " placed advisor " + advisor + " at " + columnName + " with " + bidCoins + " coins.");
            
            // Check if all players have played their advisors
            Player nextPlayer = Players.GetNextPlayer(player);
            if (nextPlayer.Advisors.Count > 0)
            {
                Players.setCurrentPlayerByColor(nextPlayer.Color);
            }
            else
            {
                // All advisors played, move to next state
                Players.setCurrentPlayerByColor(Players.getFirstPlayer().Color);
                List<int> advisors = GetAdvisorsForRound(Players.getNumberOfPlayers(), CurrentRound - 1);
                Players.SetAdvisors(advisors);
                Players.MapAdvisorsToAuctionSpaces(AuctionBoard);
                GameStates.ChangeState("retrieveAdvisor");
                
                // Reset for next round if needed
                if (CurrentRound >= 4)
                {
                    // End the game or handle end of round logic
                    // For now, just ensure we're in the right state
                }
            }
            AiEvaluateGame();
        }

        public void BeginPlayerAction()
        {
            // Validate we're in the right state to begin player action
            ValidateGameStatus("actionPhase", "BeginPlayerAction");
            Log.AddLogEntry("Beginning player action phase");
        }

        public void UndoPlayerAction()
        {
            // TODO: Implement undo player action
            Log.AddLogEntry("Undo player action - placeholder implementation");
        }

        public void TakeMainAction(string color, string advisor, string actionColumnName, int row, bool forfeitAction = false)
        {
            Player player = ValidateCurrentPlayer(color, "TakeMainAction");
            if (player == null)
                return;

            // Basic implementation of main action logic - in a real implementation this would involve
            // detailed validation and action processing based on the advisor and action column
            if (player.TookMainActionForTurn)
            {
                ThrowError("Player already took main action for this turn", "TakeMainAction");
                return;
            }

            // Validate advisor is available (simplified check)
            if (player.Advisors.Count == 0 || player.Advisors[0] != advisor)
            {
                // In a real implementation, this would be more complex validation
                Log.AddLogEntry("Warning: Advisor mismatch for " + color + " with advisor " + advisor);
            }

            // Remove advisor from player's hand (simplified)
            if (player.Advisors.Count > 0)
            {
                player.Advisors.RemoveAt(0);
            }

            // Process the action based on column name (simplified)
            Log.AddLogEntry(color + " took main action with advisor " + advisor + " in column " + actionColumnName + " row " + row);
            player.TookMainActionForTurn = true;

            // Move to next game state based on action column
            if (actionColumnName == "scheme")
            {
                // If scheme action, player might get to choose first player or draw cards
                // This is simplified - in real game, this would be more complex
                GameStates.ChangeState("actionPhase");
            }
            else
            {
                // For other actions, move to action phase
                GameStates.ChangeState("actionPhase");
            }

            AiEvaluateGame();
        }

        public void SchemeFirstPlayer(string currentPlayerColor, string firstPlayerColor)
        {
            Player player = ValidateCurrentPlayer(currentPlayerColor, "SchemeFirstPlayer");
            if (player == null)
                return;

            // TODO: Implement actual scheme first player logic
            Log.AddLogEntry(currentPlayerColor + " schemed first player to " + firstPlayerColor);
        }

        public void DrawSchemeCards(string color, string schemeDeck)
        {
            // TODO: Implement draw scheme cards
        }

        public void SelectSchemeCardToKeep(string color, string schemeCard)
        {
            // TODO: Implement select scheme card to keep
        }

        public void SelectSchemeCardToReturn(string color, int schemeDeckNumber, string schemeCardId)
        {
            // TODO: Implement select scheme card to return
        }

        public void Muster(string color, string locationName, int numberOfTroops)
        {
            // TODO: Implement muster
        }

        public void Move(string color, string fromLocationName, string toLocationName, int numberOfTroops = 1, bool moveLeader = false)
        {
            // TODO: Implement move
        }

        public void Tax(string color, string locationName, bool toBoat = true, bool marketCoinNotResource = false)
        {
            // TODO: Implement tax
        }

        public void Build(string color, string locationName, string buildingName, string targetToConvert = null)
        {
            // TODO: Implement build
        }

        public void Attack(string color, string locationName, string target, int schemeDeckNumber = 1)
        {
            // TODO: Implement attack
        }

        public void GoUpWarTrack(string currentPlayer)
        {
            // TODO: Implement go up war track
        }

        public void HandleWarTrackReward(string currentPlayer, string reward)
        {
            // TODO: Implement handle war track reward
        }

        public void TransferGood(string color, string direction, string resource)
        {
            // TODO: Implement transfer good
        }

        public void DrawOneTimeSchemeCard(string color, string schemeDeck)
        {
            // TODO: Implement draw one time scheme card
        }

        public void PlayOneTimeSchemeCard(string color)
        {
            // TODO: Implement play one time scheme card
        }

        public void PlaySchemeCard(string color, string schemeCardId, string schemeCardActionChoice)
        {
            // TODO: Implement play scheme card
        }

        public void CollectSchemeCardReward(string currentPlayer, string schemeCard, string schemeCardActionChoice = null)
        {
            // TODO: Implement collect scheme card reward
        }

        public void TakeDeedCard(string color, string deedCardName)
        {
            // TODO: Implement take deed card
        }

        public void AccomplishedDeed(string color, string deedCardName, List<Dictionary<string, string>> claimStatements)
        {
            // TODO: Implement accomplished deed
        }

        public void VerifyDeed(string color, bool verified)
        {
            // TODO: Implement verify deed
        }

        public void RedeemDeed(Player player, string deedCard)
        {
            // TODO: Implement redeem deed
        }

        public void AccomplishAndRedeemDeed(Player player, string deedCard)
        {
            // TODO: Implement accomplish and redeem deed
        }

        public void EndRound()
        {
            // TODO: Implement end round
        }

        public void EndGame()
        {
            // TODO: Implement end game
        }

        public Dictionary<string, object> CalculateEndGameStats()
        {
            // TODO: Implement calculate end game stats
            return new Dictionary<string, object>();
        }

        public List<string> GetFirstPlacePlayers(Dictionary<string, object> endGameStats)
        {
            // TODO: Implement get first place players
            return new List<string>();
        }

        public void PlayMusterConversionTile(string currentPlayer, string resource1, string resource2)
        {
            // TODO: Implement play muster conversion tile
        }

        public void PlayBuildConversionTile(string currentPlayer, string resource1, string resource2)
        {
            // TODO: Implement play build conversion tile
        }

        public void PlayAttackConversionTile(string currentPlayer)
        {
            // TODO: Implement play attack conversion tile
        }

        public void PlayConversionTile(string color, string conversionTileName, string resource1, string resource2)
        {
            // TODO: Implement play conversion tile
        }

        public void EndCurrentAction(string color)
        {
            // Validate current player
            Player player = ValidateCurrentPlayer(color, "EndCurrentAction");
            if (player == null)
                return;

            // Reset action state
            GameStates.ChangeState("actionPhase");
            Log.AddLogEntry(color + " ended current action");
        }

        public void EndTurn(string color)
        {
            Player player = ValidateCurrentPlayer(color, "EndTurn");
            if (player == null)
                return;

            // Reset main action flag for next turn
            player.TookMainActionForTurn = false;

            // Move to next player
            Player nextPlayer = Players.GetNextPlayer(player);
            Players.setCurrentPlayerByColor(nextPlayer.Color);

            Log.AddLogEntry(color + " ended turn. Next player: " + nextPlayer.Color);

            // Evaluate game state after turn ends
            AiEvaluateGame();
        }

        public void UpdateClaimsForClaimsPhase()
        {
            // TODO: Implement update claims for claims phase
        }

        public void EvaluateSecretAgendas()
        {
            // TODO: Implement evaluate secret agendas
        }

        public bool EvaluateSecretAgendaProsperous(string playerColor)
        {
            // TODO: Implement evaluate secret agenda prosperous
            return false;
        }

        public bool EvaluateSecretAgendaEsteemed(string playerColor)
        {
            // TODO: Implement evaluate secret agenda esteemed
            return false;
        }

        public bool EvaluateSecretAgendaCapable(string playerColor)
        {
            // TODO: Implement evaluate secret agenda capable
            return false;
        }

        public bool EvaluateSecretAgendaConquering(string playerColor)
        {
            // TODO: Implement evaluate secret agenda conquering
            return false;
        }

        public bool EvaluateSecretAgendaProtective(string playerColor)
        {
            // TODO: Implement evaluate secret agenda protective
            return false;
        }

        public bool EvaluateSecretAgendaSuccessful(string playerColor)
        {
            // TODO: Implement evaluate secret agenda successful
            return false;
        }

        public bool EvaluateSecretAgendaRegal(string playerColor)
        {
            // TODO: Implement evaluate secret agenda regal
            return false;
        }

        public bool EvaluateSecretAgendaCommitted(string playerColor)
        {
            // TODO: Implement evaluate secret agenda committed
            return false;
        }

        public bool EvaluateSecretAgendaDignified(string playerColor)
        {
            // TODO: Implement evaluate secret agenda dignified
            return false;
        }

        public bool EvaluateSecretAgendaCourageous(string playerColor)
        {
            // TODO: Implement evaluate secret agenda courageous
            return false;
        }

        public bool EvaluateSecretAgendaWealthy(string playerColor)
        {
            // TODO: Implement evaluate secret agenda wealthy
            return false;
        }

        public void BeginActionPhaseAction(string color, string action)
        {
            // Validate current player
            Player currentPlayer = ValidateCurrentPlayer(color, "BeginActionPhaseAction");
            if (currentPlayer == null)
                return;

            // Map action to game state
            var actionToStateMap = new Dictionary<string, string>
            {
                {"cancel", "actionPhase"},
                {"musterAction", "actionPhaseMuster"},
                {"moveAction", "actionPhaseMove"},
                {"attackAction", "actionPhaseAttack"},
                {"taxAction", "actionPhaseTax"},
                {"buildAction", "actionPhaseBuild"},
                {"transferGoodsAction", "actionPhaseTransfer"},
                {"schemeAction", "actionPhasePlaySchemeCard"},
                {"convertGoodsAction", "actionPhasePlayConversionTile"},
                {"accomplishDeedAction", "actionPhaseAccomplishDeed"}
            };

            // Validate we're in action phase and set new state
            if (GameStates.GetCurrentState().Name != null && GameStates.GetCurrentState().Name.StartsWith("actionPhase"))
            {
                if (actionToStateMap.ContainsKey(action))
                {
                    GameStates.ChangeState(actionToStateMap[action]);
                    Log.AddLogEntry("Player " + color + " began action: " + action);
                }
                else
                {
                    ThrowError("Invalid action: " + action, "BeginActionPhaseAction");
                }
            }
            else
            {
                ThrowError("Cannot begin action, not in actionPhase", "BeginActionPhaseAction");
            }
        }

        public List<int> GetAdvisorsForRound(int numberOfPlayers, int round)
        {
            // Advisors by round for different player counts (based on the JavaScript implementation)
            var advisorsByRound = new List<List<int>>
            {
                new List<int> { 1, 2, 4, 5 }, // Round 0
                new List<int> { 1, 2, 4, 5 }, // Round 1
                new List<int> { 1, 2, 2, 4, 5 }, // Round 2
                new List<int> { } // Round 3 - will be populated based on player count
            };

            // Set up round 3 advisors based on number of players
            if (round == 3)
            {
                if (numberOfPlayers == 2)
                {
                    advisorsByRound[3] = new List<int> { 1, 2, 2, 3, 4, 5 };
                }
                else if (numberOfPlayers == 3)
                {
                    advisorsByRound[3] = new List<int> { 1, 2, 2, 3, 4, 5 };
                }
                else
                {
                    advisorsByRound[3] = new List<int> { 1, 2, 2, 4, 5 };
                }
            }

            if (round >= 0 && round < advisorsByRound.Count)
            {
                return advisorsByRound[round];
            }

            // Return default advisors if round is out of range
            return new List<int> { 1, 2, 4, 5 };
        }

        public Player GetPlayer(string color)
        {
            return Players.GetPlayer(color);
        }

        public void ValidateGameStatus(string desiredState, string method, string message = null)
        {
            if (GameStates.GetCurrentState().Name != desiredState)
            {
                ThrowError("Invalid game state. Expected: " + desiredState + ", Got: " + GameStates.GetCurrentState().Name, method);
            }
        }

        public void ValidateGameStatus2(List<string> possibleStates, string method, string message = null)
        {
            if (!possibleStates.Contains(GameStates.GetCurrentState().Name))
            {
                ThrowError("Invalid game state. Expected one of: " + string.Join(", ", possibleStates) + ", Got: " + GameStates.GetCurrentState().Name, method);
            }
        }

        public Player ValidateCurrentPlayer(string color, string method)
        {
            if (Players.getCurrentPlayer().Color != color)
            {
                ThrowError("Not current player", method);
                return null;
            }
            return Players.GetPlayer(color);
        }

        public void ThrowError(string message, string method)
        {
            Log.AddLogEntry("ERROR in " + method + ": " + message);
            throw new Exception(method + "(): " + message);
        }
    }
}
