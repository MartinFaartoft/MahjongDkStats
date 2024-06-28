namespace MahjongDkStatsCalculators;

public interface IStatsCalculator
{
    void AppendGame(Game game, Ruleset ruleset);
    GlobalStatistics GetGlobalStatistics();
    RuleSetRecords GetMcrRecords();
    RuleSetRecords GetRiichiRecords();
    IEnumerable<PlayerStatistics> GetPlayerStatistics();
    IEnumerable<YearStatistics> GetYearStatistics();
}
