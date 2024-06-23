namespace MahjongDkStatsCalculators;

public record GlobalStatistics(int TotalGameCount, int TotalWindsCount, int TotalHandsCount, TimeSpan TimeSpentShufflingAndBuilding, decimal TotalWallLengthMeters);