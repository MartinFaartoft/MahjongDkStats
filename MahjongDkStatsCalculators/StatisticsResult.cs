namespace MahjongDkStatsCalculators;

public record StatisticsResult(
    IEnumerable<Statistic> GlobalStatistics, 
    IEnumerable<Statistic> McrStatistics,
    IEnumerable<Statistic> RiichiStatistics,
    IEnumerable<PlayerStatistics> PlayerStatistics,
    IEnumerable<RatingEntry> McrRatingList,
	IEnumerable<RatingEntry> RiichiRatingList
	);