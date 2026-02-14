using System;
using System.Collections.Generic;
using System.Linq;
namespace rurik;

public class Location
{
    public string name { get; set; }
    public string color { get; set; }
    public string defaultResource { get; set; }
    public int resourceCount { get; set; }
    public List<string> neighbors { get; set; }
    public Dictionary<string, int> troopsByColor { get; set; }
    public Dictionary<string, int> leaderByColor { get; set; }
    public List<string> rebelRewards { get; set; }
    public List<Building> buildings { get; set; }

    public Location(string name, string color, string defaultResource, List<string> neighbors)
    {
        this.name = name;
        this.color = color;
        this.defaultResource = defaultResource;
        this.resourceCount = 1;
        this.neighbors = neighbors;
        this.troopsByColor = new Dictionary<string, int>
        {
            {"red", 0},
            {"blue", 0},
            {"white", 0},
            {"yellow", 0}
        };
        this.leaderByColor = new Dictionary<string, int>
        {
            {"red", 0},
            {"blue", 0},
            {"white", 0},
            {"yellow", 0}
        };
        this.rebelRewards = new List<string>();
        this.buildings = new List<Building>();
    }

    public void AddTroop(string color, int count = 1)
    {
        if (count + troopsByColor[color] >= 0)
        {
            troopsByColor[color] = troopsByColor[color] + count;
        }
    }

    public void AddLeader(string color, int count = 1)
    {
        if (count > 0)
        {
            leaderByColor[color] = 1;
        }
        else
        {
            leaderByColor[color] = 0;
        }
    }

    public void AddBuilding(string color, string buildingName)
    {
        if (buildings.Count < 3)
        {
            var hasMatch = HasBuilding(buildingName);
            if (!hasMatch)
            {
                var building = new Building(color, buildingName);
                buildings.Add(building);
            }
        }
    }

    public bool HasBuilding(string buildingName)
    {
        return buildings.Any(b => b.name == buildingName);
    }

    public int CountStrongholds(string color)
    {
        return buildings.Count(b => b.name == "stronghold" && b.color == color);
    }

    public bool DoesPlayerHaveMarket(string color)
    {
        return buildings.Any(b => b.name == "market" && b.color == color);
    }

    public bool DoesPlayerHaveBuilding(string color)
    {
        return buildings.Any(b => b.color == color);
    }

    public bool DoesPlayerHaveThisBuilding(string color, string buildingName)
    {
        return buildings.Any(b => b.color == color && b.name == buildingName);
    }

    public bool DoesOccupy(string color)
    {
        if (troopsByColor[color] > 0 || leaderByColor[color] > 0)
        {
            return true;
        }
        return false;
    }

    public bool DoesRule(string color, bool isSviatopolk = false, bool isYaroslav = false)
    {
        string ruler = null;
        if (isSviatopolk && IsLeaderInLocation(color))
        {
            ruler = WhoRules(color, null);
        }
        else if (isYaroslav && IsLeaderInLocation(color))
        {
            ruler = WhoRules(null, color);
        }
        else
        {
            ruler = WhoRules();
        }
        if (ruler == color)
        {
            return true;
        }
        return false;
    }

    public bool IsLeaderInLocation(string color)
    {
        if (leaderByColor[color] > 0)
        {
            return true;
        }
        return false;
    }

    public bool HasEnemy(string color)
    {
        if (rebelRewards.Count > 0)
        {
            return true;
        }
        var colors = new List<string> { "white", "red", "blue", "yellow" };
        foreach (var c in colors)
        {
            if (c == color)
            {
                continue;
            }
            if (troopsByColor[c] > 0 || leaderByColor[c] > 0)
            {
                return true;
            }
        }
        return false;
    }

    public bool HasPlayerEnemy(string color)
    {
        var colors = new List<string> { "white", "red", "blue", "yellow" };
        foreach (var c in colors)
        {
            if (c == color)
            {
                continue;
            }
            if (troopsByColor[c] > 0 || leaderByColor[c] > 0)
            {
                return true;
            }
        }
        return false;
    }

    public string WhoRules(string sviatopolk = null, string yaroslav = null)
    {
        var yellow = troopsByColor["yellow"] + leaderByColor["yellow"];
        var red = troopsByColor["red"] + leaderByColor["red"];
        var white = troopsByColor["white"] + leaderByColor["white"];
        var blue = troopsByColor["blue"] + leaderByColor["blue"];
        if (yaroslav == null || yaroslav == "yellow")
        {
            yellow = yellow + CountStrongholds("yellow");
        }
        if (yaroslav == null || yaroslav == "red")
        {
            red = red + CountStrongholds("red");
        }
        if (yaroslav == null || yaroslav == "white")
        {
            white = white + CountStrongholds("white");
        }
        if (yaroslav == null || yaroslav == "blue")
        {
            blue = blue + CountStrongholds("blue");
        }
        var rebels = this.rebelRewards.Count;
        if (sviatopolk == "yellow")
        {
            yellow = yellow + rebels;
        }
        else if (sviatopolk == "red")
        {
            red = red + rebels;
        }
        else if (sviatopolk == "white")
        {
            white = white + rebels;
        }
        else if (sviatopolk == "blue")
        {
            blue = blue + rebels;
        }
        var highValue = rebels;
        string ruler = null;
        if (red > highValue)
        {
            ruler = "red";
            highValue = red;
        }
        else if (red == highValue && yaroslav == "red")
        {
            ruler = "red";
            highValue = red;
        }
        else if (red == highValue)
        {
            ruler = null;
        }
        if (blue > highValue)
        {
            ruler = "blue";
            highValue = blue;
        }
        else if (blue == highValue && yaroslav == "blue")
        {
            ruler = "blue";
            highValue = blue;
        }
        else if (blue == highValue)
        {
            ruler = null;
        }
        if (white > highValue)
        {
            ruler = "white";
            highValue = white;
        }
        else if (white == highValue && yaroslav == "white")
        {
            ruler = "white";
            highValue = white;
        }
        else if (white == highValue)
        {
            ruler = null;
        }
        if (yellow > highValue)
        {
            ruler = "yellow";
            highValue = yellow;
        }
        else if (yellow == highValue && yaroslav == "yellow")
        {
            ruler = "yellow";
            highValue = yellow;
        }
        else if (yellow == highValue)
        {
            ruler = null;
        }
        //if (ruler == null) {
        //    Console.WriteLine(this.name + " is ruled by no one");
        //}
        return ruler;
    }

    public int CalculateExcessTroopsForRule(string rulingColor)
    {
        var strength = new Dictionary<string, int>
        {
            {"yellow", CountStrongholds("yellow") + troopsByColor["yellow"] + leaderByColor["yellow"]},
            {"red", CountStrongholds("red") + troopsByColor["red"] + leaderByColor["red"]},
            {"white", CountStrongholds("white") + troopsByColor["white"] + leaderByColor["white"]},
            {"blue", CountStrongholds("blue") + troopsByColor["blue"] + leaderByColor["blue"]},
            {"rebels", rebelRewards.Count}
        };
        var highValue = 0;
        var nextHighValue = 0;
        var enemies = new List<string> { "yellow", "red", "white", "blue", "rebels" };
        foreach (var enemy in enemies)
        {
            if (enemy == rulingColor)
            {
                highValue = strength[rulingColor];
                continue;
            }
            if (strength[enemy] > nextHighValue)
            {
                nextHighValue = strength[enemy];
            }
        }
        var excess = highValue - nextHighValue;
        return excess;
    }

    public bool IsNeighbor(string locationName)
    {
        //Console.WriteLine("isNeighbor(): self=" + this.name + ", neighbor? " + locationName);
        return neighbors.Contains(locationName);
    }

    public bool IsEnemyYaroslavInLocation(string color, List<Player> players)
    {
        foreach (var player in players)
        {
            var leaderName = player.leader.name;
            var otherColor = player.Color;
            if (leaderName == "Yaroslav" && otherColor != color && leaderByColor[otherColor] > 0)
            {
                return true;
            }
        }
        return false;
    }
}



