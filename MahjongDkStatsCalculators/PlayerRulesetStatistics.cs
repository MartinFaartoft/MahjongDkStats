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
	RecordGame<int> LongestWinningStreak,
	RecordGame<int> RecordGameScore,
	decimal ScorePerWind, 
	IEnumerable<PlayerRulesetHeadToHeadStatistics> HeadToHeadStatistics,
	IEnumerable<Game> GameHistory,
	IEnumerable<PlayerRatingListPositionEntry> RatingListPositionHistory);

public record RecordGame<T>(Game Game, string PlayerName, T RecordValue)
{
	public static RecordGame<T> None(T initialValue)
		=> new RecordGame<T>(Game.None, string.Empty, initialValue);
}