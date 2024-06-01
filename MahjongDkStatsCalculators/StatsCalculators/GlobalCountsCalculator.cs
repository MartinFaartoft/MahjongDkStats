namespace MahjongDkStatsCalculators.StatsCalculators;

internal class GlobalCountsCalculator : StatisticsCalculatorBase
{
    protected int _gameCount = 0;
	protected int _windsCount = 0;
	protected int _handsCount = 0;

    public override void AppendGame(Game game, Ruleset ruleset)
    {
        _gameCount++;
        _windsCount += game.NumberOfWinds;
        _handsCount += game.NumberOfWinds * game.Players.Count();
    }

    private const decimal TileWidthInM = .02M;

    public override IEnumerable<Statistic> GetGlobalStatistics()
    {
        return [
            new Statistic("Games played", _gameCount.ToString()),
            new Statistic("Winds played", _windsCount.ToString()),
			new Statistic("Hands played", _handsCount.ToString()),
            new Statistic("Time spent shuffling and building walls", Math.Round(TimeSpan.FromMinutes(_handsCount * 2).TotalDays).ToString() + " days"),
            new Statistic("Total length of walls built", Math.Round(_handsCount * 4 * 17 * TileWidthInM / 1000M).ToString() + " km")
            ];
    }
}
