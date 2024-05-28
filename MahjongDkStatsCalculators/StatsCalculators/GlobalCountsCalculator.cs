namespace MahjongDkStatsCalculators.StatsCalculators;

internal class GlobalCountsCalculator : StatisticsCalculatorBase
{
    protected int _gameCount = 0;
	protected int _windsCount = 0;
	protected int _handsCount = 0;
	protected HashSet<string> _uniquePlayers = new();
	protected HashSet<string> _activePlayers = new();
	protected DateOnly ActiveIfGameAfter = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1));

    public override void AppendGame(Game game, GameType gameType)
    {
        _gameCount++;
        _windsCount += game.NumberOfWinds;
        _handsCount += game.NumberOfWinds * game.Players.Count();
        _uniquePlayers.UnionWith(game.Players.Select(p => p.Name));
        if (game.DateOfGame > ActiveIfGameAfter)
        {
            _activePlayers.UnionWith(game.Players.Select(p => p.Name));
        }
    }

    public override IEnumerable<Statistic> GetGlobalStatistics()
    {
        return [
            new Statistic("Games", _gameCount.ToString()),
            new Statistic("Winds", _windsCount.ToString()),
			new Statistic("Hands", _handsCount.ToString()),
			new Statistic("Players", _uniquePlayers.Count.ToString()),
            new Statistic("Active Players (last year)", _activePlayers.Count.ToString()),
            ];
    }
}
