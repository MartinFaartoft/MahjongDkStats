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
            new Statistic("Games played", _gameCount.ToString()),
            new Statistic("Winds played", _windsCount.ToString()),
			new Statistic("Hands played", _handsCount.ToString())
            ];
    }

    public override IEnumerable<Statistic> GetGlobalStatistics() => [];
}
