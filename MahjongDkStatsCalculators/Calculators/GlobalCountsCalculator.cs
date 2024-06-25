namespace MahjongDkStatsCalculators.Calculators;

internal class GlobalCountsCalculator
{
	private int _gameCount = 0;
    private int _windsCount = 0;
    private int _handsCount = 0;
	private HashSet<string> _allPlayers = new();
    private HashSet<string> _activePlayers = new();

    internal void AppendGame(Game game, Ruleset ruleset)
	{
		_gameCount++;
		_windsCount += game.NumberOfWinds;
		_handsCount += game.NumberOfWinds * game.Players.Count();
        foreach (var player in game.Players)
        {
			_allPlayers.Add(player.Name);
			if (game.DateOfGame > Constants.ActiveThreshold)
			{
				_activePlayers.Add(player.Name);
			}
        }
    }

	private const decimal TileWidthInM = .02M;
	private const int MinutesPerBuildAndShuffle = 2;

	internal GlobalStatistics GetGlobalStatistics()
	{
		return new GlobalStatistics(
			_gameCount,
			_windsCount,
			_handsCount,
			_allPlayers.Count,
			_activePlayers.Count,
			TimeSpan.FromMinutes(_handsCount * MinutesPerBuildAndShuffle),
			_handsCount * 4 * 17 * TileWidthInM
			);
	}
}
