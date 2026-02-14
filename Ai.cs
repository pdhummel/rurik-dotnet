using System;
using System.Collections.Generic;
using System.Linq;

namespace rurik
{
    public class Ai
    {
        private Dictionary<string, string> mapStateToFunction;
        private List<AiStrategyCard> aiCards;

        public Ai()
        {
            mapStateToFunction = new Dictionary<string, string>
            {
                { "waitingForLeaderSelection", "SelectLeader" },
                { "waitingForSecretAgendaSelection", "SelectSecretAgenda" },
                { "waitingForTroopPlacement", "PlaceInitialTroop" },
                { "waitingForLeaderPlacement", "PlaceLeader" },
                { "strategyPhase", "PlaceAdvisor" },
                { "retrieveAdvisor", "RetrieveAdvisor" },
                { "actionPhase", "TakeAction" },
                { "takeDeedCardForClaimPhase", "TakeDeedCard" },
                { "takeDeedCardForActionPhase", "TakeDeedCardForActionPhase" },
                { "schemeFirstPlayer", "SchemeFirstPlayer" },
                { "drawSchemeCards", "DrawSchemeCards" },
                { "selectSchemeCard", "SelectSchemeCard" },
                { "endGame", "EndGame" }
            };

            aiCards = new List<AiStrategyCard>();
            CreateAiCards();
        }

        public void EvaluateGame(RurikGame game)
        {
            var currentPlayer = game.Players.GetCurrentPlayer();
            if (!currentPlayer.isPlayerAi)
            {
                return;
            }

            Console.WriteLine("evaluateGame(): isPlayerAi=true");
            var currentState = game.GameStates.getCurrentState();
            if (currentState != null)
            {
                var currentStateName = currentState.Name;
                if (mapStateToFunction.TryGetValue(currentStateName, out string method))
                {
                    Console.WriteLine($"evaluateGame(): currentState={currentStateName}, currentPlayer={currentPlayer.Color}, method={method}");
                    try
                    {
                        switch (method)
                        {
                            case "SelectLeader":
                                SelectLeader(game, currentPlayer);
                                break;
                            case "SelectSecretAgenda":
                                SelectSecretAgenda(game, currentPlayer);
                                break;
                            case "PlaceInitialTroop":
                                PlaceInitialTroop(game, currentPlayer);
                                break;
                            case "PlaceLeader":
                                PlaceLeader(game, currentPlayer);
                                break;
                            case "PlaceAdvisor":
                                PlaceAdvisor(game, currentPlayer);
                                break;
                            case "RetrieveAdvisor":
                                RetrieveAdvisor(game, currentPlayer);
                                break;
                            case "TakeAction":
                                TakeAction(game, currentPlayer);
                                break;
                            case "TakeDeedCard":
                                TakeDeedCard(game, currentPlayer);
                                break;
                            case "TakeDeedCardForActionPhase":
                                TakeDeedCardForActionPhase(game, currentPlayer);
                                break;
                            case "SchemeFirstPlayer":
                                SchemeFirstPlayer(game, currentPlayer);
                                break;
                            case "DrawSchemeCards":
                                DrawSchemeCards(game, currentPlayer);
                                break;
                            case "SelectSchemeCard":
                                SelectSchemeCard(game, currentPlayer);
                                break;
                            case "EndGame":
                                EndGame(game, currentPlayer);
                                break;
                        }
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine($"ai evaluateGame(): {error.Message}");
                        Console.WriteLine(error.StackTrace);
                    }
                }
                else
                {
                    Console.WriteLine("evaluateGame(): method not found for " + currentStateName);
                }
            }
        }

        private void SelectLeader(RurikGame game, Player player)
        {
            Console.WriteLine("selectLeader()");
            var leaderNames = new List<string> { "Boris", "Sviatopolk", "Yaroslav", "Mstislav" };
            var random = new Random();
            var leaderName = leaderNames[random.Next(leaderNames.Count)];
            player.name = leaderName;
            game.ChooseLeader(player.Color, leaderName);
        }

        private void SelectSecretAgenda(RurikGame game, Player player)
        {
            Console.WriteLine("selectSecretAgenda()");
            game.SelectSecretAgenda(player.Color, player.temporarySecretAgenda[0].name);
        }

        private void PlaceInitialTroop(RurikGame game, Player player)
        {
            var random = new Random();
            var r = random.Next(game.GameMap.LocationsForGame.Count);
            var locationName = game.GameMap.LocationsForGame[r].name;
            game.PlaceInitialTroop(player.Color, locationName);
        }

        private void PlaceLeader(RurikGame game, Player player)
        {
            var random = new Random();
            var r = random.Next(game.GameMap.LocationsForGame.Count);
            var locationName = game.GameMap.LocationsForGame[r].name;
            game.PlaceLeader(player.Color, locationName);
        }

        private void PlaceAdvisor(RurikGame game, Player player)
        {
            if (player.aiCard == null)
            {
                var random = new Random();
                var r = random.Next(aiCards.Count);
                var aiCard = aiCards[r];
                player.aiCard = aiCard;
                var strategies = new List<string> { "attack-move", "build", "tax" };
                r = random.Next(strategies.Count);
                player.aiStrategy = strategies[r];
            }

            var aiStrategy = player.aiCard.strategies[player.aiStrategy];
            Console.WriteLine($"placeAdvisor(): aiCard={player.aiCard.id} {player.aiStrategy}");
            var auctions = aiStrategy.auctions;
            var advisorNumber = 0;
            var index = 0;
            var success = false;
            var auction = default(AiAuction);

            // Using the AiStrategyCard, try to place the advisor per the current turn. 
            // If that is not possible, move to the next turn recommendation on the AiStrategyCard.
            // The AiStrategyCards have some illegal moves which must be skipped.
            while (!success && index < auctions.Count && player.advisors.Count > 0)
            {
                try
                {
                    auction = auctions[index];
                    advisorNumber = auction.advisor;
                    game.PlayAdvisor(player.Color, auction.action, ""+advisorNumber, auction.coins);
                    success = true;
                }
                catch (Exception error)
                {
                    Console.WriteLine($"Could not place candidate advisor using strategy card: {auction.action} {advisorNumber}: {error.Message}");
                    index++;
                }
            }

            if (!success)
            {
                Console.WriteLine("placeAdvisor(): advisor could not be placed using strategy card");
            }

            var actions = new List<string> { "muster", "move", "attack", "tax", "build", "scheme" };
            var actionsTried = new HashSet<string>();
            while (!success && player.advisors.Count > 0 && actionsTried.Count < actions.Count)
            {
                var random = new Random();
                var r = random.Next(actions.Count);
                var action = actions[r];
                actionsTried.Add(action);
                var advisor = player.advisors[0];
                try
                {
                    game.PlayAdvisor(player.Color, action, advisor, 0);
                    success = true;
                }
                catch (Exception error)
                {
                    Console.WriteLine($"Could not place candidate advisor: {action} {advisor}: {error.Message}");
                }
            }
        }

        private bool IsAdvisorInList(int advisor, List<int> advisorList)
        {
            return advisorList.Contains(advisor);
        }

        private void RetrieveAdvisor(RurikGame game, Player player)
        {
            Console.WriteLine($"ai retrieveAdvisor(): player={player.Color}, advisors={string.Join(",", player.advisors)}");
            var color = player.Color;
            var advisor = player.advisors[0];
            var advisorCount = player.advisors.Count;
            var auctionSpaces = player.advisorsToAuctionSpace[int.Parse(advisor)];
            var auctionSpace = auctionSpaces[0];
            var actionColumnName = auctionSpace.actionName;
            var row = auctionSpace.row;
            try
            {
                game.TakeMainAction(color, advisor, actionColumnName, row);
            }
            catch (Exception error)
            {
                Console.WriteLine($"ai retrieveAdvisor(): Warning player={player.Color} forfeiting {auctionSpace.actionName}");
                if (player.advisors.Count < advisorCount)
                {
                    player.boat.money++;
                    player.tookMainActionForTurn = true;
                    game.GameStates.setCurrentState("actionPhase");
                    game.EndTurn(player.Color);
                }
                else
                {
                    game.TakeMainAction(color, advisor, actionColumnName, row, true);
                }
            }
        }

        private void TakeAction(RurikGame game, Player player)
        {
            Console.WriteLine($"ai takeAction(): player={player.Color}");
            // Clone the game and calculate current points for all players.
            // TODO: var clonedGame = game.Clone();
            //var endGameStats = clonedGame.CalculateEndGameStats();
            //var decisionValue = CalculateDecisionValue(clonedGame, player);

            var takeAction = true;
            while (takeAction)
            {
                takeAction = false;
                if (player.deedCards.Count > 0 && !player.accomplishedDeedForTurn && game.CurrentRound >= 3)
                {
                    PlayDeedCard(game, player);
                }
                if (player.schemeCards.Count > 0 && player.schemeCardsCanPlay > 0)
                {
                    PlaySchemeCard(game, player);
                }
                if (player.troopsToDeploy > 0)
                {
                    if (Muster(game, player))
                    {
                        takeAction = true;
                    }
                }
                if (player.taxActions > 0)
                {
                    if (Tax(game, player))
                    {
                        takeAction = true;
                    }
                }
                if (player.attackActions > 0)
                {
                    if (Attack(game, player))
                    {
                        takeAction = true;
                    }
                }
                if (player.moveActions > 0)
                {
                    if (Move(game, player))
                    {
                        takeAction = true;
                    }
                }
                if (player.buildActions > 0)
                {
                    if (Build(game, player))
                    {
                        takeAction = true;
                    }
                }
            }
            game.EndTurn(player.Color);
        }

        private void PlaySchemeCard(RurikGame game, Player player)
        {
            Console.WriteLine($"ai playSchemeCard(): player={player.Color}");
            var color = player.Color;
            // For now, just play the first scheme card available which won't be too smart,
            // but better than holding the cards for the whole game.
            if (player.boat.money >= 3)
            {
                var random = new Random();
                var r = random.Next(player.schemeCards.Count);
                if (!player.schemeCards[r].rewards.Contains("deedCard"))
                {
                    game.BeginActionPhaseAction(color, "schemeAction");
                    game.PlaySchemeCard(color, player.schemeCards[r].id, null);
                }
            }
        }

        private void PlayDeedCard(RurikGame game, Player player)
        {
            Console.WriteLine($"ai playDeedCard(): player={player.Color}");
            var color = player.Color;
            var isSviatopolk = player.leader.name == "Sviatopolk";
            var isYaroslav = player.leader.name == "Yaroslav";
            for (var i = 0; i < player.deedCards.Count; i++)
            {
                var deedCard = player.deedCards[i];
                var canFulfill = false;
                if (player.accomplishedDeedForTurn)
                {
                    break;
                }
                if (deedCard.accomplished)
                {
                    continue;
                }
                if (deedCard.canAi)
                {
                    Console.WriteLine($"ai playDeedCard(): player={player.Color}, deedCard={deedCard.name}");
                    canFulfill = true;
                    if (deedCard.name == "Create Republic")
                    {
                        // Novgorod, Chernigov, Volyn
                        var novgorod = game.GameMap.GetLocation("Novgorod");
                        var chernigov = game.GameMap.GetLocation("Chernigov");
                        var volyn = game.GameMap.GetLocation("Volyn");
                        if (novgorod.DoesRule(color, isSviatopolk, isYaroslav) && chernigov.DoesRule(color, isSviatopolk, isYaroslav) &&
                            volyn.DoesRule(color, isSviatopolk, isYaroslav))
                        {
                            game.AccomplishAndRedeemDeed(player, deedCard.name);
                        }
                        continue;
                    }
                    else if (deedCard.name == "Distant Rule")
                    {
                        // Pereyaslavl, Polotsk, Rostov
                        var pereyaslavl = game.GameMap.GetLocation("Pereyaslavl");
                        var polotsk = game.GameMap.GetLocation("Polotsk");
                        var rostov = game.GameMap.GetLocation("Rostov");
                        if (pereyaslavl.DoesRule(color, isSviatopolk, isYaroslav) && polotsk.DoesRule(color, isSviatopolk, isYaroslav) &&
                            rostov.DoesRule(color, isSviatopolk, isYaroslav))
                        {
                            game.AccomplishAndRedeemDeed(player, deedCard.name);
                        }
                        continue;
                    }

                    var rebels = player.boat.capturedRebels;
                    var money = player.boat.money;
                    var stone = player.boat.goodsOnDock["stone"];
                    var wood = player.boat.goodsOnDock["wood"];
                    var fish = player.boat.goodsOnDock["fish"];
                    var honey = player.boat.goodsOnDock["honey"];
                    var fur = player.boat.goodsOnDock["fur"];
                    var tradeBoon = player.boat.goodsOnDock["tradeBoon"];
                    var schemeCards = new List<SchemeCard>(player.schemeCards);

                    if (deedCard.name == "Hoard")
                    {
                        if (stone >= 3)
                        {
                            player.boat.goodsOnDock["stone"] = stone - 3;
                            game.AccomplishAndRedeemDeed(player, deedCard.name);
                        }
                        else if (wood >= 3)
                        {
                            player.boat.goodsOnDock["wood"] = wood - 3;
                            game.AccomplishAndRedeemDeed(player, deedCard.name);
                        }
                        else if (fish >= 3)
                        {
                            player.boat.goodsOnDock["fish"] = fish - 3;
                            game.AccomplishAndRedeemDeed(player, deedCard.name);
                        }
                        else if (honey >= 3)
                        {
                            player.boat.goodsOnDock["honey"] = honey - 3;
                            game.AccomplishAndRedeemDeed(player, deedCard.name);
                        }
                        else if (fur >= 3)
                        {
                            player.boat.goodsOnDock["fur"] = fur - 3;
                            game.AccomplishAndRedeemDeed(player, deedCard.name);
                        }
                        continue;
                    }
                    else if (deedCard.name == "Trade Route")
                    {
                        var count = 0;
                        if (stone > 0)
                        {
                            stone--;
                            count++;
                        }
                        if (wood > 0)
                        {
                            wood--;
                            count++;
                        }
                        if (fish > 0)
                        {
                            fish--;
                            count++;
                        }
                        if (count < 3 && honey > 0)
                        {
                            honey--;
                            count++;
                        }
                        if (count < 3 && fur > 0)
                        {
                            fur--;
                            count++;
                        }
                        if (count >= 3)
                        {
                            player.boat.goodsOnDock["stone"] = stone;
                            player.boat.goodsOnDock["wood"] = wood;
                            player.boat.goodsOnDock["fish"] = fish;
                            player.boat.goodsOnDock["honey"] = honey;
                            player.boat.goodsOnDock["fur"] = fur;
                            game.AccomplishAndRedeemDeed(player, deedCard.name);
                        }
                        continue;
                    }
                    else if (deedCard.name == "Mead Brewery")
                    {
                        if (honey > 0 && player.buildings["tavern"] == 0)
                        {
                            player.boat.goodsOnDock["honey"] = honey - 1;
                            game.AccomplishAndRedeemDeed(player, deedCard.name);
                        }
                        continue;
                    }
                    else if (deedCard.name == "Horse Breeder")
                    {
                        if (wood > 0 && player.buildings["stable"] == 0)
                        {
                            player.boat.goodsOnDock["wood"] = wood - 1;
                            game.AccomplishAndRedeemDeed(player, deedCard.name);
                        }
                        continue;
                    }
                    else if (deedCard.name == "Establish Fortress")
                    {
                        if (wood > 1)
                        {
                            var found = false;
                            for (var l = 0; l < game.GameMap.LocationsForGame.Count; l++)
                            {
                                var location = game.GameMap.LocationsForGame[l];
                                if (location.DoesPlayerHaveThisBuilding(color, "stronghold") &&
                                    location.DoesPlayerHaveThisBuilding(color, "church"))
                                {
                                    found = true;
                                }
                            }
                            if (found)
                            {
                                player.boat.goodsOnDock["wood"] = wood - 2;
                                game.AccomplishAndRedeemDeed(player, deedCard.name);
                            }
                        }
                        continue;
                    }
                    else if (deedCard.name == "Besiege Citadel")
                    {
                        if (money > 1)
                        {
                            var found = false;
                            for (var l = 0; l < game.GameMap.LocationsForGame.Count; l++)
                            {
                                var location = game.GameMap.LocationsForGame[l];
                                if (location.DoesPlayerHaveThisBuilding(color, "stronghold"))
                                {
                                    found = true;
                                }
                            }
                            if (found)
                            {
                                player.boat.money = money - 2;
                                game.AccomplishAndRedeemDeed(player, deedCard.name);
                            }
                        }
                        continue;
                    }
                    else if (deedCard.name == "Conquest")
                    {
                        var rules = 0;
                        for (var l = 0; l < game.GameMap.LocationsForGame.Count; l++)
                        {
                            var location = game.GameMap.LocationsForGame[l];
                            if (location.DoesRule(color, isSviatopolk, isYaroslav))
                            {
                                rules++;
                            }
                        }
                        if (rules >= 3)
                        {
                            game.AccomplishAndRedeemDeed(player, deedCard.name);
                        }
                        continue;
                    }
                    else if (deedCard.name == "Amass Forces")
                    {
                        var found = false;
                        for (var l = 0; l < game.GameMap.LocationsForGame.Count; l++)
                        {
                            var location = game.GameMap.LocationsForGame[l];
                            if (location.troopsByColor[color] >= 6)
                            {
                                found = true;
                            }
                        }
                        if (found)
                        {
                            game.AccomplishAndRedeemDeed(player, deedCard.name);
                        }
                        continue;
                    }
                    else if (deedCard.name == "Dispatch Messengers")
                    {
                        var count = 0;
                        for (var l = 0; l < game.GameMap.LocationsForGame.Count; l++)
                        {
                            var location = game.GameMap.LocationsForGame[l];
                            if (location.troopsByColor[color] >= 1)
                            {
                                count++;
                            }
                        }
                        if (count >= 8)
                        {
                            game.AccomplishAndRedeemDeed(player, deedCard.name);
                        }
                        continue;
                    }
                    else if (deedCard.name == "Capital City")
                    {
                        var found = false;
                        for (var l = 0; l < game.GameMap.LocationsForGame.Count; l++)
                        {
                            var location = game.GameMap.LocationsForGame[l];
                            if (location.DoesPlayerHaveThisBuilding(color, "stronghold") &&
                                location.DoesPlayerHaveThisBuilding(color, "church") &&
                                location.DoesPlayerHaveThisBuilding(color, "market"))
                            {
                                found = true;
                            }
                        }
                        if (found)
                        {
                            game.AccomplishAndRedeemDeed(player, deedCard.name);
                        }
                        continue;
                    }

                    var costs = deedCard.costs;
                    for (var c = 0; c < costs.Count; c++)
                    {
                        if (!canFulfill)
                        {
                            continue;
                        }
                        if (costs[c] == "coin")
                        {
                            if (money < 1)
                            {
                                Console.WriteLine("ai playDeedCard(): can't pay a coin");
                                canFulfill = false;
                            }
                            else
                            {
                                Console.WriteLine("ai playDeedCard(): can pay a coin");
                                money--;
                            }
                        }
                        else if (costs[c] == "wood")
                        {
                            if (wood < 1)
                            {
                                Console.WriteLine("ai playDeedCard(): can't pay wood");
                                canFulfill = false;
                            }
                            else
                            {
                                wood--;
                            }
                        }
                        else if (costs[c] == "stone")
                        {
                            if (stone < 1)
                            {
                                Console.WriteLine("ai playDeedCard(): can't pay stone");
                                canFulfill = false;
                            }
                            else
                            {
                                stone--;
                            }
                        }
                        else if (costs[c] == "fish")
                        {
                            if (fish < 1)
                            {
                                Console.WriteLine("ai playDeedCard(): can't pay fish");
                                canFulfill = false;
                            }
                            else
                            {
                                fish--;
                            }
                        }
                        else if (costs[c] == "honey")
                        {
                            if (honey < 1)
                            {
                                Console.WriteLine("ai playDeedCard(): can't pay honey");
                                canFulfill = false;
                            }
                            else
                            {
                                honey--;
                            }
                        }
                        else if (costs[c] == "fur")
                        {
                            if (fur < 1)
                            {
                                Console.WriteLine("ai playDeedCard(): can't pay fur");
                                canFulfill = false;
                            }
                            else
                            {
                                fur--;
                            }
                        }
                        else if (costs[c] == "schemeCard")
                        {
                            if (schemeCards.Count < 1)
                            {
                                Console.WriteLine("ai playDeedCard(): can't pay a schemeCard");
                                canFulfill = false;
                            }
                            else
                            {
                                var schemeCard = schemeCards[0];
                                schemeCards.RemoveAt(0);
                                // TODO: The scheme card should only be put on the discard pile, if canFulfill.
                                // discardSchemeCard(schemeCard);
                            }
                        }
                        else if (costs[c] == "resource")
                        {
                            var canPay = false;
                            if (tradeBoon > 0)
                            {
                                tradeBoon--;
                                canPay = true;
                            }
                            else if (stone > 0)
                            {
                                stone--;
                                canPay = true;
                            }
                            else if (wood > 0)
                            {
                                wood--;
                                canPay = true;
                            }
                            else if (fish > 0)
                            {
                                fish--;
                                canPay = true;
                            }
                            else if (honey > 0)
                            {
                                honey--;
                                canPay = true;
                            }
                            else if (fur > 0)
                            {
                                fur--;
                                canPay = true;
                            }
                            if (!canPay)
                            {
                                Console.WriteLine("ai playDeedCard(): can't pay a resource");
                                canFulfill = false;
                            }
                        }
                        else
                        {
                            Console.WriteLine("ai playDeedCard(): can't pay " + costs[c]);
                            canFulfill = false;
                        }
                    }
                    Console.WriteLine("ai playDeedCard(): canFulfill=" + canFulfill + ", player=" + player.Color + ", deedCard=" + deedCard);
                    if (canFulfill)
                    {
                        Console.WriteLine("ai playDeedCard(): check achievements, player=" + player.Color + ", deedCard=" + deedCard);
                        var achievements = deedCard.achievements;
                        for (var a = 0; a < achievements.Count; a++)
                        {
                            if (!canFulfill)
                            {
                                continue;
                            }
                            if (achievements[a] == "defeatRebel")
                            {
                                if (rebels < 1)
                                {
                                    canFulfill = false;
                                }
                                else
                                {
                                    rebels--;
                                }
                            }
                            else if (achievements[a] == "firstPlayer")
                            {
                                if (!player.isNextFirstPlayer)
                                {
                                    canFulfill = false;
                                }
                            }
                            else
                            {
                                canFulfill = false;
                            }
                        }
                    }
                    if (canFulfill)
                    {
                        Console.WriteLine("ai playDeedCard(): canFulfill, player=" + player.Color + ", deedCard=" + deedCard);
                        player.boat.capturedRebels = rebels;
                        player.boat.money = money;
                        player.boat.goodsOnDock["stone"] = stone;
                        player.boat.goodsOnDock["wood"] = wood;
                        player.boat.goodsOnDock["fish"] = fish;
                        player.boat.goodsOnDock["honey"] = honey;
                        player.boat.goodsOnDock["fur"] = fur;
                        player.boat.goodsOnDock["tradeBoon"] = tradeBoon;
                        player.schemeCards = schemeCards;
                        game.AccomplishAndRedeemDeed(player, deedCard.name);
                    }
                }
            }
        }

        private bool Muster(RurikGame game, Player player)
        {
            Console.WriteLine("ai muster(): player=" + player.Color);
            var tookAction = false;
            var color = player.Color;
            //var aiCard = player.aiCard;
            //var locationNames = aiCard.locationOrder.split(",");

            while (player.troopsToDeploy > 0 && player.supplyTroops + player.supplyLeader > 0)
            {
                var occupyMap = DetermineOccupiedLocations(player, game.GameMap.LocationsForGame);
                var occupies = occupyMap["occupies"];
                var locationName = default(string);
                if (occupies.Count > 0)
                {
                    var occupiesButDoesNotRule = occupyMap["occupiesButDoesNotRule"];
                    if (occupiesButDoesNotRule.Count > 0)
                    {
                        var random = new Random();
                        var r = random.Next(occupiesButDoesNotRule.Count);
                        locationName = occupiesButDoesNotRule[r];
                    }
                    else
                    {
                        var rules = occupyMap["rules"];
                        var random = new Random();
                        var r = random.Next(rules.Count);
                        locationName = rules[r];
                    }
                }
                else
                {
                    // This should handle cases when wiped off the board.
                    var random = new Random();
                    var r = random.Next(game.GameMap.LocationsForGame.Count);
                    locationName = game.GameMap.LocationsForGame[r].name;
                }
                game.BeginActionPhaseAction(color, "musterAction");
                game.Muster(color, locationName, 1);
                tookAction = true;
            }
            return tookAction;
        }

        private bool Tax(RurikGame game, Player player)
        {
            Console.WriteLine("ai tax(): player=" + player.Color);
            var tookAction = false;
            var color = player.Color;
            var canTax = true;
            var isSviatopolk = player.leader.name == "Sviatopolk";
            var isYaroslav = player.leader.name == "Yaroslav";

            while (player.taxActions > 0 && canTax)
            {
                // Priorities:
                // locations with goods remaining
                // locations with goods needed
                // location ruled with market
                // location ruled
                // location occupied with market
                // location occupied

                // goodsNeeded = goodsOnBoatSlots - goodsOnBoat
                var goodsNeeded = new HashSet<string>();
                var boat = player.boat;
                var goods = new[] { "stone", "wood", "fish", "fur", "honey" };
                for (var i = 0; i < goods.Length; i++)
                {
                    if (boat.goodsOnBoatSlots[goods[i]] - boat.goodsOnBoat[goods[i]] > 0)
                    {
                        goodsNeeded.Add(goods[i]);
                    }
                }

                var locationsWithGoods = new List<Location>();
                var locationsRuled = new List<Location>();
                var locationsOccupied = new List<Location>();
                var locationsWithMarket = new List<Location>();
                var locationsWithNeededGoods = new List<Location>();

                for (var i = 0; i < game.GameMap.LocationsForGame.Count; i++)
                {
                    var location = game.GameMap.LocationsForGame[i];
                    if (location.resourceCount > 0)
                    {
                        locationsWithGoods.Add(location);
                        if (goodsNeeded.Contains(location.defaultResource))
                        {
                            locationsWithNeededGoods.Add(location);
                        }
                    }
                    if (location.DoesPlayerHaveMarket(color))
                    {
                        locationsWithMarket.Add(location);
                    }
                    if (location.DoesRule(color, isSviatopolk, isYaroslav))
                    {
                        locationsRuled.Add(location);
                    }
                    if (location.DoesOccupy(color))
                    {
                        locationsOccupied.Add(location);
                    }
                }

                canTax = false;
                var locationName = default(string);
                var intersectionSet = locationsWithGoods.Intersect(locationsOccupied).ToList();
                if (intersectionSet.Count > 0)
                {
                    var intersectionSetGoodsNeeded = intersectionSet.Intersect(locationsWithNeededGoods).ToList();
                    if (intersectionSetGoodsNeeded.Count > 0)
                    {
                        var intersectionSetRuled = intersectionSetGoodsNeeded.Intersect(locationsRuled).ToList();
                        if (intersectionSetRuled.Count > 0)
                        {
                            var intersectionSetRuledWithMarket = intersectionSetRuled.Intersect(locationsWithMarket).ToList();
                            if (intersectionSetRuledWithMarket.Count > 0)
                            {
                                canTax = true;
                                Console.WriteLine("tax(): intersectionSetRuledWithMarket=" + intersectionSetRuledWithMarket.Count);
                                locationName = intersectionSetRuledWithMarket[0].name;
                            }
                            else
                            {
                                canTax = true;
                                Console.WriteLine("tax(): intersectionSetRuled=" + intersectionSetRuled.Count);
                                locationName = intersectionSetRuled[0].name;
                            }
                        }
                        else
                        {
                            if (player.taxActions > 1)
                            {
                                canTax = true;
                                Console.WriteLine("tax(): intersectionSetGoodsNeeded=" + intersectionSetGoodsNeeded.Count);
                                locationName = intersectionSetGoodsNeeded[0].name;
                            }
                        }
                    }
                    else
                    {
                        if (player.taxActions > 1)
                        {
                            canTax = true;
                            Console.WriteLine("tax(): intersectionSet=" + intersectionSet.Count);
                            locationName = intersectionSet[0].name;
                        }
                    }
                }
                if (canTax == true)
                {
                    game.BeginActionPhaseAction(color, "taxAction");
                    game.Tax(color, locationName);
                    tookAction = true;
                }
            }
            return tookAction;
        }

        private string PickTarget(Player player, Location location, List<string> firstPlacePlayerColors, bool convert = false)
        {
            var target = default(string);
            var possibleTargets = new List<string>();
            if (location.rebelRewards.Count > 0)
            {
                possibleTargets.Add("rebel");
            }
            var colors = new[] { "blue", "white", "red", "yellow" };
            for (var i = 0; i < colors.Length; i++)
            {
                var color = colors[i];
                if (player.Color == color)
                {
                    continue;
                }
                if (location.troopsByColor[color] > 0 || (convert == false && location.leaderByColor[color] > 0))
                {
                    possibleTargets.Add(color);
                }
            }
            var firstPlaceTargets = firstPlacePlayerColors.Intersect(possibleTargets).ToList();
            if (firstPlaceTargets.Count > 0)
            {
                target = firstPlaceTargets[0];
            }
            else
            {
                if (possibleTargets.Count > 0)
                {
                    target = possibleTargets[possibleTargets.Count - 1];
                }
            }
            return target;
        }

        private bool Attack(RurikGame game, Player player)
        {
            Console.WriteLine("ai attack(): player=" + player.Color);
            var tookAction = false;
            var color = player.Color;
            var canAttack = true;
            // TODO: var clonedGame = game.Clone();
            //var endGameStats = clonedGame.CalculateEndGameStats();
            //var firstPlacePlayerColors = clonedGame.GetFirstPlacePlayers(endGameStats);

            while (player.attackActions > 0 && canAttack)
            {
                // Priorities:
                // build or tax strategy - prefer to attack where rule
                // attack-move strategy - prefer to attack where do not rule
                // prefer to attack where can rule (0 -> +1 diff)
                // prefer to attack where can disrupt rule of another player (-1 -> 0 diff)
                // prefer to attack first place player
                // if in first place, prefer to attack second place player
                // prefer to attack another player vs. rebel 
                // prefer to attack human player vs. ai player

                canAttack = false;
                var occupyMap = DetermineOccupiedLocations(player, game.GameMap.LocationsForGame);
                var occupiesButDoesNotRule = occupyMap["occupiesButDoesNotRule"];
                var rules = occupyMap["rules"];
                var occupies = occupyMap["occupies"];
                var locationsWithEnemies = new List<string>();
                var locationsWithRebels = new List<string>();
                var locationsWithOtherPlayers = new List<string>();
                for (var i = 0; i < game.GameMap.LocationsForGame.Count; i++)
                {
                    var location = game.GameMap.LocationsForGame[i];
                    if (location.rebelRewards.Count > 0)
                    {
                        locationsWithEnemies.Add(location.name);
                        locationsWithRebels.Add(location.name);
                    }
                    if (location.HasPlayerEnemy(color))
                    {
                        locationsWithOtherPlayers.Add(location.name);
                    }
                }
                var locationName = default(string);
                var target = default(string);
                if (locationsWithEnemies.Count > 0)
                {
                    var occupiesWithEnemies = occupies.Intersect(locationsWithEnemies).ToList();
                    Console.WriteLine("attack(): occupiesWithEnemies=" + string.Join(",", occupiesWithEnemies));
                    if (occupiesWithEnemies.Count > 0)
                    {
                        var random = new Random();
                        var r = random.Next(occupiesWithEnemies.Count);
                        locationName = occupiesWithEnemies[r];
                        var location = game.GameMap.LocationByName[locationName];
                        // TODO: target = PickTarget(player, location, firstPlacePlayerColors);
                        Console.WriteLine("attack(): target=" + target);
                        if (target != null)
                        {
                            canAttack = true;
                        }
                    }

                    // TODO: implement preferences
                    // attack-move, build, tax
                    if (player.aiStrategy == "attack-move")
                    {
                        // Prefer to attack where they do not rule.
                        if (occupiesButDoesNotRule.Count > 0)
                        {
                            var noRule = occupiesButDoesNotRule.Intersect(locationsWithEnemies).ToList();
                            if (noRule.Count > 0)
                            {

                            }
                        }
                        else if (rules.Count > 0)
                        {

                        }
                    }
                    else
                    {
                        // Prefer to attack where they do rule.
                    }
                }
                if (canAttack)
                {
                    game.BeginActionPhaseAction(color, "attackAction");
                    game.Attack(color, locationName, target, 1);
                    tookAction = true;
                }
            }
            return tookAction;
        }

        private bool Move(RurikGame game, Player player)
        {
            Console.WriteLine("ai move(): player=" + player.Color);
            var tookAction = false;
            var color = player.Color;
            var canMove = true;
            var isSviatopolk = player.leader.name == "Sviatopolk";
            var isYaroslav = player.leader.name == "Yaroslav";
            while (player.moveActions > 0 && canMove)
            {
                canMove = false;
                // Priorities:
                // build or tax strategy - prefer to move to a location they do not occupy
                // attack-move strategy - prefer to move to a location they do not rule
                // will not move if it will make someone else gain rule
                // will not move if it will make them lose rule
                // prefers to move leader over troop
                var occupyMap = DetermineOccupiedLocations(player, game.GameMap.LocationsForGame);
                var occupiesButDoesNotRule = occupyMap["occupiesButDoesNotRule"];
                var rules = occupyMap["rules"];
                var occupies = occupyMap["occupies"];
                var fromLocationName = default(string);
                var toLocationName = default(string);
                var moveLeader = false;
                var locationsForGameNames = game.GameMap.GetLocationsForGameNames();
                for (var i = 0; i < rules.Count; i++)
                {
                    moveLeader = false;
                    fromLocationName = rules[i];
                    Console.WriteLine("move(): fromLocationName=" + fromLocationName);
                    var fromLocation = game.GameMap.LocationByName[fromLocationName];
                    if (fromLocation.leaderByColor[color] > 0)
                    {
                        Console.WriteLine("move(): leader in fromLocation=" + fromLocation);
                        moveLeader = true;
                    }
                    var excess = fromLocation.CalculateExcessTroopsForRule(color);
                    if (excess > 1)
                    {
                        var neighbors = fromLocation.neighbors;
                        Console.WriteLine("move(): neighbors=" + string.Join(",", neighbors));
                        var validNeighbors = locationsForGameNames.Intersect(neighbors).ToList();
                        Console.WriteLine("move(): validNeighbors=" + string.Join(",", validNeighbors));
                        for (var n = 0; n < validNeighbors.Count; n++)
                        {
                            var neighbor = game.GameMap.LocationByName[validNeighbors[n]];
                            if (player.aiStrategy == "attack-move")
                            {
                                if (neighbor.DoesOccupy(color) && !neighbor.DoesRule(color, isSviatopolk, isYaroslav))
                                {
                                    toLocationName = validNeighbors[n];
                                    canMove = true;
                                    break;
                                }
                            }
                            else
                            {
                                if (!neighbor.DoesOccupy(color))
                                {
                                    toLocationName = validNeighbors[n];
                                    canMove = true;
                                    break;
                                }
                            }
                        }
                        if (!canMove)
                        {
                            var random = new Random();
                            var r = random.Next(validNeighbors.Count);
                            toLocationName = validNeighbors[r];
                            canMove = true;
                        }
                        break;
                    }
                }
                if (canMove)
                {
                    game.BeginActionPhaseAction(color, "moveAction");
                    game.Move(color, fromLocationName, toLocationName, 1, moveLeader);
                    tookAction = true;
                }
            }
            return tookAction;
        }

        private List<string> CheckLocationForBuildingsToPlay(Location location, Player player)
        {
            Console.WriteLine("checkLocationForBuildingsToPlay(): " + location + " " + player.Color);
            var isSviatopolk = player.leader.name == "Sviatopolk";
            var isYaroslav = player.leader.name == "Yaroslav";
            var locationBuildings = new List<string>();
            for (var i = 0; i < location.buildings.Count; i++)
            {
                locationBuildings.Add(location.buildings[i].name);
            }
            var allBuildings = new[] { "stable", "tavern", "church", "market", "stronghold" };
            var playerBuildings = new List<string>();
            for (var i = 0; i < allBuildings.Length; i++)
            {
                var building = allBuildings[i];
                if (player.buildings[building] > 0)
                {
                    playerBuildings.Add(building);
                }
            }
            var buildingsAllowed = new List<string>();
            //Console.WriteLine("checkLocationForBuildingsToPlay(): locationBuildings=" + JSON.stringify(locationBuildings));
            //Console.WriteLine("checkLocationForBuildingsToPlay(): playerBuildings=" + JSON.stringify(playerBuildings));
            var buildingsAllowedInLocation = allBuildings.Except(locationBuildings).ToList();
            Console.WriteLine("checkLocationForBuildingsToPlay(): buildingsAllowedInLocation=" + string.Join(",", buildingsAllowedInLocation));
            if (buildingsAllowedInLocation.Count > 0)
            {
                var candidateBuildings = buildingsAllowedInLocation.Intersect(playerBuildings).ToList();
                Console.WriteLine("checkLocationForBuildingsToPlay(): candidateBuildings=" + string.Join(",", candidateBuildings));
                for (var i = 0; i < candidateBuildings.Count; i++)
                {
                    var candidate = candidateBuildings[i];
                    if (candidate == "stable" && location.DoesRule(player.Color, isSviatopolk, isYaroslav))
                    {
                        if (location.CalculateExcessTroopsForRule(player.Color) >= 3)
                        {
                            buildingsAllowed.Add(candidate);
                            break;
                        }
                    }
                    if (candidate == "tavern" && locationBuildings.Count == 2)
                    {
                        buildingsAllowed.Add(candidate);
                        break;
                    }
                    if (candidate == "church" && location.HasEnemy(player.Color))
                    {
                        buildingsAllowed.Add(candidate);
                        break;
                    }
                    if (candidate == "stronghold" || candidate == "market")
                    {
                        buildingsAllowed.Add(candidate);
                    }
                }
            }
            Console.WriteLine("checkLocationForBuildingsToPlay(): buildingsAllowed=" + string.Join(",", buildingsAllowed));
            return buildingsAllowed;
        }

        private (string location, string building) PickLocationAndBuilding(GameMap gameMap, List<string> locationNames, Player player, List<string> buildingOrder)
        {
            var location = default(string);
            var building = default(string);
            for (var i = 0; i < locationNames.Count; i++)
            {
                location = locationNames[i];
                var locationObj = gameMap.LocationByName[location];
                var buildingsAllowed = CheckLocationForBuildingsToPlay(locationObj, player);
                if (buildingsAllowed.Count > 0)
                {
                    // TODO: consider buildingOrder or at least use random
                    building = buildingsAllowed[0];
                    break;
                }
            }
            return (location, building);
        }

        private bool Build(RurikGame game, Player player)
        {
            Console.WriteLine("ai build(): player=" + player.Color);
            var tookAction = false;
            var color = player.Color;
            var aiCard = player.aiCard;
            var gameMap = game.GameMap;
            var isSviatopolk = player.leader.name == "Sviatopolk";
            var isYaroslav = player.leader.name == "Yaroslav";
            var buildingOrder = aiCard.strategies[player.aiStrategy].buildingOrder;
            var canBuild = true;

            var locationsForGame = gameMap.LocationsForGame;
            while (player.buildActions > 0 && canBuild)
            {
                canBuild = false;
                // Priorities:
                // build where they rule
                // build where there are none of their buildings
                // build where there are enemy or neutral troops
                // build where there are no enemy or neutral troops
                // TODO: build to improve adjaceny
                var rules = new List<string>();
                var occupies = new List<string>();
                var enemies = new List<string>();
                var noPlayerBuildings = new List<string>();
                var openings = new List<string>();
                var targetToConvert = default(string);
                for (var i = 0; i < locationsForGame.Count; i++)
                {
                    var location = locationsForGame[i];
                    if (location.DoesRule(color, isSviatopolk, isYaroslav))
                    {
                        rules.Add(location.name);
                    }
                    if (location.DoesOccupy(color))
                    {
                        occupies.Add(location.name);
                    }
                    if (location.HasPlayerEnemy(color))
                    {
                        enemies.Add(location.name);
                    }
                    if (!location.DoesPlayerHaveBuilding(color))
                    {
                        noPlayerBuildings.Add(location.name);
                    }
                    if (location.buildings.Count < 3)
                    {
                        openings.Add(location.name);
                    }
                }
                var locationName = default(string);
                var buildingName = default(string);
                var random = new Random();
                var r = random.Next(occupies.Count);
                if (occupies.Count > 0)
                {
                    var location = occupies[r];
                    var locationObj = gameMap.LocationByName[location];
                    var buildingsAllowed = CheckLocationForBuildingsToPlay(locationObj, player);
                    if (buildingsAllowed.Count > 0)
                    {
                        buildingName = buildingsAllowed[0];
                        locationName = location;
                        canBuild = true;
                    }
                }
                if (canBuild)
                {
                    game.BeginActionPhaseAction(color, "buildAction");
                    game.Build(color, locationName, buildingName);
                    tookAction = true;
                }
            }
            return tookAction;
        }

        private int CalculateDecisionValue(RurikGame game, Player player)
        {
            // Placeholder for decision value calculation logic
            return 0;
        }

        private Dictionary<string, List<string>> DetermineOccupiedLocations(Player player, List<Location> locations)
        {
            var result = new Dictionary<string, List<string>>
            {
                { "occupiesButDoesNotRule", new List<string>() },
                { "rules", new List<string>() },
                { "occupies", new List<string>() }
            };

            var isSviatopolk = player.leader.name == "Sviatopolk";
            var isYaroslav = player.leader.name == "Yaroslav";

            foreach (var location in locations)
            {
                if (location.DoesOccupy(player.Color))
                {
                    result["occupies"].Add(location.name);
                    if (location.DoesRule(player.Color, isSviatopolk, isYaroslav))
                    {
                        result["rules"].Add(location.name);
                    }
                    else
                    {
                        result["occupiesButDoesNotRule"].Add(location.name);
                    }
                }
            }

            return result;
        }

        private void CreateAiCards()
        {
            // This method would populate the aiCards list with all the strategy cards
            // Implementation would be similar to the JavaScript version
            // For brevity, we're not implementing all the cards here
        }

        private void SchemeFirstPlayer(RurikGame game, Player player)
        {
            // Implementation for scheme first player logic
        }

        private void DrawSchemeCards(RurikGame game, Player player)
        {
            // Implementation for drawing scheme cards
        }

        private void SelectSchemeCard(RurikGame game, Player player)
        {
            // Implementation for selecting scheme cards
        }

        private void EndGame(RurikGame game, Player player)
        {
            // Implementation for end game logic
        }

        private void TakeDeedCard(RurikGame game, Player player)
        {
            // Implementation for taking deed card
        }

        private void TakeDeedCardForActionPhase(RurikGame game, Player player)
        {
            // Implementation for taking deed card for action phase
        }
    }
}
