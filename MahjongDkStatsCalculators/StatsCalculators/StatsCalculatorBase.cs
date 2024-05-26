namespace MahjongDkStatsCalculators.StatsCalculators;

internal abstract class StatsCalculatorBase : IStatsCalculator
{
    public abstract void AppendGame(Game game, GameType gameType);
    public virtual IEnumerable<Statistic> GetGlobalStatistics() => [];
    public virtual IEnumerable<Statistic> GetGlobalMcrStatistics() => [];
    public virtual IEnumerable<Statistic> GetGlobalRiichiStatistics() => [];
}
