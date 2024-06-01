namespace MahjongDkStatsCalculators;

public record StatisticsResult(
    Statistic[] GlobalStatistics,
    Statistic[] McrStatistics,
    Statistic[] RiichiStatistics,
    PlayerStatistics[] PlayerStatistics,
    RatingEntry[] McrRatingList,
	RatingEntry[] RiichiRatingList
	);