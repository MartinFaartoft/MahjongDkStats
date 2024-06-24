namespace MahjongDkStatsCalculators.Calculators;

internal class RulesetRecordsCalculator
{
	private RecordGame<int> _highestMcrScore = RecordGame<int>.None(int.MinValue);
	private RecordGame<int> _highestRecentMcrScore = RecordGame<int>.None(int.MinValue);
    private RecordGame<int> _highestRiichiScore = RecordGame<int>.None(int.MinValue);
    private RecordGame<int> _highestRecentRiichiScore = RecordGame<int>.None(int.MinValue);

    private RecordGame<decimal> _highestMcrRating = RecordGame<decimal>.None(decimal.MinValue);
    private RecordGame<decimal> _highestRecentMcrRating = RecordGame<decimal>.None(decimal.MinValue);
    private RecordGame<decimal> _highestRiichiRating = RecordGame<decimal>.None(decimal.MinValue);
    private RecordGame<decimal> _highestRecentRiichiRating = RecordGame<decimal>.None(decimal.MinValue);

    internal void AppendGame(Game game, Ruleset ruleset)
	{
		var highest = game.Players.MaxBy(p => p.Score)!;
		var highestRating = game.Players.MaxBy(p => p.NewRating)!;

		if (ruleset == Ruleset.Mcr)
		{
			_highestMcrScore = SelectHighestGame(_highestMcrScore, highest.Name, highest.Score, game);
			_highestMcrRating = SelectHighestGame(_highestMcrRating, highestRating.Name, highestRating.NewRating, game);

			if (game.DateOfGame > Constants.ActiveThreshold)
			{
				_highestRecentMcrScore = SelectHighestGame(_highestRecentMcrScore, highest.Name, highest.Score, game);
                _highestRecentMcrRating = SelectHighestGame(_highestRecentMcrRating, highestRating.Name, highestRating.NewRating, game);
            }
		}
		else if (ruleset == Ruleset.Riichi)
		{
            _highestRiichiScore = SelectHighestGame(_highestRiichiScore, highest.Name, highest.Score, game);
            _highestRiichiRating = SelectHighestGame(_highestRiichiRating, highestRating.Name, highestRating.NewRating, game);

            if (game.DateOfGame > Constants.ActiveThreshold)
            {
                _highestRecentRiichiScore = SelectHighestGame(_highestRecentRiichiScore, highest.Name, highest.Score, game);
                _highestRecentRiichiRating = SelectHighestGame(_highestRecentRiichiRating, highestRating.Name, highestRating.NewRating, game);
            }
        }
	}

	internal RuleSetRecords GetMcrRecords()
	{
		return new RuleSetRecords(_highestMcrScore, _highestRecentMcrScore, _highestMcrRating, _highestRecentMcrRating);
	}

	internal RuleSetRecords GetRiichiRecords()
	{
        return new RuleSetRecords(_highestRiichiScore, _highestRecentRiichiScore, _highestRiichiRating, _highestRecentRiichiRating);
    }

	private RecordGame<T> SelectHighestGame<T>(RecordGame<T> current, string playerName, T value, Game game) where T : IComparable
	{
		return value.CompareTo(current.RecordValue) >= 0 ? new RecordGame<T>(game, playerName, value) : current;
	}
}
