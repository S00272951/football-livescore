using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public class MatchEvent
    {
        public int Minute { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{Minute}' - {Description}";
        }
    }
}