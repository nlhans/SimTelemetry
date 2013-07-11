namespace SimTelemetry.Domain.Entities
{
    public class Team
    {
        public int Position { get; private set; }
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

        public Team(int position, string name, string headquarters, int founded, int wins, int poles, int championships, int seasons, int races)
        {
            Position = position;
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