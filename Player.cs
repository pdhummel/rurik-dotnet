using System;
using System.Collections.Generic;

namespace rurik
{
    public class Player
    {
        public string name { get; set; }
        public string Color { get; set; }
        public string tablePosition { get; set; } // N, E, S, W
        public bool isPlayerAi { get; set; }
        public BoatMat boat { get; set; }
        public Leader leader { get; set; }
        public List<SecretAgendaCard> secretAgenda { get; set; } // Assuming object type for cards
        public List<SecretAgendaCard> temporarySecretAgenda { get; set; } // Assuming object type for cards
        public List<DeedCard> deedCards { get; set; } // Assuming object type for cards
        public int victoryPoints { get; set; }
        //public int capturedRebels { get; set; }

        public List<SchemeCard> schemeCards { get; set; } // Assuming object type for cards
        public List<SchemeCard> temporarySchemeCards { get; set; } // Assuming object type for cards
        public int returnSchemeDeck { get; set; }
        public int schemeCardsToDraw { get; set; }
        public bool canKeepSchemeCard { get; set; }

        public int troopsToDeploy { get; set; }

        public List<string> advisors { get; set; }
        public Dictionary<int, List<AuctionSpace>> advisorsToAuctionSpace { get; set; } // Key: advisor id, Value: list of auction spaces
        public int advisorCountForTurn { get; set; }

        public bool tookMainActionForTurn { get; set; }
        public int schemeCardsCanPlay { get; set; }
        public object oneTimeSchemeCard { get; set; }
        public bool accomplishedDeedForTurn { get; set; }
        public bool convertedGoodsForTurn { get; set; }
        public int taxActions { get; set; }
        public int buildActions { get; set; }
        public int moveActions { get; set; }
        public int attackActions { get; set; }
        public Dictionary<string, int> moveActionsFromLocation { get; set; } // Key: location name, Value: number of actions
        public int moveAnywhereActions { get; set; }
        public bool finishedRound { get; set; }

        public bool isFirstPlayer { get; set; }
        public bool isNextFirstPlayer { get; set; }

        // 12 - 3 to deploy.
        public int supplyTroops { get; set; }
        public int supplyLeader { get; set; }
        public Dictionary<string, int> buildings { get; set; } // Key: building name, Value: count

        public AiStrategyCard aiCard { get; set; }
        public string aiStrategy { get; set; }
        public bool usedGlebAttack { get; set; }
        public bool usedTheofanaTax { get; set; }
        public bool usedMariaMuster { get; set; }
        public bool usedPredslavaMove { get; set; }
        //public bool glebAttackInProgress { get; set; }

        public Player(string name, string color, string tablePosition, bool isPlayerAi)
        {
            this.name = name;
            this.Color = color;
            this.tablePosition = tablePosition;
            this.isPlayerAi = isPlayerAi;
            this.boat = new BoatMat();
            this.leader = null;
            this.secretAgenda = new List<SecretAgendaCard>();
            this.temporarySecretAgenda = new List<SecretAgendaCard>();
            this.deedCards = new List<DeedCard>();
            this.victoryPoints = 0;
            //this.capturedRebels = 0;

            this.schemeCards = new List<SchemeCard>();
            this.temporarySchemeCards = new List<SchemeCard>();
            this.returnSchemeDeck = 1;
            this.schemeCardsToDraw = 0;
            this.canKeepSchemeCard = false;

            this.troopsToDeploy = 3;

            this.advisors = new List<string>();
            this.advisorsToAuctionSpace = new Dictionary<int, List<AuctionSpace>>();
            this.advisorCountForTurn = 0;

            this.tookMainActionForTurn = false;
            this.schemeCardsCanPlay = 1;
            this.oneTimeSchemeCard = null;
            this.accomplishedDeedForTurn = false;
            this.convertedGoodsForTurn = false;
            this.taxActions = 0;
            this.buildActions = 0;
            this.moveActions = 0;
            this.attackActions = 0;
            this.moveActionsFromLocation = new Dictionary<string, int>();
            this.moveAnywhereActions = 0;
            this.finishedRound = false;

            this.isFirstPlayer = false;
            this.isNextFirstPlayer = false;

            // 12 - 3 to deploy.
            this.supplyTroops = 9;
            this.supplyLeader = 0;
            this.buildings = new Dictionary<string, int>();
            this.buildings["church"] = 3;
            this.buildings["stronghold"] = 3;
            this.buildings["market"] = 3;
            this.buildings["tavern"] = 2;
            this.buildings["stable"] = 2;

            this.aiCard = null;
            this.aiStrategy = null;
            this.usedGlebAttack = false;
            this.usedTheofanaTax = false;
            this.usedMariaMuster = false;
            this.usedPredslavaMove = false;
            //this.glebAttackInProgress = false;
        }

        public void resetMoveActionsFromLocation(List<object> locations) // Assuming locations is a list of objects with a 'name' property
        {
            foreach (var location in locations)
            {
                // Assuming location has a 'name' property
                this.moveActionsFromLocation[location.ToString()] = 0; // This will need to be adjusted based on actual location object structure
            }
        }

        public void setLeader(Leader leader)
        {
            this.leader = leader;
        }

        public void addSecretAgenda(SecretAgendaCard card)
        {
            this.temporarySecretAgenda.Add(card);
        }

        public void setAdvisors(List<string> advisors)
        {
            this.advisors = advisors;
        }

        public bool isAdvisorAvailable(string advisor)
        {
            foreach (var adv in this.advisors)
            {
                if (advisor == adv)
                {
                    return true;
                }
            }
            return false;
        }

        public void useAdvisor(string advisor)
        {
            var newAdvisors = new List<string>();
            var advisorFound = false;
            foreach (var adv in this.advisors)
            {
                if (advisor == adv && !advisorFound)
                {
                    advisorFound = true;
                }
                else
                {
                    newAdvisors.Add(adv);
                }
            }
            this.advisors = newAdvisors;
        }

        public int getTotalMoveActions()
        {
            var moveActions = this.moveActions;
            foreach (var kvp in this.moveActionsFromLocation)
            {
                moveActions = moveActions + kvp.Value;
            }
            return moveActions;
        }

        public bool hasSchemeCard(string schemeCardId)
        {
            foreach (var card in this.schemeCards)
            {
                // Assuming card has an 'id' property
                if (schemeCardId == card.ToString()) // This will need to be adjusted based on actual card object structure
                {
                    return true;
                }
            }
            return false;
        }
    }
}
