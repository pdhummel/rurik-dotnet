using System;
using System.Collections.Generic;

namespace rurik
{
    public class BoatMat
    {
        public int capturedRebels { get; set; }
        public int money { get; set; }

        public Dictionary<string, int> goodsOnDock { get; set; }
        public Dictionary<string, int> goodsOnBoatSlots { get; set; }
        public Dictionary<string, int> goodsOnBoat { get; set; }
        public Dictionary<string, int> tradeBoon { get; set; }

        public bool canPlayMusterConversionTile { get; set; }
        public bool canPlayAttackConversionTile { get; set; }
        public bool canPlayBuildConversionTile { get; set; }

        public BoatMat()
        {
            capturedRebels = 0;
            money = 3;

            goodsOnDock = new Dictionary<string, int>();
            goodsOnDock["stone"] = 0;
            goodsOnDock["wood"] = 0;
            goodsOnDock["fish"] = 0;
            goodsOnDock["honey"] = 0;
            goodsOnDock["fur"] = 0;
            goodsOnDock["tradeBoon"] = 0;

            goodsOnBoatSlots = new Dictionary<string, int>();
            goodsOnBoatSlots["stone"] = 2;
            goodsOnBoatSlots["wood"] = 3;
            goodsOnBoatSlots["fish"] = 3;
            goodsOnBoatSlots["honey"] = 2;
            goodsOnBoatSlots["fur"] = 1;

            goodsOnBoat = new Dictionary<string, int>();
            goodsOnBoat["stone"] = 0;
            goodsOnBoat["wood"] = 0;
            goodsOnBoat["fish"] = 0;
            goodsOnBoat["honey"] = 0;
            goodsOnBoat["fur"] = 0;

            tradeBoon = new Dictionary<string, int>();
            tradeBoon["wood"] = 1;
            tradeBoon["fish"] = 1;        
            tradeBoon["honey"] = 1;
            tradeBoon["stone"] = 0;
            tradeBoon["fur"] = 0;

            canPlayMusterConversionTile = true;
            canPlayAttackConversionTile = true;
            canPlayBuildConversionTile = true;
        }

        public void AddGoodToBoatOrDock(string resource)
        {
            if (DoesBoatHaveRoom(resource))
            {
                AddGoodToBoat(resource);
            }
            else
            {
                AddGoodToDock(resource);
            }
        }

        public void AddGoodToDock(string resource)
        {
            Console.WriteLine("AddGoodToDock(): " + resource);
            goodsOnDock[resource]++;
        }

        public void AddGoodToBoat(string resource)
        {
            Console.WriteLine("AddGoodToBoat(): " + resource);
            int openSlots = goodsOnBoatSlots[resource] - goodsOnBoat[resource];
            if (openSlots > 0)
            {
                goodsOnBoat[resource] = goodsOnBoat[resource] + 1;
                if (openSlots == 1 && tradeBoon[resource] > 0)
                {
                    Console.WriteLine("AddGoodToBoat(): tradeBoon bonus for " + resource);
                    goodsOnDock["tradeBoon"]++;
                    tradeBoon[resource] = 0;
                }
            }
        }

        public bool DoesBoatHaveRoom(string resource)
        {
            int openSlots = goodsOnBoatSlots[resource] - goodsOnBoat[resource];
            return openSlots > 0;
        }
        
        public int CalculateCoinIncome()
        {
            int coinIncome = 0;
            if (goodsOnBoatSlots["stone"] == goodsOnBoat["stone"])
            {
                coinIncome++;
            }
            if (goodsOnBoatSlots["wood"] == goodsOnBoat["wood"])
            {
                coinIncome++;
            }
            if (goodsOnBoatSlots["fish"] == goodsOnBoat["fish"])
            {
                coinIncome++;
            }
            if (goodsOnBoatSlots["honey"] == goodsOnBoat["honey"])
            {
                coinIncome++;
            }
            if (goodsOnBoatSlots["fur"] == goodsOnBoat["fur"])
            {
                coinIncome++;
            }
            return coinIncome;
        }

        public void MoveResourceFromBoatToDock(string resource)
        {
            if (goodsOnBoat[resource] > 0)
            {
                goodsOnBoat[resource]--;
                goodsOnDock[resource]++;
            }
        }

        public void MoveResourceFromDockToBoat(string resource)
        {
            int openSlots = goodsOnBoatSlots[resource] - goodsOnBoat[resource];
            if (goodsOnDock[resource] > 0 && openSlots > 0)
            {
                goodsOnBoat[resource]++;
                goodsOnDock[resource]--;
            }
        }

        public int UseResourceConversionTile(string resource1, string resource2, string resourceToMatch1, string resourceToMatch2)
        {
            Console.WriteLine("UseResourceConversionTile(): " + resource1 + " " + resource2 + " " + resourceToMatch1 + " " + resourceToMatch2);
            int actions = 0;
            bool canConvert = false;
            if (resource1 == resourceToMatch1 || resource1 == resourceToMatch2 || resource1 == "tradeboon" || 
                resource2 == resourceToMatch1 || resource2 == resourceToMatch2 || resource2 == "tradeboon")
            {
                if (resource1 == resource2 && goodsOnDock[resource1] > 1)
                {
                    canConvert = true;
                }
                else if (goodsOnDock[resource1] > 0 && goodsOnDock[resource2] > 0 )
                {
                    canConvert = true;
                }
            }
            if (!canConvert)
            {
                Console.WriteLine("UseResourceConversionTile(): " + goodsOnDock[resource1] + " " + goodsOnDock[resource2]);
                throw new Exception("UseResourceConversionTile(): Resources not available to complete conversion.");
            }
            if (canConvert)
            {
                goodsOnDock[resource1]--;
                goodsOnDock[resource2]--;
                actions++;
            }
            return actions;
        }

        public int UseMusterConversionTile(string resource1, string resource2)
        {
            if (!canPlayMusterConversionTile)
            {
                throw new Exception("UseMusterConversionTile(): Cannot use muster conversion tile at this time.");
            }
            int actions = UseResourceConversionTile(resource1, resource2, "fish", "honey");
            canPlayMusterConversionTile = false;
            return actions;
        }

        public int UseBuildConversionTile(string resource1, string resource2)
        {
            if (!canPlayBuildConversionTile)
            {
                throw new Exception("UseBuildConversionTile(): Cannot use build conversion tile at this time.");
            }
            int actions = UseResourceConversionTile(resource1, resource2, "wood", "stone");
            canPlayBuildConversionTile = false;
            return actions;
        }

        public int UseAttackConversionTile()
        {
            if (!canPlayAttackConversionTile)
            {
                throw new Exception("UseAttackConversionTile(): Cannot use attack conversion tile at this time.");
            }
            int actions = 0;
            if (capturedRebels > 0 && money > 1)
            {
                capturedRebels--;
                money = money - 2;
                actions++;
                canPlayAttackConversionTile = false;
            }
            else
            {
                throw new Exception("UseAttackConversionTile(): Either rebels or money are not available to complete the conversion");
            }
            return actions;
        }
    }
}
