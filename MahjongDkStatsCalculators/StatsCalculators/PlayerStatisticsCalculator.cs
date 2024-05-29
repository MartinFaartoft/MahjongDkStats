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
				[new Statistic("Game count", kv.Value.GameCount.ToString()), new Statistic("Most recent game", kv.Value.LatestGame.ToString("yyyy-MM-dd"))],
				new DateTimeChart(kv.Value.McrRating.Keys.Select(d => d.ToDateTime(TimeOnly.MinValue)).ToArray(), kv.Value.McrRating.Values.Select(r => (double)r).ToArray())));
	}

	private void UpdatePlayer(Player player, Game game, GameType gameType)
	{
		if (!Players.ContainsKey(player.Name))
		{
			Players[player.Name] = new PlayerStats { GameCount = 0, LatestGame = DateOnly.MinValue };
		}

		var stats = Players[player.Name];

		stats.GameCount++;

		stats.LatestGame = stats.LatestGame > game.DateOfGame ? stats.LatestGame : game.DateOfGame;

		if (gameType == GameType.Mcr)
		{
			stats.McrRating[game.DateOfGame] = player.NewRating;
		}
	}

	private class PlayerStats()
	{
		public int GameCount { get; set; }

		public DateOnly LatestGame { get; set; }

		public Dictionary<DateOnly, decimal> McrRating { get; set; } = [];
    }
}
