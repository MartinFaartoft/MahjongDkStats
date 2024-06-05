namespace MahjongDkStatsCalculators;

public record Game(DateOnly DateOfGame, string Id, int NumberOfWinds, decimal Difficulty, IEnumerable<Player> Players)
{
	public static Game None
		=> new Game(DateOnly.MinValue, string.Empty, 0, 0, []);
}
