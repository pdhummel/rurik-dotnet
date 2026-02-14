using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rurik
{
    public class AiStrategyCard
    {
        public int id { get; set; }
        public Dictionary<string, AiStrategy> strategies { get; set; }
        public List<string> locationOrder { get; set; }

        public AiStrategyCard(int id, Dictionary<string, AiStrategy> strategies, List<string> locationOrder)
        {
            this.id = id;
            this.strategies = strategies;
            this.locationOrder = locationOrder;
        }
    }
}
