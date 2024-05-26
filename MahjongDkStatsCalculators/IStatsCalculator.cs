namespace MahjongDkStatsCalculators;

public interface IStatsCalculator
{
    public void AppendGame(Game game, GameType gameType);

    public IEnumerable<Statistic> GetGlobalStatistics();

    public IEnumerable<Statistic> GetGlobalMcrStatistics();
    IEnumerable<Statistic> GetGlobalRiichiStatistics();
}
