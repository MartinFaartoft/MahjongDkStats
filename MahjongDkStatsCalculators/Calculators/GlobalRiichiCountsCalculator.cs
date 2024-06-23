namespace MahjongDkStatsCalculators.Calculators;

internal class GlobalRiichiCountsCalculator : GlobalCountsCalculator
{
	public override void AppendGame(Game game, Ruleset ruleset)
	{
		if (ruleset == Ruleset.Riichi)
		{
			base.AppendGame(game, Ruleset.Riichi);
		}
	}

	public override IEnumerable<Statistic> GetGlobalRiichiStatistics()
	{
		return [
			new Statistic("Games played", _gameCount.ToString()),
			new Statistic("Winds played", _windsCount.ToString()),
			new Statistic("Hands played", _handsCount.ToString()),
			];
	}

	public override GlobalStatistics GetGlobalStatistics() => null!;
}
