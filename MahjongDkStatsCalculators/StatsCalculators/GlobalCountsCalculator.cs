namespace MahjongDkStatsCalculators.StatsCalculators;

internal class GlobalCountsCalculator : StatsCalculatorBase
{
    private int _gameCount = 0;
    private int _windsCount = 0;
    private HashSet<string> _uniquePlayers = new();

    public override void AppendGame(Game game, GameType gameType)
    {
        _gameCount++;
        _windsCount += game.NumberOfWinds;
        _uniquePlayers.UnionWith(game.Players.Select(p => p.Name));
    }

    public override IEnumerable<Statistic> GetGlobalStatistics()
    {
        return [
            new Statistic("Total Games", _gameCount.ToString()),
            new Statistic("Total Winds", _windsCount.ToString()),
            new Statistic("Unique Players", _uniquePlayers.Count.ToString()),
            ];
    }
}
