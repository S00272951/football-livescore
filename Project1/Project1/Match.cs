using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Project1
{
    public enum MatchStatus 
    { 
        Live, 
        Fixture, 
        Finished 
    }

    public class Match : IComparable<Match>
    {
        public string League { get; set; } = string.Empty;

        public Team Home { get; set; } = new Team();
        public Team Away { get; set; } = new Team();

        public DateTime Kickoff { get; set; }

        public MatchStatus Status { get; set; }

        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }
        public string EventId { get; set; }

        // Only used when match is Live
        public int Minute { get; set; }

        public ObservableCollection<MatchEvent> Events { get; set; } = new ObservableCollection<MatchEvent>();

        // Display properties (XAML)


        public string DisplayTeams
        {
            get { return $"{Home?.Name} vs {Away?.Name}"; }
        }

        public string DisplayScore
        {
            get
            {
                if (Status == MatchStatus.Fixture)
                    return "-";

                return $"{HomeGoals} - {AwayGoals}";
            }
        }

        public string DisplayStatus
        {
            get
            {
                switch (Status)
                {
                    case MatchStatus.Live:
                        return $"{Minute}'";
                    case MatchStatus.Finished:
                        return "FT";
                    default:
                        return "KO";
                }
            }
        }

        public string DisplayKickoff
        {
            get { return $"Kickoff: {Kickoff:dd/MM HH:mm}"; }
        }



        public int CompareTo(Match other)
        {
            if (other == null)
                return 1;

            return Kickoff.CompareTo(other.Kickoff);
        }
    }
}
