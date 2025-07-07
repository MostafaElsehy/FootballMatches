namespace FootballMatches.Models
{
    public class Match
    {
        public int Id { get; set; }
        public string? HomeTeam { get; set; }
        public string? AwayTeam { get; set; }
        public string? HomeTeamLogo { get; set; }
        public string? AwayTeamLogo { get; set; }
        public DateTime MatchTime { get; set; }
        public string? Competition { get; set; }
        public string? Country { get; set; }
        public string? Channel { get; set; }
    }
} 