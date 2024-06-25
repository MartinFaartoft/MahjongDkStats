namespace MahjongDkStatsCalculators;

public record GlobalStatistics(
    int TotalGameCount,
    int TotalWindsCount,
    int TotalHandsCount,
    int TotalPlayerCount,
    int ActivePlayerCount,
    TimeSpan TimeSpentShufflingAndBuilding,
    decimal TotalWallLengthMeters);