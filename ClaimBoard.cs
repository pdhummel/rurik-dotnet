using System;
using System.Collections.Generic;
using System.Linq;
namespace rurik;

public class ClaimBoard
{
    public Dictionary<string, Dictionary<string, int>> ClaimsByPlayer { get; set; }
    public Dictionary<string, Dictionary<string, int>> PreviousClaimsByPlayer { get; set; }
    public Dictionary<string, Dictionary<int, string>> WarfareRewards { get; set; }
    public Dictionary<string, Dictionary<int, List<string>>> ClaimsByTrack { get; set; }
    public Dictionary<string, Dictionary<int, int>> RequirementsByTrack { get; set; }

    public ClaimBoard()
    {
        ClaimsByPlayer = new Dictionary<string, Dictionary<string, int>>();
        PreviousClaimsByPlayer = new Dictionary<string, Dictionary<string, int>>();
        WarfareRewards = new Dictionary<string, Dictionary<int, string>>();
        ClaimsByTrack = new Dictionary<string, Dictionary<int, List<string>>>();
        RequirementsByTrack = new Dictionary<string, Dictionary<int, int>>();

        var colors = new List<string> { "blue", "red", "white", "yellow" };
        foreach (var color in colors)
        {
            ClaimsByPlayer[color] = new Dictionary<string, int>
            {
                ["rule"] = 0,
                ["build"] = 0,
                ["trade"] = 0,
                ["warfare"] = 0
            };

            PreviousClaimsByPlayer[color] = new Dictionary<string, int>
            {
                ["rule"] = 0,
                ["build"] = 0,
                ["trade"] = 0,
                ["warfare"] = 0
            };
        }

        var possibleWarfareRewards = new List<string> { "2 wood", "2 coins", "2 fish", "fur", "schemeCard", "victoryPoint" };
        ShuffleArray(possibleWarfareRewards);
        
        WarfareRewards["warfare"] = new Dictionary<int, string>();
        for (int i = 0; i <= 10; i++)
        {
            if (i > 0 && i % 2 == 0)
            {
                WarfareRewards["warfare"][i] = possibleWarfareRewards.FirstOrDefault();
                if (possibleWarfareRewards.Any())
                    possibleWarfareRewards.RemoveAt(0);
            }
            else
            {
                WarfareRewards["warfare"][i] = null;
            }
        }

        DefineRequirementsByTrack();
        ResetClaimsByTrack();
    }

    public void DefineRequirementsByTrack()
    {
        var tracks = new List<string> { "rule", "build", "trade" };
        var points = new List<int> { 8, 5, 3, 2, 1 };
        
        foreach (var track in tracks)
        {
            RequirementsByTrack[track] = new Dictionary<int, int>();
            foreach (var point in points)
            {
                RequirementsByTrack[track][point] = 0;
            }
        }
        
        RequirementsByTrack["rule"][8] = 5; // including Kiev & Novgorod
        RequirementsByTrack["rule"][5] = 5;
        RequirementsByTrack["rule"][3] = 4;
        RequirementsByTrack["rule"][2] = 3;
        RequirementsByTrack["rule"][1] = 2;
        RequirementsByTrack["build"][8] = 7;
        RequirementsByTrack["build"][5] = 5;
        RequirementsByTrack["build"][3] = 4;
        RequirementsByTrack["build"][2] = 3;
        RequirementsByTrack["build"][1] = 2;
        RequirementsByTrack["trade"][8] = 11;
        RequirementsByTrack["trade"][5] = 9;
        RequirementsByTrack["trade"][3] = 7;
        RequirementsByTrack["trade"][2] = 5;
        RequirementsByTrack["trade"][1] = 3;
    }

    public int CalculateCoins(string color)
    {
        Console.WriteLine("CalculateCoins(): " + color);
        var coins = 0;
        if (ClaimsByPlayer[color]["rule"] == 0)
            coins++;
        if (ClaimsByPlayer[color]["build"] == 0)
            coins++;
        if (ClaimsByPlayer[color]["trade"] == 0)
            coins++;
        if (ClaimsByPlayer[color]["warfare"] == 0)
            coins++;
        return coins;
    }

    // This resets the values which are used in the visualization of the claims track.
    public void ResetClaimsByTrack()
    {
        Console.WriteLine("ResetClaimsByTrack()");
        var tracks = new List<string> { "rule", "build", "trade" };
        var points = new List<int> { 8, 5, 3, 2, 1, 0 };
        
        foreach (var track in tracks)
        {
            ClaimsByTrack[track] = new Dictionary<int, List<string>>();
            foreach (var point in points)
            {
                ClaimsByTrack[track][point] = new List<string>();
            }
        }
        
        ClaimsByTrack["warfare"] = new Dictionary<int, List<string>>();
        for (int p = 0; p <= 10; p++)
        {
            ClaimsByTrack["warfare"][p] = new List<string>();
        }
    }

    // This updates the values which are used in the visualization of the claims track
    // ClaimsByPlayer -> ClaimsByTrack
    public void UpdateClaimsByTrack()
    {
        Console.WriteLine("UpdateClaimsByTrack()");
        var colors = new List<string> { "blue", "red", "white", "yellow" };
        
        foreach (var color in colors)
        {
            var rulePoints = ClaimsByPlayer[color]["rule"];
            ClaimsByTrack["rule"][rulePoints].Add(color);
            
            var buildPoints = ClaimsByPlayer[color]["build"];
            ClaimsByTrack["build"][buildPoints].Add(color);
            
            var tradePoints = ClaimsByPlayer[color]["trade"];
            ClaimsByTrack["trade"][tradePoints].Add(color);
            
            var warfarePoints = ClaimsByPlayer[color]["warfare"];
            if (warfarePoints > 10)
                warfarePoints = 10;
            ClaimsByTrack["warfare"][warfarePoints].Add(color);
        }
    }

    // This is used to save ClaimsByPlayer to PreviousClaimsByPlayer after the ClaimsByPlayer
    // have been calculated at the end of the round.
    public void UpdatePreviousClaims()
    {
        Console.WriteLine("UpdatePreviousClaims()");
        var colors = new List<string> { "blue", "red", "white", "yellow" };
        
        foreach (var color in colors)
        {
            PreviousClaimsByPlayer[color]["rule"] = ClaimsByPlayer[color]["rule"];
            PreviousClaimsByPlayer[color]["build"] = ClaimsByPlayer[color]["build"];
            PreviousClaimsByPlayer[color]["trade"] = ClaimsByPlayer[color]["trade"];
            PreviousClaimsByPlayer[color]["warfare"] = ClaimsByPlayer[color]["warfare"];
        }
    }

    /*

    // This is useful to calculate in game scoring for a player. It is an ephemeral value,
    // which could be used by AI to evaluate who is winning and whether one decision is better than another.
    public int CalculateTotalClaims(Player player, GameMap gameMap)
    {
        Console.WriteLine("CalculateTotalClaims(): " + player.Color);
        var totalClaimPoints = 0;
        var color = player.Color;
        
        var rulePoints = CalculateClaimsForRule(player, gameMap);
        if (rulePoints > PreviousClaimsByPlayer[color]["rule"])
            totalClaimPoints = totalClaimPoints + rulePoints;
        else
            totalClaimPoints = totalClaimPoints + PreviousClaimsByPlayer[color]["rule"];

        var tradePoints = CalculateClaimsForTrade(player);
        if (tradePoints > PreviousClaimsByPlayer[color]["trade"])
            totalClaimPoints = totalClaimPoints + tradePoints;
        else
            totalClaimPoints = totalClaimPoints + PreviousClaimsByPlayer[color]["trade"];

        var buildPoints = CalculateClaimsForBuild(player, gameMap);
        if (buildPoints > PreviousClaimsByPlayer[color]["build"])
            totalClaimPoints = totalClaimPoints + buildPoints;
        else
            totalClaimPoints = totalClaimPoints + PreviousClaimsByPlayer[color]["build"];
            
        totalClaimPoints = totalClaimPoints + ClaimsByPlayer[color]["warfare"];
        Console.WriteLine("CalculateTotalClaims(): totalClaimPoints=" + totalClaimPoints);
        return totalClaimPoints;
    }

    public void UpdateClaimsForClaimsPhase(Game game, List<Player> players, GameMap gameMap)
    {
        foreach (var player in players)
        {
            // Coin compensation logic is done by players.endRoundForPlayers() - see CalculateCoins().
            var coinCompensation = UpdateClaims(player, gameMap);
        }
        ResetClaimsByTrack();
        UpdateClaimsByTrack();
    }

    // This updates the ClaimsByPlayer using the player's position on the map.
    // The ClaimsByPlayer never decrease so it is never worst than the PreviousClaimsByPlayer.
    public int UpdateClaims(Player player, GameMap gameMap)
    {
        Console.WriteLine("UpdateClaims(): " + player.Color);
        var coinCompensation = 0;
        var color = player.Color;
        
        var rulePoints = CalculateClaimsForRule(player, gameMap);
        if (rulePoints > PreviousClaimsByPlayer[color]["rule"])
            ClaimsByPlayer[color]["rule"] = rulePoints;
        if (ClaimsByPlayer[color]["rule"] <= 0)
            coinCompensation++;

        var tradePoints = CalculateClaimsForTrade(player);
        if (tradePoints > PreviousClaimsByPlayer[color]["trade"])
            ClaimsByPlayer[color]["trade"] = tradePoints;
        if (ClaimsByPlayer[color]["trade"] <= 0)
            coinCompensation++;

        var buildPoints = CalculateClaimsForBuild(player, gameMap);
        if (buildPoints > PreviousClaimsByPlayer[color]["build"])
            ClaimsByPlayer[color]["build"] = buildPoints;
        if (ClaimsByPlayer[color]["build"] <= 0)
            coinCompensation++;
            
        return coinCompensation;
    }

    public int CalculateClaimsForRule(Player player, GameMap gameMap)
    {
        Console.WriteLine("CalculateClaimsForRule(): " + player.Color);
        var playerRulePoints = 0;
        var color = player.Color;
        var rules = 0;
        var rulesKiev = false;
        var rulesNovgorod = false;
        
        foreach (var location in gameMap.LocationsForGame)
        {
            var isSviatopolk = false;
            if (player.Leader.Name == "Sviatopolk" && location.LeaderByColor[color] > 0)
                isSviatopolk = true;
                
            var isYaroslav = false;
            if (player.Leader.Name == "Yaroslav" && location.LeaderByColor[color] > 0)
                isYaroslav = true;
                
            var rulesLocation = location.DoesRule(color, isSviatopolk, isYaroslav);
            if (rulesLocation)
            {
                if (location.Name == "Kiev")
                    rulesKiev = true;
                if (location.Name == "Novgorod")
                    rulesNovgorod = true;
                rules++;
            }
        }
        
        var points = new List<int> { 8, 5, 3, 2, 1 };
        foreach (var rulePoints in points)
        {
            if (rules >= RequirementsByTrack["rule"][rulePoints] && rulePoints == 8)
            {
                if (rulesKiev && rulesNovgorod)
                {
                    playerRulePoints = rulePoints;
                    break;
                }
            }
            else if (rules >= RequirementsByTrack["rule"][rulePoints])
            {
                playerRulePoints = rulePoints;
                break;
            }
        }
        
        Console.WriteLine("CalculateClaimsForRule(): playerRulePoints=" + playerRulePoints);
        return playerRulePoints;
    }

    public int CalculateClaimsForTrade(Player player)
    {
        Console.WriteLine("CalculateClaimsForTrade(): " + player.Color);
        var playerTradePoints = 0;
        var goodsOnBoat = player.Boat.GoodsOnBoat["stone"] + player.Boat.GoodsOnBoat["wood"] +
            player.Boat.GoodsOnBoat["fish"] + player.Boat.GoodsOnBoat["honey"] + 
            player.Boat.GoodsOnBoat["fur"];

        var points = new List<int> { 8, 5, 3, 2, 1 };
        foreach (var tradePoints in points)
        {
            if (goodsOnBoat >= RequirementsByTrack["trade"][tradePoints])
            {
                playerTradePoints = tradePoints;
                break;
            }
        }
        
        Console.WriteLine("CalculateClaimsForTrade(): playerTradePoints=" + playerTradePoints);
        return playerTradePoints;
    }

    public int CalculateClaimsForBuild(Player player, GameMap gameMap)
    {
        Console.WriteLine("CalculateClaimsForBuild(): " + player.Color);
        var playerBuildPoints = 0;
        var color = player.Color;

        var locationsWithBuildings = new List<Location>();
        foreach (var location in gameMap.LocationsForGame)
        {
            if (location.DoesPlayerHaveBuilding(color))
                locationsWithBuildings.Add(location);
        }
        
        var adjacentBuildRegions = CountConnectedLocations(gameMap, color, locationsWithBuildings);

        var points = new List<int> { 8, 5, 3, 2, 1 };
        foreach (var buildPoints in points)
        {
            if (adjacentBuildRegions >= RequirementsByTrack["build"][buildPoints])
            {
                playerBuildPoints = buildPoints;
                break;
            }
        }
        
        Console.WriteLine("CalculateClaimsForBuild(): playerBuildPoints=" + playerBuildPoints);
        return playerBuildPoints;
    }

    public List<string> DFSUtil(GameMap gameMap, string color, List<string> cluster, string locationName, Dictionary<string, bool> visited)
    {
        // Mark the current node as visited and print it
        visited[locationName] = true;
        var location = gameMap.GetLocation(locationName);
        
        if (location.DoesPlayerHaveBuilding(color))
            cluster.Add(locationName);
            
        // Recur for all the vertices
        // adjacent to this vertex
        if (location.DoesPlayerHaveBuilding(color))
        {
            foreach (var neighbor in location.Neighbors)
            {
                if (!visited[neighbor])
                {
                    DFSUtil(gameMap, color, cluster, neighbor, visited);
                }   
            }
        }
        return cluster;
    }
     
    public int CountConnectedLocations(GameMap gameMap, string color, List<Location> locationsWithBuildings)
    {
        Console.WriteLine("CountConnectedLocations(): " + string.Join(",", locationsWithBuildings.Select(l => l.Name)));
        var largestClusterCount = 0;
        
        // Mark all the vertices as not visited
        var V = locationsWithBuildings.Count;
        var visited = new Dictionary<string, bool>();
        foreach (var location in locationsWithBuildings)
        {
            visited[location.Name] = false;
        }
        
        for (int v = 0; v < V; ++v)
        {
            var location = locationsWithBuildings[v];
            if (!visited[location.Name] && location.DoesPlayerHaveBuilding(color))
            {
                // Count all reachable vertices from v
                var cluster = DFSUtil(gameMap, color, new List<string>(), location.Name, visited);
                if (cluster.Count > largestClusterCount)
                    largestClusterCount = cluster.Count;
            }
        }
        
        Console.WriteLine("CountConnectedLocations(): largestClusterCount=" + largestClusterCount);
        return largestClusterCount;
    }
*/     
    // Randomize array in-place using Durstenfeld shuffle algorithm 
    public void ShuffleArray(List<string> array)
    {
        Random random = new Random();
        for (int i = array.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            string temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }    

    
}
