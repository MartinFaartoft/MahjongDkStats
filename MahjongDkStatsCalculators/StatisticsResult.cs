namespace MahjongDkStatsCalculators;

public record StatisticsResult(
    GlobalStatistics GlobalStatistics,
    RuleSetRecords McrRecords,
    RuleSetRecords RiichiRecords,
    PlayerStatistics[] PlayerStatistics,
	PlayerStatistics[] MemberStatistics,
	RatingEntry[] McrRatingList,
	RatingEntry[] RiichiRatingList,
    YearStatistics[] YearStatistics
	);