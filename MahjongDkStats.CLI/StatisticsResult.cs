using MahjongDkStatsCalculators;

namespace MahjongDkStats.CLI;

public record StatisticsResult(
    GlobalStatistics GlobalStatistics,
    RuleSetRecords McrRecords,
    RuleSetRecords RiichiRecords,
    PlayerStatistics[] PlayerStatistics,
    PlayerStatistics[] MemberStatistics,
    RatingEntry[] McrRatingList,
    RatingEntry[] RiichiRatingList,
    YearStatistics[] YearStatistics,
    DateOnly NewestGameDate
    );