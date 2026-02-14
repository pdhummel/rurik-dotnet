using System;
using System.Collections.Generic;

namespace rurik
{
    public class SchemeCard
    {
        public List<string> rewards { get; set; }
        public int rewardCoinCost { get; set; }
        public int deaths { get; set; }
        public string id { get; set; }

        public SchemeCard(List<string> rewards, int rewardCoinCost = 0, int deaths = 0)
        {
            this.rewards = rewards ?? new List<string>();
            this.rewardCoinCost = rewardCoinCost;
            this.deaths = deaths;
            
            // Generate ID similar to JavaScript version
            this.id = "";
            for (int i = 0; i < rewards.Count; i++)
            {
                if (i > 0)
                {
                    this.id = this.id + "-" + rewards[i];
                }
                else
                {
                    this.id = rewards[i];
                }
            }
            this.id = this.id + "-" + rewardCoinCost + "-" + deaths;
        }

        public bool isEqual(SchemeCard schemeCard)
        {
            if (this.rewardCoinCost == schemeCard.rewardCoinCost && 
                this.deaths == schemeCard.deaths && 
                this.rewards.Count == schemeCard.rewards.Count)
            {
                for (int i = 0; i < this.rewards.Count; i++)
                {
                    if (this.rewards[i] != schemeCard.rewards[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
