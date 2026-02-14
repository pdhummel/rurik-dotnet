using System;
using System.Collections.Generic;
using System.Linq;

namespace rurik
{
    public class GameMap
    {
        public Dictionary<string, Location> LocationByName { get; set; }
        public List<Location> Locations { get; set; }
        public List<Location> LocationsForGame { get; set; }
        public Rebels Rebels { get; set; }

        public GameMap()
        {
            LocationByName = new Dictionary<string, Location>();
            Locations = new List<Location>();
            LocationsForGame = new List<Location>();
            Rebels = new Rebels();
            AddLocation(1, "Novgorod", "green", "wood", new List<string> { "Pskov", "Polotsk", "Smolensk", "Rostov" });
            AddLocation(2, "Rostov", "green", "stone", new List<string> { "Novgorod", "Smolensk", "Chernigov", "Suzdal" });
            AddLocation(3, "Polotsk", "green", "stone", new List<string> { "Novgorod", "Pskov", "Brest", "Volyn", "Kiev", "Chernigov", "Smolensk" });
            AddLocation(4, "Smolensk", "green", "honey", new List<string> { "Novgorod", "Rostov", "Polotsk", "Chernigov" });
            AddLocation(5, "Volyn", "green", "fish", new List<string> { "Brest", "Polotsk", "Galich", "Kiev" });
            AddLocation(6, "Kiev", "green", "wood", new List<string> { "Volyn", "Polotsk", "Chernigov", "Galich", "Azov", "Pereyaslavl" });
            AddLocation(7, "Chernigov", "green", "fish", new List<string> { "Rostov", "Polotsk", "Smolensk", "Kiev", "Suzdal", "Murom", "Pereyaslavl" });
            AddLocation(8, "Pereyaslavl", "green", "fur", new List<string> { "Chernigov", "Kiev", "Murom", "Azov" });
            AddLocation(9, "Pskov", "yellow", "fish", new List<string> { "Novgorod", "Polotsk", "Brest" });
            AddLocation(10, "Suzdal", "yellow", "wood", new List<string> { "Rostov", "Chernigov", "Murom" });
            AddLocation(11, "Galich", "yellow", "honey", new List<string> { "Volyn", "Kiev", "Peresech", "Azov", "Brest" });
            AddLocation(12, "Brest", "brown", "wood", new List<string> { "Polotsk", "Volyn", "Pskov", "Galich", "Peresech" });
            AddLocation(13, "Murom", "brown", "stone", new List<string> { "Chernigov", "Pereyaslavl", "Suzdal", "Azov" });
            AddLocation(14, "Peresech", "brown", "fur", new List<string> { "Galich", "Brest", "Azov" });
            AddLocation(15, "Azov", "brown", "fish", new List<string> { "Kiev", "Pereyaslavl", "Galich", "Murom", "Peresech" });
        }

        public void AddLocation(int id, string name, string color, string resource, List<string> neighbors)
        {
            var location = new Location(name, color, resource, neighbors);
            Locations.Add(location);
            LocationByName[name] = location;
            location.rebelRewards.Add(Rebels.PlaceRandomRebel());
        }

        public Location GetLocation(string name)
        {
            return LocationByName[name];
        }

        public void SetLocationsForGame(int numberOfPlayers)
        {
            List<Location> locations;
            // green
            if (numberOfPlayers <= 2)
            {
                locations = Locations.Take(8).ToList();
            }
            // yellow
            else if (numberOfPlayers == 3)
            {
                locations = Locations.Take(11).ToList();
            }
            // brown
            else
            {
                locations = Locations.Take(15).ToList();
            }
            LocationsForGame = locations;
        }

        public List<Location> GetLocations(int numberOfPlayers)
        {
            if (LocationsForGame.Count <= 0)
            {
                SetLocationsForGame(numberOfPlayers);
            }
            return LocationsForGame;
        }

        public List<string> GetLocationsForGameNames()
        {
            return LocationsForGame.Select(l => l.name).ToList();
        }

        public Dictionary<string, object> GetLocationsForPlayer(string color, bool isSviatopolk = false, bool isYaroslav = false)
        {
            var locationMap = new Dictionary<string, object>
            {
                {"rules", new List<string>()},
                {"occupies", new List<string>()},
                {"hasBuildings", new List<string>()},
                {"neighbors", new List<string>()},
                {"enemies", new List<string>()}
            };

            var neighbors = new List<string>();
            var locationsForGame = new HashSet<string>(LocationsForGame.Select(l => l.name));
            
            foreach (var location in Locations)
            {
                locationMap[location.name] = new Dictionary<string, object>
                {
                    {"rules", location.DoesRule(color, isSviatopolk, isYaroslav)},
                    {"occupies", location.DoesOccupy(color)},
                    {"hasStronghold", location.CountStrongholds(color) > 0},
                    {"hasMarket", location.DoesPlayerHaveMarket(color)},
                    {"hasBuildings", location.DoesPlayerHaveBuilding(color)}
                };

                if (location.DoesRule(color, isSviatopolk, isYaroslav))
                {
                    ((List<string>)locationMap["rules"]).Add(location.name);
                }

                if (location.DoesOccupy(color))
                {
                    ((List<string>)locationMap["occupies"]).Add(location.name);
                    foreach (var locationName in location.neighbors)
                    {
                        if (locationsForGame.Contains(locationName))
                        {
                            neighbors.Add(locationName);
                        }
                    }
                }

                if (location.DoesPlayerHaveBuilding(color))
                {
                    ((List<string>)locationMap["hasBuildings"]).Add(location.name);
                }
            }

            locationMap["neighbors"] = new HashSet<string>(neighbors).ToList();
            return locationMap;
        }

        public int CountTroopsInLocations(string color)
        {
            return LocationsForGame.Sum(location => location.troopsByColor[color]);
        }

        public void ResetResources()
        {
            foreach (var location in LocationsForGame)
            {
                location.resourceCount = 1;
            }
        }
    }
}
