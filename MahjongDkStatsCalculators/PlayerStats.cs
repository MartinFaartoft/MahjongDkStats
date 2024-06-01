namespace MahjongDkStatsCalculators;

public record PlayerStatistics(
	string Name,
	IEnumerable<Statistic> Statistics, 
	PlayerRulesetStatistics McrStatistics, 
	PlayerRulesetStatistics RiichiStatistics
	);

public record DateTimeChart(DateTime[] X, double[] Y);

public record PlayerRulesetStatistics(
	Ruleset Ruleset,
	DateTimeChart Rating,
	int GameCount,
	decimal MaxRating,
	decimal CurrentRating,
	DateOnly LatestGame,
	int ScoreSum,
	int LongestWinningStreak,
	decimal ScorePerWind);