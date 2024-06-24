namespace MahjongDkStatsCalculators.Calculators;

public class StatisticsCalculator : IStatsCalculator
{
    private PlayerStatisticsCalculator _playerStatsCalc = new PlayerStatisticsCalculator();
    private GlobalCountsCalculator _globalCountsCalc = new GlobalCountsCalculator();
    private RulesetRecordsCalculator _rulesetRecordsCalc = new RulesetRecordsCalculator();

    public void AppendGame(Game game, Ruleset ruleset)
    {
        _playerStatsCalc.AppendGame(game, ruleset);
        _globalCountsCalc.AppendGame(game, ruleset);
        _rulesetRecordsCalc.AppendGame(game, ruleset);
    }
    public GlobalStatistics GetGlobalStatistics() => _globalCountsCalc.GetGlobalStatistics();
    public RuleSetRecords GetMcrRecords() => _rulesetRecordsCalc.GetMcrRecords();
    public RuleSetRecords GetRiichiRecords() => _rulesetRecordsCalc.GetRiichiRecords();
	public IEnumerable<PlayerStatistics> GetPlayerStatistics() => _playerStatsCalc.GetPlayerStatistics();
}
