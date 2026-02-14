using System;
using System.Collections.Generic;

namespace rurik
{
    public class SecretAgendaCard
    {
        public string name { get; set; }
        public string text { get; set; }
        public int points { get; set; }
        public bool accomplished { get; set; }

        public SecretAgendaCard(string name, string text, int points)
        {
            this.name = name;
            this.text = text;
            this.points = points;
            this.accomplished = false;
        }
    }
}
