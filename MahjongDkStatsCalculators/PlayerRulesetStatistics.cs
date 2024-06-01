namespace MahjongDkStatsCalculators;

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