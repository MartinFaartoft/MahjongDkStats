﻿namespace MahjongDkStatsCalculators.Calculators;

internal abstract class StatisticsCalculatorBase : IStatsCalculator
{
    public abstract void AppendGame(Game game, Ruleset ruleset);
    public virtual GlobalStatistics GetGlobalStatistics() => null!;
    public virtual IEnumerable<Statistic> GetGlobalMcrStatistics() => [];
    public virtual IEnumerable<Statistic> GetGlobalRiichiStatistics() => [];
	public virtual IEnumerable<PlayerStatistics> GetPlayerStatistics() => [];
}
