namespace MahjongDkStatsCalculators.StatsCalculators;

internal class GlobalMcrCountsCalculator : GlobalCountsCalculator
{
    public override void AppendGame(Game game, GameType gameType)
    {
        if (gameType == GameType.Mcr)
        {
            base.AppendGame(game, GameType.Mcr);
        }
    }

    public override IEnumerable<Statistic> GetGlobalMcrStatistics()
    {
        return [
            new Statistic("Games", _gameCount.ToString()),
            new Statistic("Winds", _windsCount.ToString()),
			new Statistic("Hands", _handsCount.ToString())
            ];
    }

    public override IEnumerable<Statistic> GetGlobalStatistics() => [];
}
