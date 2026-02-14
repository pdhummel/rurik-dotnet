using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rurik
{
    public class AiStrategy
    {
        public string name { get; set; }
        public List<string> buildingOrder { get; set; }
        public List<AiAuction> auctions { get; set; }

        public AiStrategy(string name, List<string> buildingOrder, List<AiAuction> auctions)
        {
            this.name = name;
            this.buildingOrder = buildingOrder;
            this.auctions = auctions;
        }
    }
}
