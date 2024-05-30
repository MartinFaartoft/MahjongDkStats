﻿namespace MahjongDkStatsCalculators;

public record PlayerStatistics(string Name, IEnumerable<Statistic> Statistics, PlayerVariantStatistics McrStatistics, PlayerVariantStatistics RiichiStatistics);

public record DateTimeChart(DateTime[] X, double[] Y);

public record PlayerVariantStatistics(DateTimeChart Rating, int GameCount, decimal MaxRating, DateOnly LatestGame);