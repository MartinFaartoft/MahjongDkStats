namespace MahjongDkStatsCalculators;

public record Player(string Name, int Score, decimal OldRating, decimal NewRating);

public static class PlayerExtensions
{
	public static decimal RatingDelta(this Player p) => p.NewRating - p.OldRating;
}
