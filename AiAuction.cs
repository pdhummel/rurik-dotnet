using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rurik
{
    public class AiAuction
    {
        public int advisor { get; set; }
        public string action { get; set; }
        public int coins { get; set; }

        public AiAuction(int advisor, string action, int coins)
        {
            this.advisor = advisor;
            this.action = action;
            this.coins = coins;
        }
    }
}
