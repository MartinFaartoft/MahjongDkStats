namespace MahjongDkStatsCalculators;

public record StatisticsResult(
    GlobalStatistics GlobalStatistics,
    RuleSetRecords McrRecords,
    RuleSetRecords RiichiRecords,
    PlayerStatistics[] PlayerStatistics,
    RatingEntry[] McrRatingList,
	RatingEntry[] RiichiRatingList
	);