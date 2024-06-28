namespace MahjongDkStatsCalculators.Calculators;

public class StatisticsCalculator : IStatsCalculator
{
    private PlayerStatisticsCalculator _playerStatsCalc = new();
    private GlobalCountsCalculator _globalCountsCalc = new();
    private RulesetRecordsCalculator _rulesetRecordsCalc = new();
    private YearStatisticsCalculator _yearStatsCalc = new();

    public void AppendGame(Game game, Ruleset ruleset)
    {
        _playerStatsCalc.AppendGame(game, ruleset);
        _globalCountsCalc.AppendGame(game, ruleset);
        _rulesetRecordsCalc.AppendGame(game, ruleset);
        _yearStatsCalc.AppendGame(game, ruleset);
    }
    public GlobalStatistics GetGlobalStatistics() => _globalCountsCalc.GetGlobalStatistics();
    public RuleSetRecords GetMcrRecords() => _rulesetRecordsCalc.GetMcrRecords();
    public RuleSetRecords GetRiichiRecords() => _rulesetRecordsCalc.GetRiichiRecords();
	public IEnumerable<PlayerStatistics> GetPlayerStatistics() => _playerStatsCalc.GetPlayerStatistics();
    public IEnumerable<YearStatistics> GetYearStatistics() => _yearStatsCalc.GetYearStatistics();
}
