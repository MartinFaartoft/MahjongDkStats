namespace MahjongDkStatsCalculators.StatsCalculators;

internal class GlobalCountsCalculator : StatisticsCalculatorBase
{
    protected int _gameCount = 0;
	protected int _windsCount = 0;
	protected int _handsCount = 0;

    public override void AppendGame(Game game, GameType gameType)
    {
        _gameCount++;
        _windsCount += game.NumberOfWinds;
        _handsCount += game.NumberOfWinds * game.Players.Count();
    }

    public override IEnumerable<Statistic> GetGlobalStatistics()
    {
        return [
            new Statistic("Games", _gameCount.ToString()),
            new Statistic("Winds", _windsCount.ToString()),
			new Statistic("Hands", _handsCount.ToString())
            ];
    }
}
