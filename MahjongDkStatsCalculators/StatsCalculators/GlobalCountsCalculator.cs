namespace MahjongDkStatsCalculators.StatsCalculators;

internal class GlobalCountsCalculator : StatsCalculatorBase
{
    private int _gameCount = 0;
    private int _windsCount = 0;
    private HashSet<string> _uniquePlayers = new();
    private HashSet<string> _activePlayers = new();
    private DateOnly ActiveIfGameAfter = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1));

    public override void AppendGame(Game game, GameType gameType)
    {
        _gameCount++;
        _windsCount += game.NumberOfWinds;
        _uniquePlayers.UnionWith(game.Players.Select(p => p.Name));
        if (game.DateOfGame > ActiveIfGameAfter)
        {
            _activePlayers.UnionWith(game.Players.Select(p => p.Name));
        }
    }

    public override IEnumerable<Statistic> GetGlobalStatistics()
    {
        return [
            new Statistic("Total Games", _gameCount.ToString()),
            new Statistic("Total Winds", _windsCount.ToString()),
            new Statistic("Total Players", _uniquePlayers.Count.ToString()),
            new Statistic("Active Players (last year)", _activePlayers.Count.ToString()),
            ];
    }
}
