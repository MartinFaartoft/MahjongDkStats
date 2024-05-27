namespace MahjongDkStatsCalculators.StatsCalculators;

internal class GlobalRiichiCountsCalculator : GlobalCountsCalculator
{
    public override void AppendGame(Game game, GameType gameType)
    {
        if (gameType == GameType.Riichi)
        {
            base.AppendGame(game, GameType.Riichi);
        }
    }

    public override IEnumerable<Statistic> GetGlobalRiichiStatistics()
    {
        return [
            new Statistic("Games", _gameCount.ToString()),
            new Statistic("Winds", _windsCount.ToString()),
			new Statistic("Hands", _handsCount.ToString()),
			new Statistic("Players", _uniquePlayers.Count.ToString()),
            new Statistic("Active Players (last year)", _activePlayers.Count.ToString()),
            ];
    }

    public override IEnumerable<Statistic> GetGlobalStatistics() => [];
}
