namespace MahjongDkStatsCalculators.Calculators;

internal class GlobalCountsCalculator
{
	protected int _gameCount = 0;
	protected int _windsCount = 0;
	protected int _handsCount = 0;

	internal void AppendGame(Game game, Ruleset ruleset)
	{
		_gameCount++;
		_windsCount += game.NumberOfWinds;
		_handsCount += game.NumberOfWinds * game.Players.Count();
	}

	private const decimal TileWidthInM = .02M;
	private const int MinutesPerBuildAndShuffle = 2;

	internal GlobalStatistics GetGlobalStatistics()
	{
		return new GlobalStatistics(
			_gameCount,
			_windsCount,
			_handsCount,
			TimeSpan.FromMinutes(_handsCount * MinutesPerBuildAndShuffle),
			_handsCount * 4 * 17 * TileWidthInM
			);
	}
}
