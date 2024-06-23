namespace MahjongDkStatsCalculators;

public record StatisticsResult(
    GlobalStatistics GlobalStatistics,
    Statistic[] McrStatistics,
    Statistic[] RiichiStatistics,
    PlayerStatistics[] PlayerStatistics,
    RatingEntry[] McrRatingList,
	RatingEntry[] RiichiRatingList
	);