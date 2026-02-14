using System;
using System.Collections.Generic;
using System.Linq;

namespace rurik
{
    public class Cards
    {
        public Dictionary<string, DeedCard> allDeedCards { get; set; }
        public List<DeedCard> displayedDeedCards { get; set; }
        public List<DeedCard> deedCardDeck { get; set; }
        public Dictionary<string, SecretAgendaCard> allSecretAgendaCards { get; set; }
        public List<SecretAgendaCard> secretAgendaCards { get; set; }
        public Dictionary<string, SecretAgendaCard> availableSecretAgendaCards { get; set; }
        public Dictionary<string, SchemeCard> schemeCardIds { get; set; }
        public List<SchemeCard> discardedSchemeCards { get; set; }
        public List<SchemeCard> schemeDeck1 { get; set; }
        public List<SchemeCard> schemeDeck2 { get; set; }
        public List<SchemeCard> schemeCardList { get; set; }
        public List<SchemeCard> temporarySchemeCards { get; set; }

        public Cards()
        {
            this.allDeedCards = new Dictionary<string, DeedCard>();
            this.displayedDeedCards = new List<DeedCard>();
            this.deedCardDeck = new List<DeedCard>();
            this.allSecretAgendaCards = new Dictionary<string, SecretAgendaCard>();
            this.secretAgendaCards = new List<SecretAgendaCard>();
            this.availableSecretAgendaCards = new Dictionary<string, SecretAgendaCard>();
            this.schemeCardIds = new Dictionary<string, SchemeCard>();
            this.discardedSchemeCards = new List<SchemeCard>();
            this.schemeDeck1 = new List<SchemeCard>();
            this.schemeDeck2 = new List<SchemeCard>();
            this.schemeCardList = new List<SchemeCard>();
            this.temporarySchemeCards = new List<SchemeCard>();

            this.setupSecretAgendaCards();

            // TODO: handle scheme decks being exhausted
            this.setupSchemeCards();

            this.setupDeedCards();
        }

        public void setupDeedCards()
        {
            this.allDeedCards = new Dictionary<string, DeedCard>();
            this.displayedDeedCards = new List<DeedCard>();
            this.deedCardDeck = new List<DeedCard>();
            var shuffledDeedCardDeck = new List<DeedCard>();

            // These have simple costs to fulfill
            this.addDeedCard("Generous Prince", 1, "Pay 4 coins.", new List<string> { "muster", "muster" }, new List<string> { "coin", "coin", "coin", "coin" }, new List<string>(), true, true);
            this.addDeedCard("Splendid Feast", 1, "Pay fish, honey, 2 coins.", new List<string> { "scheme2cards" }, new List<string> { "fish", "honey", "coin", "coin" }, new List<string>(), true, true);
            this.addDeedCard("Send Gifts", 1, "Pay fish, fur, 2 coins.", new List<string> { "move", "move" }, new List<string> { "fish", "fur", "coin", "coin" }, new List<string>(), true, true);
            this.addDeedCard("Wilderness Forts", 1, "Pay stone, fur, wood.", new List<string> { "build" }, new List<string> { "stone", "fur", "wood" }, new List<string>(), true, true);
            this.addDeedCard("Hire Mercenaries", 1, "Pay fish, stone, 2 coins.", new List<string> { "attackMinusScheme" }, new List<string> { "fish", "stone", "coin", "coin" }, new List<string>(), true, true);

            // These are cost based, but requires decision for which resource to pay.
            //this.addDeedCard("Trade Route", 1, "Pay 3 different resources.", new List<string> { "coin", "coin" }, new Dictionary<string, object> { { "differentResource", new List<string> { "resource", "resource", "resource" } } }, new List<string>(), true, true);
            this.addDeedCard("Reward Laborers", 1, "Pay 2 resources and 2 coins.", new List<string> { "scheme2Cards" }, new List<string> { "resource", "resource", "coin", "coin" }, new List<string>(), true, true);
            //this.addDeedCard("Hoard", 1, "Pay 3 of the same resource.", new List<string> { "scheme2Cards" }, new Dictionary<string, object> { { "sameResource", new List<string> { "resource", "resource", "resource" } } }, new List<string>(), true, true);

            // These require decisions for sacrificing scheme card, buildings, or troops.
            this.addDeedCard("Great Library", 1, "Pay a scheme card and stone.", new List<string> { "build" }, new List<string> { "schemeCard", "stone" }, new List<string>(), true, true);
            this.addDeedCard("Master Beekeeper", 2, "Pay a scheme card and 2 honey.", new List<string> { "build" }, new List<string> { "schemeCard", "honey", "honey" }, new List<string>(), true, true);
            this.addDeedCard("Honorable Prince", 1, "Pay a scheme card and 3 coin.", new List<string> { "muster", "muster" }, new List<string> { "schemeCard", "coin", "coin", "coin" }, new List<string>(), true, true);
            this.addDeedCard("New Beginning", 1, "Sacrifice market, stronghold, and church.", new List<string> { "tax", "tax" }, new List<string> { "market", "stronghold", "church" }, new List<string>());
            this.addDeedCard("Retire Veterans", 1, "Remove 2 troops and pay 2 coins.", new List<string> { "scheme2Cards" }, new List<string> { "troop", "troop", "coin", "coin" }, new List<string>());
            this.addDeedCard("Victory March", 1, "Remove 2 troops and pay a scheme card.", new List<string> { "moveAnywhere", "moveAnywhere" }, new List<string> { "schemeCard", "troop", "troop" }, new List<string>());
            this.addDeedCard("Peace Maker", 1, "Remove 2 troops from a ruled region.", new List<string> { "scheme2Cards" }, new List<string> { "troop", "troop" }, new List<string>());
            //this.addDeedCard("Border Patrols", 1, "Remove 3 troops from different regions.", new List<string> { "moveAnywhere", "moveAnywhere" }, new Dictionary<string, object> { { "differentRegions", new List<string> { "troop", "troop", "troop" } } }, new List<string>());

            // These include some achievements which must be fulfilled and may also have costs.
            this.addDeedCard("Enforce Peace", 1, "Pay wood and honey and defeat 2 rebels.", new List<string> { "attackMinusScheme" }, new List<string> { "wood", "honey" }, new List<string> { "defeatRebel", "defeatRebel" }, true, true);
            this.addDeedCard("Law Giver", 1, "Pay 2 coins and defeat 3 rebels.", new List<string> { "scheme2Cards" }, new List<string> { "coin", "coin" }, new List<string> { "defeatRebel", "defeatRebel", "defeatRebel" }, true, true);
            this.addDeedCard("Mead Brewery", 1, "Pay honey and build 2 taverns.", new List<string> { "tax" }, new List<string> { "honey" }, new List<string> { "tavern", "tavern" }, true, true);
            //this.addDeedCard("Establish Fortress", 1, "Pay 2 wood and build a stronghold and church in a region.", new List<string> { "scheme2Cards" }, new List<string> { "wood", "wood" }, new Dictionary<string, object> { { "sameRegion", new List<string> { "stronghold", "church" } } }, true, true);
            //this.addDeedCard("Besiege Citadel", 1, "Pay 2 coins and rule a region with a stronghold.", new List<string> { "warTrack" }, new Dictionary<string, object> { { "sameRegion", new List<string> { "rule", "stronghold" } } }, true, true);
            this.addDeedCard("Horse Breeder", 1, "Pay wood and build 2 stables.", new List<string> { "muster" }, new List<string> { "wood" }, new List<string> { "stable", "stable" }, true, true);
            this.addDeedCard("Conquest", 1, "Rule 3 regions.", new List<string> { "move", "move" }, new List<string>(), new List<string> { "rule", "rule", "rule" }, true, true);
            //this.addDeedCard("Amass Forces", 1, "Have 6 troops in a region.", new List<string> { "move" }, new List<string>(), new Dictionary<string, object> { { "sameRegion", new List<string> { "troop", "troop", "troop", "troop", "troop", "troop" } } }, true, true);
            //this.addDeedCard("Tithe Payments", 1, "Have 3 churches in adjacent regions.", new List<string> { "coin", "coin", "coin" }, new List<string>(), new Dictionary<string, object> { { "adjacentRegions", new List<string> { "church", "church", "church" } } });
            //this.addDeedCard("Dispatch Messengers", 2, "Have troops in 8 regions.", new List<string> { "muster", "muster" }, new List<string>(), new Dictionary<string, object> { { "differentRegions", new List<string> { "troop", "troop", "troop", "troop", "troop", "troop", "troop", "troop" } } }, true, true);
            //this.addDeedCard("Create Republic", 2, "Rule Novgorod, Chernigov, Volyn.", new List<string> { "tax" }, new List<string>(), new Dictionary<string, object> { { "rule", new List<string> { "Novgorod", "Chernigov", "Volyn" } } }, true, true);
            //this.addDeedCard("Distant Rule", 2, "Rule Pereyaslavl, Polotsk, Rostov.", new List<string> { "warTrack" }, new List<string>(), new Dictionary<string, object> { { "rule", new List<string> { "Pereyaslavl", "Polotsk", "Rostov" } } }, true, true);
            //this.addDeedCard("Market Day", 2, "Have 3 markets with different resources.", new List<string> { "scheme2Cards" }, new List<string>(), new Dictionary<string, object> { { "differentResources", new List<string> { "market", "market", "market" } } });
            //this.addDeedCard("Defensive Belt", 1, "Have 3 strongholds in adjacent regions.", new List<string> { "attackMinusScheme" }, new List<string>(), new Dictionary<string, object> { { "adjacentRegions", new List<string> { "stronghold", "stronghold", "stronghold" } } });
            //this.addDeedCard("Capital City", 2, "Have a market, stronghold, and church in a single region.", new List<string> { "tax" }, new List<string>(), new Dictionary<string, object> { { "sameRegion", new List<string> { "market", "stronghold", "church" } } }, true, true);
            this.addDeedCard("Grand Hunter", 2, "Pay 2 fur and have the first player token.", new List<string> { "move" }, new List<string> { "fur", "fur" }, new List<string> { "firstPlayer" }, true, true);

            // shuffle the deed cards
            for (int i = 0; i < this.deedCardDeck.Count; i++)
            {
                int r = new Random().Next(this.deedCardDeck.Count);
                DeedCard deedCard = this.deedCardDeck[r];
                shuffledDeedCardDeck.Add(deedCard);
                this.deedCardDeck[r] = this.deedCardDeck[this.deedCardDeck.Count - 1];
                this.deedCardDeck.RemoveAt(this.deedCardDeck.Count - 1);
            }
            this.deedCardDeck = shuffledDeedCardDeck;

            var card = this.deedCardDeck[this.deedCardDeck.Count - 1];
            this.displayedDeedCards.Add(card);
            this.deedCardDeck.RemoveAt(this.deedCardDeck.Count - 1);
            card = this.deedCardDeck[this.deedCardDeck.Count - 1];
            this.displayedDeedCards.Add(card);
            this.deedCardDeck.RemoveAt(this.deedCardDeck.Count - 1);
            card = this.deedCardDeck[this.deedCardDeck.Count - 1];
            this.displayedDeedCards.Add(card);
            this.deedCardDeck.RemoveAt(this.deedCardDeck.Count - 1);
        }

        public void addDeedCard(string name, int victoryPoints, string requirementText, List<string> rewards = null, List<string> costs = null, List<string> achievements = null, bool canSolo = true, bool canAi = false)
        {
            var card = new DeedCard(name, victoryPoints, requirementText, rewards, costs, achievements, canSolo, canAi);
            this.allDeedCards[name] = card;
            this.deedCardDeck.Add(card);
        }

        public void takeDeedCard(Player player, string deedCardName)
        {
            Console.WriteLine("takeDeedCard(): " + player.Color + " " + deedCardName);
            var newDeedCardDisplay = new List<DeedCard>();
            for (int i = 0; i < this.displayedDeedCards.Count; i++)
            {
                if (this.displayedDeedCards[i].name == deedCardName)
                {
                    player.deedCards.Add(this.displayedDeedCards[i]);
                }
                else
                {
                    newDeedCardDisplay.Add(this.displayedDeedCards[i]);
                }
            }
            var nextDeedCard = this.deedCardDeck[this.deedCardDeck.Count - 1];
            newDeedCardDisplay.Add(nextDeedCard);
            this.deedCardDeck.RemoveAt(this.deedCardDeck.Count - 1);
            this.displayedDeedCards = newDeedCardDisplay;
        }

        public void setupSecretAgendaCards()
        {
            this.allSecretAgendaCards = new Dictionary<string, SecretAgendaCard>();
            this.secretAgendaCards = new List<SecretAgendaCard>();
            this.availableSecretAgendaCards = new Dictionary<string, SecretAgendaCard>();
            this.addSecretAgendaCard("Esteemed", "Occupy the most regions with your troops.", 2);
            this.addSecretAgendaCard("Conquering", "Finish in first place on the warfare track.", 2);
            this.addSecretAgendaCard("Prosperous", "Finish in first place on the trade track.", 2);
            this.addSecretAgendaCard("Capable", "Have the most goods.", 2);
            this.addSecretAgendaCard("Successful", "Finish in first place on the rule track.", 2);
            this.addSecretAgendaCard("Regal", "Have the most combined fur and honey.", 2);
            this.addSecretAgendaCard("Protective", "Rule the most structures.", 2);
            this.addSecretAgendaCard("Committed", "Finish in first place on the build track.", 2);
            this.addSecretAgendaCard("Dignified", "Accomplish the most deeds.", 2);
            this.addSecretAgendaCard("Courageous", "Defeat the most rebels.", 2);
            this.addSecretAgendaCard("Wealthy", "Have the most coins.", 2);
        }

        public void addSecretAgendaCard(string name, string text, int points)
        {
            var card = new SecretAgendaCard(name, text, points);
            this.allSecretAgendaCards[name] = card;
            this.availableSecretAgendaCards[name] = card;
            this.secretAgendaCards.Add(card);
        }

        public SecretAgendaCard getSecretAgendaCardByName(string name)
        {
            return this.allSecretAgendaCards.ContainsKey(name) ? this.allSecretAgendaCards[name] : null;
        }

        public SecretAgendaCard dealRandomSecretAgendaCard()
        {
            int r = new Random().Next(this.secretAgendaCards.Count);
            SecretAgendaCard card = this.secretAgendaCards[r];
            this.secretAgendaCards[r] = this.secretAgendaCards[this.secretAgendaCards.Count - 1];
            this.secretAgendaCards.RemoveAt(this.secretAgendaCards.Count - 1);
            this.availableSecretAgendaCards.Remove(card.name);
            return card;
        }

        public SecretAgendaCard chooseSecretAgendaCard(string name)
        {
            if (this.availableSecretAgendaCards.ContainsKey(name))
            {
                SecretAgendaCard card = this.availableSecretAgendaCards[name];
                this.availableSecretAgendaCards.Remove(name);
                return card;
            }
            return null;
        }

        public bool isSecretAgendaCardAvailable(string name)
        {
            return this.availableSecretAgendaCards.ContainsKey(name);
        }

        public void setupSchemeCards()
        {
            this.schemeCardIds = new Dictionary<string, SchemeCard>();
            this.discardedSchemeCards = new List<SchemeCard>();
            this.schemeDeck1 = new List<SchemeCard>();
            this.schemeDeck2 = new List<SchemeCard>();
            this.schemeCardList = new List<SchemeCard>();
            this.temporarySchemeCards = new List<SchemeCard>();
            var schemeCards = this.schemeCardList;
            // Rewards, coin cost, deaths
            this.addSchemeCard(new List<string> { "deedCard", "coin" }, 0, 1);
            this.addSchemeCard(new List<string> { "muster" }, 1, 0);
            this.addSchemeCard(new List<string> { "move", "move" }, 0, 0);
            this.addSchemeCard(new List<string> { "coin", "coin" }, 0, 0);
            this.addSchemeCard(new List<string> { "tax" }, 0, 0);
            this.addSchemeCard(new List<string> { "move", "move", "move" }, 0, 1);
            this.addSchemeCard(new List<string> { "tax" }, 0, 0);
            this.addSchemeCard(new List<string> { "attack" }, 1, 0);
            this.addSchemeCard(new List<string> { "tax", "coin" }, 0, 1);
            this.addSchemeCard(new List<string> { "move", "move", "warTrack" }, 0, 1);
            this.addSchemeCard(new List<string> { "build" }, 0, 0);
            this.addSchemeCard(new List<string> { "move", "move" }, 0, 0);
            this.addSchemeCard(new List<string> { "attack", "coin" }, 0, 1);
            this.addSchemeCard(new List<string> { "deedCard" }, 1, 0);
            this.addSchemeCard(new List<string> { "build" }, 0, 0);
            this.addSchemeCard(new List<string> { "deedCard" }, 0, 1);
            this.addSchemeCard(new List<string> { "tax", "tax" }, 0, 1);
            this.addSchemeCard(new List<string> { "build", "coin" }, 0, 1);
            this.addSchemeCard(new List<string> { "attack" }, 0, 0);
            this.addSchemeCard(new List<string> { "attack" }, 0, 0);
            this.addSchemeCard(new List<string> { "build" }, 1, 0);
            this.addSchemeCard(new List<string> { "muster" }, 0, 0);
            this.addSchemeCard(new List<string> { "muster" }, 0, 0);
            this.addSchemeCard(new List<string> { "muster", "coin" }, 0, 1);
            this.addSchemeCard(new List<string> { "buildOrAttack" }, 0, 2);
            this.addSchemeCard(new List<string> { "attack" }, 0, 0);
            this.addSchemeCard(new List<string> { "move" }, 0, 0);
            this.addSchemeCard(new List<string> { "move", "muster" }, 0, 2);
            this.addSchemeCard(new List<string> { "muster", "muster" }, 0, 1);
            this.addSchemeCard(new List<string> { "tax", "tax" }, 1, 1);
            this.addSchemeCard(new List<string> { "coin", "coin" }, 0, 0);
            this.addSchemeCard(new List<string> { "taxOrMuster", "taxOrMuster" }, 0, 2);
            
            for (int i = 0; i < 16; i++)
            {
                int r = new Random().Next(schemeCards.Count);
                SchemeCard schemeCard = schemeCards[r];
                this.schemeDeck1.Add(schemeCard);
                schemeCards[r] = schemeCards[schemeCards.Count - 1];
                schemeCards.RemoveAt(schemeCards.Count - 1);
            }
            this.schemeDeck2 = schemeCards;
            Console.WriteLine("setupSchemeCards(): " + this.schemeDeck1.Count + " " + this.schemeDeck2.Count);
        }

        public void addSchemeCard(List<string> rewards, int rewardCoinCost = 0, int deaths = 0)
        {
            var schemeCard = new SchemeCard(rewards, rewardCoinCost, deaths);
            this.schemeCardList.Add(schemeCard);
            this.schemeCardIds[schemeCard.id] = schemeCard;
        }

        public SchemeCard getSchemeCardById(string schemeCardId)
        {
            return this.schemeCardIds.ContainsKey(schemeCardId) ? this.schemeCardIds[schemeCardId] : null;
        }

        public void reshuffleSchemeCards()
        {
            var combinedSchemeCards = this.discardedSchemeCards.Concat(this.schemeDeck1).Concat(this.schemeDeck2).ToList();
            int half = combinedSchemeCards.Count / 2;
            this.discardedSchemeCards = new List<SchemeCard>();
            this.schemeDeck1 = new List<SchemeCard>();
            for (int i = 0; i < half; i++)
            {
                int r = new Random().Next(combinedSchemeCards.Count);
                SchemeCard schemeCard = combinedSchemeCards[r];
                this.schemeDeck1.Add(schemeCard);
                combinedSchemeCards[r] = combinedSchemeCards[combinedSchemeCards.Count - 1];
                combinedSchemeCards.RemoveAt(combinedSchemeCards.Count - 1);
            }
            this.schemeDeck2 = combinedSchemeCards;
        }

        // schemeDeck=1 or 2 or a list
        public SchemeCard drawSchemeCard(int schemeDeck)
        {
            Console.WriteLine("cards drawSchemeCard(): schemeDeck= " + schemeDeck);
            List<SchemeCard> schemeDeckList = null;
            if (schemeDeck == 1)
            {
                schemeDeckList = this.schemeDeck1;
            }
            else if (schemeDeck == 2)
            {
                schemeDeckList = this.schemeDeck2;
            }
            else
            {
                Console.WriteLine("drawSchemeCard(): schemeDeck=" + schemeDeck);
                throw new ArgumentException("Not a valid schemeDeckNumber " + schemeDeck, "drawAndDiscardSchemeCard()");
            }
            Console.WriteLine("drawSchemeCard(): length of schemeDeckList=" + schemeDeckList.Count);
            if (schemeDeckList.Count < 1)
            {
                this.reshuffleSchemeCards();
                if (schemeDeck == 1)
                {
                    schemeDeckList = this.schemeDeck1;
                }
                else
                {
                    schemeDeckList = this.schemeDeck2;
                }
            }
            SchemeCard card = schemeDeckList[0];
            schemeDeckList.RemoveAt(0);
            Console.WriteLine("drawSchemeCard(): length: " + this.schemeDeck1.Count + " " + this.schemeDeck2.Count);
            Console.WriteLine("drawSchemeCard(): card=" + card.id);
            return card;
        }

        // schemeDeck=1 or 2 or a list
        public SchemeCard drawAndDiscardSchemeCard(int schemeDeck)
        {
            Console.WriteLine("drawAndDiscardSchemeCard(): " + schemeDeck);
            List<SchemeCard> schemeDeckList = null;
            if (schemeDeck == 1)
            {
                schemeDeckList = this.schemeDeck1;
            }
            else if (schemeDeck == 2)
            {
                schemeDeckList = this.schemeDeck2;
            }
            else
            {
                Console.WriteLine("drawAndDiscardSchemeCard(): schemeDeck=" + schemeDeck);
                throw new ArgumentException("Not a valid schemeDeckNumber " + schemeDeck, "drawAndDiscardSchemeCard()");
            }
            SchemeCard card = null;
            if (schemeDeckList != null && schemeDeckList.Count > 0)
            {
                card = schemeDeckList[0];
                schemeDeckList.RemoveAt(0);
                this.discardedSchemeCards.Add(card);
            }
            Console.WriteLine("drawAndDiscardSchemeCard(): card=" + (card?.id ?? "null"));
            return card;
        }

        // schemeDeck=1 or 2 or a list
        public SchemeCard drawAndReturnSchemeCard(int schemeDeck, int schemeCardsToDraw)
        {
            Console.WriteLine("drawAndReturnSchemeCard(): " + schemeDeck);
            List<SchemeCard> schemeDeckList = null;
            if (schemeDeck == 1)
            {
                schemeDeckList = this.schemeDeck1;
            }
            else if (schemeDeck == 2)
            {
                schemeDeckList = this.schemeDeck2;
            }
            else
            {
                Console.WriteLine("drawAndReturnSchemeCard(): schemeDeck=" + schemeDeck);
                throw new ArgumentException("Not a valid schemeDeckNumber " + schemeDeck, "drawAndReturnSchemeCard()");
            }
            SchemeCard card = null;
            if (schemeDeckList != null && schemeDeckList.Count > 0)
            {
                card = schemeDeckList[schemeDeckList.Count - 1];
                schemeDeckList.RemoveAt(schemeDeckList.Count - 1);
                this.temporarySchemeCards.Add(card);
                if (this.temporarySchemeCards.Count >= schemeCardsToDraw)
                {
                    for (int i = 0; i < this.temporarySchemeCards.Count; i++)
                    {
                        SchemeCard tcard = this.temporarySchemeCards[0];
                        this.temporarySchemeCards.RemoveAt(0);
                        schemeDeckList.Add(tcard);
                    }
                    Console.WriteLine("drawAndReturnSchemeCard(): temporarySchemeCards=" + this.temporarySchemeCards.Count);
                }
            }
            Console.WriteLine("drawAndReturnSchemeCard(): card=" + (card?.id ?? "null"));
            return card;
        }

        public void discardSchemeCard(SchemeCard card)
        {
            this.discardedSchemeCards.Add(card);
        }

        // 1 or 2
        public List<SchemeCard> getSchemeDeckByNumber(int num)
        {
            Console.WriteLine("getSchemeDeckByNumber(): " + num);
            if (num == 1)
            {
                return this.schemeDeck1;
            }
            else
            {
                return this.schemeDeck2;
            }
        }
    }
}
