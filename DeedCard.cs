using System;
using System.Collections.Generic;

namespace rurik
{
    public class DeedCard
    {
        public string name { get; set; }
        public int victoryPoints { get; set; }
        public string requirementText { get; set; }
        public List<string> rewards { get; set; }
        public List<string> costs { get; set; } // Can be List<string> or Dictionary<string, object>
        public List<string> achievements { get; set; }
        public bool canSolo { get; set; }
        public bool canAi { get; set; }
        public bool accomplished { get; set; }
        public Dictionary<string, object> verifiedByPlayers { get; set; }
        public string playerColor { get; set; }
        public List<string> claimStatements { get; set; }
        public List<string> summarizedClaimStatements { get; set; }

        public DeedCard(string name, int victoryPoints, string requirementText, List<string> rewards = null, List<string> costs = null, List<string> achievements = null, bool canSolo = true, bool canAi = false)
        {
            this.name = name;
            this.victoryPoints = victoryPoints;
            this.requirementText = requirementText;
            this.rewards = rewards ?? new List<string>();
            this.costs = costs;
            this.achievements = achievements ?? new List<string>();
            this.canSolo = canSolo;
            this.canAi = canAi;
            this.accomplished = false;
            this.verifiedByPlayers = new Dictionary<string, object>
            {
                {"red", null},
                {"blue", null},
                {"yellow", null},
                {"white", null}
            };
            this.playerColor = null;
            this.claimStatements = new List<string>();
            this.summarizedClaimStatements = new List<string>();
        }
    }
}
