namespace MahjongDkStatsCalculators.StatsCalculators;

internal class PlayerStatisticsCalculator : StatisticsCalculatorBase
{
	private readonly Dictionary<string, PlayerStats> Players = [];

	public override void AppendGame(Game game, GameType gameType)
	{
		foreach(var player in game.Players)
		{
			UpdatePlayer(player, game.DateOfGame);
		}
	}

	public override IEnumerable<PlayerStatistics> GetPlayerStatistics()
	{
		return Players.Select(kv => new PlayerStatistics(kv.Key, [new Statistic("Game count", kv.Value.GameCount.ToString()), new Statistic("Most recent game", kv.Value.LatestGame.ToString("yyyy-MM-dd"))]));
		
	}

	private void UpdatePlayer(Player player, DateOnly dateOfGame)
	{
		if (!Players.ContainsKey(player.Name))
		{
			Players[player.Name] = new PlayerStats { GameCount = 0, LatestGame = DateOnly.MinValue };
		}

		var stats = Players[player.Name];

		stats.GameCount++;

		stats.LatestGame = stats.LatestGame > dateOfGame ? stats.LatestGame : dateOfGame;
	}

	private class PlayerStats()
	{
		public int GameCount { get; set; }

		public DateOnly LatestGame { get; set; }
	}
}
