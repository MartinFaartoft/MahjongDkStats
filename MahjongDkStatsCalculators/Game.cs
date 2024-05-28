namespace MahjongDkStatsCalculators;

public record Game(DateOnly DateOfGame, string Id, int NumberOfWinds, decimal Difficulty, IEnumerable<Player> Players);
