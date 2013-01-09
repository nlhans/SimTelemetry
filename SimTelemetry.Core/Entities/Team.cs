namespace SimTelemetry.Core.Entities
{
    public class Team
    {
        // Name
        public string Name { get; private set; }

        // Additional information
        public string Headquarters { get; private set; }
        public int Founded { get; private set; }

        // Statistics
        public int Wins { get; private set; }
        public int Poles { get; private set; }
        public int Championships { get; private set; }
        public int Seasons { get; private set; }
        public int Races { get; private set; }

        public Team(string name, string headquarters, int founded, int wins, int poles, int championships, int seasons, int races)
        {
            Name = name;
            Headquarters = headquarters;
            Founded = founded;
            Wins = wins;
            Poles = poles;
            Championships = championships;
            Seasons = seasons;
            Races = races;
        }
    }
}