using System;
using System.Collections.Generic;
using System.Linq;

namespace rurik
{
    public class Rebels
    {
        private List<string> rewards;

        public Rebels()
        {
            rewards = new List<string>
            {
                "honey",
                "honey",
                "fish",
                "stone",
                "fur",
                "2 coins",
                "wood",
                "coin",
                "2 coins",
                "coin",
                "wood",
                "stone",
                "coin",
                "coin",
                "fish",
                "wood",
                "2 coins",
                "stone",
                "fish"
            };
        }

        public string PlaceRandomRebel()
        {
            if (rewards.Count == 0)
                return null;

            var i = new Random().Next(rewards.Count);
            var reward = rewards[i];
            rewards[i] = rewards[rewards.Count - 1];
            rewards.RemoveAt(rewards.Count - 1);
            return reward;
        }
    }
}
