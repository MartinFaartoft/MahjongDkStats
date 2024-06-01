namespace MahjongDkStatsCalculators.StatsCalculators;

internal class PlayerStatisticsCalculator : StatisticsCalculatorBase
{
	private readonly Dictionary<string, PlayerStats> Players = [];

	public override void AppendGame(Game game, Ruleset ruleset)
	{
		foreach(var player in game.Players)
		{
			UpdatePlayer(player, game, ruleset);
		}
	}

	public override IEnumerable<PlayerStatistics> GetPlayerStatistics()
	{
		return Players
			.Where(p => p.Value.LatestGame > Constants.ActiveThreshold)
			.Select(kv => new PlayerStatistics(
				kv.Key,
				[
					new Statistic("Games played", kv.Value.GameCount.ToString()),
					new Statistic("Most recent game", kv.Value.LatestGame.ToString("yyyy-MM-dd")),
				],
				GetPlayerRulesetStatistics(kv.Value, Ruleset.Mcr),
				GetPlayerRulesetStatistics(kv.Value, Ruleset.Riichi)));
	}

	private void UpdatePlayer(Player player, Game game, Ruleset ruleset)
	{
		if (!Players.ContainsKey(player.Name))
		{
			Players[player.Name] = new PlayerStats();
		}

		var stats = Players[player.Name];
		var rulesetStats = ruleset == Ruleset.Mcr ? stats.McrStats : stats.RiichiStats;

		stats.GameCount++;
		stats.LatestGame = stats.LatestGame > game.DateOfGame ? stats.LatestGame : game.DateOfGame;

		UpdatePlayerRulesetStats(player, rulesetStats, game);
		UpdatePlayerRulesetHeadToHeadStats(player, rulesetStats, game);
	}

	private void UpdatePlayerRulesetStats(Player player, PlayerRulesetStats stats, Game game)
	{
		stats.Rating[game.DateOfGame] = player.NewRating; // This overwrites previous games played on same day, potentially hiding peaks and troughs in the graph
		stats.LatestGame = (stats.LatestGame?.DateOfGame ?? DateOnly.MinValue) > game.DateOfGame ? stats.LatestGame : game;
		stats.MaxRating = stats.MaxRating > player.NewRating ? stats.MaxRating : player.NewRating;
		stats.GameCount++;
		stats.WindCount += game.NumberOfWinds;
		stats.ScoreSum += player.Score;
		stats.CurrentWinningStreak = player.Score > 0 ? stats.CurrentWinningStreak + 1 : 0;
		stats.LongestWinningStreak = Math.Max(stats.CurrentWinningStreak, stats.LongestWinningStreak);
		stats.GameHistory.Add(new GameHistoryEntry(game, player));	
	}

	private void UpdatePlayerRulesetHeadToHeadStats(Player player, PlayerRulesetStats stats, Game game)
	{
		var h = stats.HeadToHeadStats;

		foreach (var opponent in game.Players.Where(p => p.Name != player.Name))
		{
			if (!h.ContainsKey(opponent.Name))
			{
				h[opponent.Name] = new PlayerRulesetHeadToHeadStats { OpponentName = opponent.Name };
			}

			var s = h[opponent.Name];

			s.ScoreSumAgainst += player.Score;
			s.WindsPlayedAgainst += game.NumberOfWinds;
			s.GamesPlayedAgainst += 1;
		}
	}

	private PlayerRulesetStatistics GetPlayerRulesetStatistics(PlayerStats stats, Ruleset ruleset)
	{
		var rulesetStats = ruleset == Ruleset.Mcr ? stats.McrStats : stats.RiichiStats;
		var rating = GetPlayerRatingHistory(rulesetStats.Rating);
		var headToHeadStatistics = rulesetStats.HeadToHeadStats.Values
			.Select(h => new PlayerRulesetHeadToHeadStatistics(h.OpponentName, h.ScoreSumAgainst, Math.Round(h.ScoreSumAgainst / (decimal)h.WindsPlayedAgainst, 2), h.WindsPlayedAgainst, h.GamesPlayedAgainst))
			.OrderByDescending(h => h.ScoreSumAgainst)
			.ToArray();
		var currentRating = rulesetStats.GameHistory.OrderByDescending(x => x.Game.Id).FirstOrDefault()?.Player.NewRating ?? 0;
		var scorePerWind = Math.Round(rulesetStats.WindCount > 0 ? (decimal)rulesetStats.ScoreSum / rulesetStats.WindCount : 0, 2);
		return new PlayerRulesetStatistics(
					ruleset,
					rating,
					rulesetStats.GameCount,
					rulesetStats.MaxRating,
					currentRating,
					rulesetStats.LatestGame?.DateOfGame ?? DateOnly.MinValue,
					rulesetStats.ScoreSum,
					rulesetStats.LongestWinningStreak,
					scorePerWind,
					headToHeadStatistics
					);
	}

	private DateTimeChart GetPlayerRatingHistory(Dictionary<DateOnly, decimal> rating)
	{
		EnsureRatingHistoryStartsWithZero(rating);

		var dates = rating.Keys.Order().ToArray();
		var values = dates.Select(d => rating[d]);

		return new DateTimeChart(
			dates.Select(d => d.ToDateTime(TimeOnly.MinValue)).ToArray(),
			values.Select(r => (double)r).ToArray());
	}

	private void EnsureRatingHistoryStartsWithZero(Dictionary<DateOnly, decimal> rating)
	{
        if (rating.Count == 0)
        {
			return;
        }

        var minDate = rating.Keys.Min();
		rating[minDate.AddDays(-1)] = 0M;
	}

	private class PlayerStats()
	{
		public int GameCount { get; set; }

		public DateOnly LatestGame { get; set; } = DateOnly.MinValue;

        public PlayerRulesetStats McrStats { get; set; } = new PlayerRulesetStats();

		public PlayerRulesetStats RiichiStats { get; set; } = new PlayerRulesetStats();
    }

	private class PlayerRulesetStats()
	{
		public Game? LatestGame { get; set; }

		public int GameCount { get; set; }

		public int WindCount { get; set; }

		public int ScoreSum { get; set; }

        public decimal MaxRating { get; set; } = decimal.MinValue;

		public int CurrentWinningStreak { get; set; }

		public int LongestWinningStreak { get; set; }

		public Dictionary<DateOnly, decimal> Rating { get; set; } = [];

		public Dictionary<string, PlayerRulesetHeadToHeadStats> HeadToHeadStats { get; set; } = [];

        public List<GameHistoryEntry> GameHistory { get; } = [];
	}

	private class PlayerRulesetHeadToHeadStats()
	{
		public string OpponentName { get; set; } = string.Empty;

        public int ScoreSumAgainst { get; set; }

        public int GamesPlayedAgainst { get; set; }

        public int WindsPlayedAgainst { get; set; }
    }

	private record GameHistoryEntry(Game Game, Player Player);
}
