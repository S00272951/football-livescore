using System;

namespace Project1
{
    public class Team
    {
        public string Name { get; set; } = "";

        public string Logo { get; set; } = "";

        public override string ToString()
        {
            return Name;
        }
    }
}