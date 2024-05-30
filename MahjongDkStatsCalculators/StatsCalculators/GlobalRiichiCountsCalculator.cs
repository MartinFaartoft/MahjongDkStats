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
            ];
    }

    public override IEnumerable<Statistic> GetGlobalStatistics() => [];
}
