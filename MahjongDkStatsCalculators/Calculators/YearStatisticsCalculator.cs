namespace MahjongDkStatsCalculators.Calculators;

internal class YearStatisticsCalculator
{ 
    private Dictionary<int, YearStats> _yearStats = [];

    internal void AppendGame(Game game, Ruleset ruleset)
    {
        var year = game.DateOfGame.Year;

        if (!_yearStats.TryGetValue(year, out YearStats? value))
        {
            value = new YearStats();
            _yearStats.Add(year, value);
        }

        var s = value;

        if (ruleset == Ruleset.Mcr)
        {
            s.McrGameCount++;
            s.McrPlayerNames.UnionWith(game.Players.Select(p => p.Name));
        }
        else if (ruleset == Ruleset.Riichi)
        {
            s.RiichiGameCount++;
            s.RiichiPlayerNames.UnionWith(game.Players.Select(p => p.Name));
        }
    }

    internal IEnumerable<YearStatistics> GetYearStatistics()
    {
        return _yearStats.Select(kv => new YearStatistics(kv.Key, kv.Value.McrGameCount, kv.Value.RiichiGameCount, kv.Value.McrPlayerNames.Count, kv.Value.RiichiPlayerNames.Count))
            .OrderBy(y => y.Year);
    }
    

    private class YearStats
    {
        public int McrGameCount { get; set; }

        public int RiichiGameCount { get; set; }

        public HashSet<string> McrPlayerNames { get; set; } = new HashSet<string>();
        
        public HashSet<string> RiichiPlayerNames { get; set; } = new HashSet<string>();
    }
}