namespace MahjongDkStatsCalculators.Calculators;

internal class GlobalMcrCountsCalculator : GlobalCountsCalculator
{
	public override void AppendGame(Game game, Ruleset ruleset)
	{
		if (ruleset == Ruleset.Mcr)
		{
			base.AppendGame(game, Ruleset.Mcr);
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

	public override GlobalStatistics GetGlobalStatistics() => null!;
}
