using System;
using System.Collections.Generic;

namespace rurik
{
    public class AvailableLeaders
    {
        private Dictionary<string, Leader> allLeaders;
        private Dictionary<string, Leader> availableLeaders;

        public AvailableLeaders()
        {
            allLeaders = new Dictionary<string, Leader>();
            availableLeaders = new Dictionary<string, Leader>();
            AddLeader("Boris", "When you attack an opponent in Boris's region, reveal one less scheme card and steal one coin from that opponent.");
            AddLeader("Sviatopolk", "Rebels in Sviatopolk's region count as your troops for purposes of rule. If you defeat a rebel in that region, replace it with one of your troops.");
            AddLeader("Yaroslav", "In Yaroslav's region, you win ties for rule and your opponents lose the abilities of their structures.");
            AddLeader("Mstislav", "In Mstislav's region, it only costs you one tax point to tax or one build point to build, regardless of who rules that region.");
            AddLeader("Gleb", "Once per round, when you attack in Gleb's region with no casualty, gain two movement points to use with any of your troops located in his region (including Gleb).");
            AddLeader("Theofana", "Once per round, when you tax in Theofana's region, you may move her to an adjacent region or gain one coin from the supply. If you use this ability, you cannot tax again this turn.");
            AddLeader("Maria", "Once per round when you muster troops, you may place them in a single region adjacent to Maria's region that you do not occupy.");
            AddLeader("Predslava", "Once per round, on your turn, you may move an opponent's troop to an adjacent region for free. The opponent gains one coin from the supply.");
            AddLeader("Sviatoslav", "Before you spend any build points on your turn, you may move Sviatoslav to an adjacent region with a rebel or spend one coin to move Sviatoslav to any adjacent region.");
            AddLeader("Agatha", "When you move Agatha, you may move up to two of your troops along with her for free.");
            AddLeader("Sudislav", "You may spend attack points as muster points instead, to muster troops in Sudislav's region.");
        }

        private void AddLeader(string name, string description = null)
        {
            Leader leader = new Leader(name, description);
            availableLeaders[name] = leader;
            allLeaders[name] = leader;
        }

        public Leader GetLeaderByName(string name)
        {
            return allLeaders.ContainsKey(name) ? allLeaders[name] : null;
        }

        public Leader ChooseLeader(string name)
        {
            if (availableLeaders.ContainsKey(name))
            {
                Leader leader = availableLeaders[name];
                availableLeaders.Remove(name);
                return leader;
            }
            return null;
        }

        public bool IsLeaderAvailable(string name)
        {
            return availableLeaders.ContainsKey(name);
        }
    }
}
