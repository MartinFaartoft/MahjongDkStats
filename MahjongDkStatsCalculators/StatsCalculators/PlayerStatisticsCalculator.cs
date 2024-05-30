namespace MahjongDkStatsCalculators.StatsCalculators;

internal class PlayerStatisticsCalculator : StatisticsCalculatorBase
{
	private static readonly DateOnly ActiveThreshold = DateOnly.FromDateTime(DateTime.Now.AddYears(-1));

	private readonly Dictionary<string, PlayerStats> Players = [];

	public override void AppendGame(Game game, GameType gameType)
	{
		foreach(var player in game.Players)
		{
			UpdatePlayer(player, game, gameType);
		}
	}

	public override IEnumerable<PlayerStatistics> GetPlayerStatistics()
	{
		return Players
			.Where(p => p.Value.LatestGame > ActiveThreshold)
			.Select(kv => new PlayerStatistics(
				kv.Key,
				[
					new Statistic("Games played", kv.Value.GameCount.ToString()),
					new Statistic("Most recent game", kv.Value.LatestGame.ToString("yyyy-MM-dd")),
				],
				GetPlayerVariantStatistics(kv.Value, GameType.Mcr),
				GetPlayerVariantStatistics(kv.Value, GameType.Riichi)));
	}

	private void UpdatePlayer(Player player, Game game, GameType gameType)
	{
		if (!Players.ContainsKey(player.Name))
		{
			Players[player.Name] = new PlayerStats();
		}

		var stats = Players[player.Name];
		var variantStats = gameType == GameType.Mcr ? stats.McrStats : stats.RiichiStats;

		stats.GameCount++;
		stats.LatestGame = stats.LatestGame > game.DateOfGame ? stats.LatestGame : game.DateOfGame;

		UpdatePlayerVariantStats(player, variantStats, game);
	}

	private void UpdatePlayerVariantStats(Player player, PlayerVariantStats stats, Game game)
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

	private PlayerVariantStatistics GetPlayerVariantStatistics(PlayerStats stats, GameType gameType)
	{
		var variantStats = gameType == GameType.Mcr ? stats.McrStats : stats.RiichiStats;
		var rating = GetPlayerRatingHistory(variantStats.Rating);
		var currentRating = variantStats.GameHistory.OrderByDescending(x => x.Game.Id).FirstOrDefault()?.Player.NewRating ?? 0;
		var scorePerWind = Math.Round(variantStats.WindCount > 0 ? (decimal)variantStats.ScoreSum / variantStats.WindCount : 0, 2);
		return new PlayerVariantStatistics(
					rating,
					variantStats.GameCount,
					variantStats.MaxRating,
					currentRating,
					variantStats.LatestGame?.DateOfGame ?? DateOnly.MinValue,
					variantStats.ScoreSum,
					variantStats.LongestWinningStreak,
					scorePerWind
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

        public PlayerVariantStats McrStats { get; set; } = new PlayerVariantStats();

		public PlayerVariantStats RiichiStats { get; set; } = new PlayerVariantStats();
    }

	private class PlayerVariantStats()
	{
		public Game? LatestGame { get; set; }

		public int GameCount { get; set; }

		public int WindCount { get; set; }

		public int ScoreSum { get; set; }

        public decimal MaxRating { get; set; } = decimal.MinValue;

		public int CurrentWinningStreak { get; set; }

		public int LongestWinningStreak { get; set; }

		public Dictionary<DateOnly, decimal> Rating { get; set; } = [];

		public List<GameHistoryEntry> GameHistory { get; } = [];
	}

	private record GameHistoryEntry(Game Game, Player Player);
}
