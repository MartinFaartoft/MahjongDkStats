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
	decimal ScorePerWind, 
	IEnumerable<PlayerRulesetHeadToHeadStatistics> HeadToHeadStatistics);

public record PlayerRulesetHeadToHeadStatistics(string OpponentName, int ScoreSumAgainst, decimal ScorePerWindAgainst, int WindsPlayedAgainst, int GamesPlayedAgainst);