using MahjongDkStatsCalculators.StatsCalculators;

namespace MahjongDkStatsCalculators;

public record PlayerRulesetStatistics(
	Ruleset Ruleset,
	DateTimeChart Rating,
	DateTimeChart RatingListPosition,
	int GameCount,
	decimal MaxRating,
	decimal CurrentRating,
	DateOnly LatestGame,
	int ScoreSum,
	int LongestWinningStreak,
	decimal ScorePerWind, 
	IEnumerable<PlayerRulesetHeadToHeadStatistics> HeadToHeadStatistics,
	IEnumerable<Game> GameHistory,
	IEnumerable<PlayerRatingListPositionEntry> RatingListPositionHistory);