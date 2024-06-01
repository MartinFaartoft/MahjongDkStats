namespace MahjongDkStatsCalculators;

public record PlayerStatistics(
	string Name,
	IEnumerable<Statistic> Statistics, 
	PlayerRulesetStatistics McrStatistics, 
	PlayerRulesetStatistics RiichiStatistics
	);
