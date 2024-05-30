﻿namespace MahjongDkStatsCalculators.StatsCalculators;

internal abstract class StatisticsCalculatorBase : IStatsCalculator
{
    public abstract void AppendGame(Game game, GameType gameType);
    public virtual IEnumerable<Statistic> GetGlobalStatistics() => [];
    public virtual IEnumerable<Statistic> GetGlobalMcrStatistics() => [];
    public virtual IEnumerable<Statistic> GetGlobalRiichiStatistics() => [];
	public virtual IEnumerable<PlayerStatistics> GetPlayerStatistics() => [];
}