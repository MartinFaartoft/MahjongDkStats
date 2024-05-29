namespace MahjongDkStatsCalculators;

public record PlayerStatistics(string Name, IEnumerable<Statistic> Statistics, DateTimeChart McrRating, DateTimeChart RiichiRating);

public record DateTimeChart(DateTime[] X, double[] Y);