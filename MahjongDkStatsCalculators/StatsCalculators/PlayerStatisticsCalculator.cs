namespace MahjongDkStatsCalculators.StatsCalculators;

internal class PlayerStatisticsCalculator : StatisticsCalculatorBase
{
	private readonly Dictionary<string, PlayerStats> Players = [];
	private readonly DateOnly activeThreshold = DateOnly.FromDateTime(DateTime.Now.AddYears(-1));

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
			.Where(p => p.Value.LatestGame > activeThreshold)
			.Select(kv => new PlayerStatistics(
				kv.Key,
				[
					new Statistic("Game count", kv.Value.GameCount.ToString()),
					new Statistic("Most recent game", kv.Value.LatestGame.ToString("yyyy-MM-dd")),
					new Statistic("", kv.Value.GameCount.ToString()),
				],
				new PlayerVariantStatistics(
					new DateTimeChart(kv.Value.McrRating.Keys.Select(d => d.ToDateTime(TimeOnly.MinValue)).ToArray(), kv.Value.McrRating.Values.Select(r => (double)r).ToArray()),
					kv.Value.McrGameCount,
					kv.Value.MaxMcrRating,
					kv.Value.LatestMcrGame),
				new PlayerVariantStatistics(
					new DateTimeChart(kv.Value.RiichiRating.Keys.Select(d => d.ToDateTime(TimeOnly.MinValue)).ToArray(), kv.Value.RiichiRating.Values.Select(r => (double)r).ToArray()),
					kv.Value.RiichiGameCount,
					kv.Value.MaxRiichiRating,
					kv.Value.LatestRiichiGame)));
	}

	private void UpdatePlayer(Player player, Game game, GameType gameType)
	{
		if (!Players.ContainsKey(player.Name))
		{
			Players[player.Name] = new PlayerStats();
		}

		var stats = Players[player.Name];

		stats.GameCount++;

		stats.LatestGame = stats.LatestGame > game.DateOfGame ? stats.LatestGame : game.DateOfGame;

		if (gameType == GameType.Mcr)
		{
			stats.McrRating[game.DateOfGame] = player.NewRating; // TODO fix games on same date replacing each other
			stats.LatestMcrGame = stats.LatestMcrGame > game.DateOfGame ? stats.LatestMcrGame : game.DateOfGame;
			stats.MaxMcrRating = stats.MaxMcrRating > player.NewRating ? stats.MaxMcrRating : player.NewRating;
			stats.McrGameCount++;
		}

		if (gameType == GameType.Riichi)
		{
			stats.RiichiRating[game.DateOfGame] = player.NewRating; // TODO fix games on same date replacing each other
			stats.LatestRiichiGame = stats.LatestRiichiGame > game.DateOfGame ? stats.LatestRiichiGame : game.DateOfGame;
			stats.MaxRiichiRating = stats.MaxRiichiRating > player.NewRating ? stats.MaxRiichiRating : player.NewRating;
			stats.RiichiGameCount++;
		}
	}

	private class PlayerStats()
	{
		public int GameCount { get; set; }

		public DateOnly LatestGame { get; set; } = DateOnly.MinValue;

		public DateOnly LatestMcrGame { get; set; } = DateOnly.MinValue;

		public DateOnly LatestRiichiGame { get; set; } = DateOnly.MinValue;

		public int McrGameCount { get; set; }

		public int RiichiGameCount { get; set; }

		public decimal MaxMcrRating { get; set; } = decimal.MinValue;

		public decimal MaxRiichiRating { get; set; } = decimal.MinValue;

		public Dictionary<DateOnly, decimal> McrRating { get; set; } = [];

		public Dictionary<DateOnly, decimal> RiichiRating { get; set; } = [];
	}
}
