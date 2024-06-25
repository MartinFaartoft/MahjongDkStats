namespace MahjongDkStatsCalculators;

public record PlayerRulesetStatistics(
	Ruleset Ruleset,
	DateTimeChart Rating,
	DateTimeChart RatingListPosition,
	int GameCount,
	int WindCount,
	int HandCount,
	decimal MaxRating,
	decimal CurrentRating,
	DateOnly LatestGame,
	int ScoreSum,
	RecordGame<int> LongestWinningStreak,
	RecordGame<int> RecordGameScore,
	decimal ScorePerWind, 
	IEnumerable<PlayerRulesetHeadToHeadStatistics> HeadToHeadStatistics,
	IEnumerable<Game> GameHistory,
	IEnumerable<PlayerRatingListPositionEntry> RatingListPositionHistory);
