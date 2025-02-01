
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
            foreach(var player in game.Players)
            {
                if (s.McrPlayers.ContainsKey(player.Name))
                {
                    s.McrPlayers[player.Name] += game.NumberOfWinds;
                } else
                {
					s.McrPlayers[player.Name] = game.NumberOfWinds;
				}
            }
        }
        else if (ruleset == Ruleset.Riichi)
        {
            s.RiichiGameCount++;
			foreach (var player in game.Players)
			{
				if (s.RiichiPlayers.ContainsKey(player.Name))
				{
					s.RiichiPlayers[player.Name] += game.NumberOfWinds;
				}
				else
				{
					s.RiichiPlayers[player.Name] = game.NumberOfWinds;
				}
			}
		}
    }

    internal IEnumerable<YearStatistics> GetYearStatistics()
    {
        return _yearStats.Select(kv => CalculateYearStatistics(kv.Key, kv.Value)).OrderBy(y => y.Year);
    }

    private YearStatistics CalculateYearStatistics(int year, YearStats yearStats)
    {
        var mostActiveMcrPlayer = yearStats.McrPlayers.Any() ? yearStats.McrPlayers.MaxBy(x => x.Value) : new KeyValuePair<string, int>("No games played", 0);
		var mostActiveRiichiPlayer = yearStats.RiichiPlayers.MaxBy(x => x.Value);
        var mergedPlayers = MergePlayers(yearStats.McrPlayers, yearStats.RiichiPlayers);
        var mostActivePlayer = mergedPlayers.MaxBy(x => x.Value);

        return new YearStatistics(
            year,
            yearStats.McrGameCount,
            yearStats.RiichiGameCount,
            yearStats.McrPlayers.Count,
            new PlayerYearWindCount(mostActiveMcrPlayer.Key, mostActiveMcrPlayer.Value),
            yearStats.RiichiPlayers.Count,
            new PlayerYearWindCount(mostActiveRiichiPlayer.Key, mostActiveRiichiPlayer.Value),
            new PlayerYearWindCount(mostActivePlayer.Key, mostActivePlayer.Value));
	}

	private Dictionary<string, int> MergePlayers(Dictionary<string, int> mcrPlayers, Dictionary<string, int> riichiPlayers)
	{
		var keys = mcrPlayers.Keys.Union(riichiPlayers.Keys);
        return keys.ToDictionary(k => k, v => (mcrPlayers.TryGetValue(v, out int mcr) ? mcr : 0) + (riichiPlayers.TryGetValue(v, out int riichi) ? riichi : 0));
	}

	private class YearStats
    {
        public int McrGameCount { get; set; }

        public int RiichiGameCount { get; set; }

		public Dictionary<string, int> McrPlayers { get; set; } = new();

		public Dictionary<string, int> RiichiPlayers { get; set; } = new ();
    }
}