namespace MahjongDkStatsCalculators;

public record Game(DateOnly DateOfGame, string Id, int NumberOfWinds, IEnumerable<Player> Players);

public record Player(string Name, int Score);