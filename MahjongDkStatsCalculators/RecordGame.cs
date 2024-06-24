namespace MahjongDkStatsCalculators;

public record RecordGame<T>(Game Game, string PlayerName, T RecordValue)
{
	public static RecordGame<T> None(T initialValue)
		=> new RecordGame<T>(Game.None, string.Empty, initialValue);
}