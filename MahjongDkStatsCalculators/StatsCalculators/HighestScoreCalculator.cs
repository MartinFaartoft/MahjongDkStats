namespace MahjongDkStatsCalculators.StatsCalculators;

internal class HighestScoreCalculator : StatisticsCalculatorBase
{
	private Player _highestMcrScore = new Player(string.Empty, int.MinValue, decimal.MinValue, decimal.MinValue);
	private Player _highestRecentMcrScore = new Player(string.Empty, int.MinValue, decimal.MinValue, decimal.MinValue);
	private Player _highestRiichiScore = new Player(string.Empty, int.MinValue, decimal.MinValue, decimal.MinValue);
	private Player _highestRecentRiichiScore = new Player(string.Empty, int.MinValue, decimal.MinValue, decimal.MinValue);

	private Player _highestMcrRating = new Player(string.Empty, int.MinValue, decimal.MinValue, decimal.MinValue);
	private Player _highestRecentMcrRating = new Player(string.Empty, int.MinValue, decimal.MinValue, decimal.MinValue);
	private Player _highestRiichiRating = new Player(string.Empty, int.MinValue, decimal.MinValue, decimal.MinValue);
	private Player _highestRecentRiichiRating = new Player(string.Empty, int.MinValue, decimal.MinValue, decimal.MinValue);

	public override void AppendGame(Game game, Ruleset ruleset)
	{
		var highest = game.Players.MaxBy(p => p.Score)!;
		var highestRating = game.Players.MaxBy(p => p.NewRating)!;

		if (ruleset == Ruleset.Mcr)
		{
			_highestMcrScore = SelectHighestScore(_highestMcrScore, highest);
			_highestMcrRating = SelectHighestRating(_highestMcrRating, highestRating);

			if (game.DateOfGame > Constants.ActiveThreshold)
			{
				_highestRecentMcrScore = SelectHighestScore(_highestRecentMcrScore, highest);
				_highestRecentMcrRating = SelectHighestRating(_highestRecentMcrRating, highestRating);
			}
		}
		else if (ruleset == Ruleset.Riichi)
		{
			_highestRiichiScore = SelectHighestScore(_highestRiichiScore, highest);
			_highestRiichiRating = SelectHighestRating(_highestRiichiRating, highestRating);

			if (game.DateOfGame > Constants.ActiveThreshold)
			{
				_highestRecentRiichiScore = SelectHighestScore(_highestRecentRiichiScore, highest);
				_highestRecentRiichiRating = SelectHighestRating(_highestRecentRiichiRating, highestRating);
			}
		}
	}

	public override IEnumerable<Statistic> GetGlobalMcrStatistics()
	{
		return [
			new Statistic("Highest score ever", $"{_highestMcrScore.Score} - {_highestMcrScore.Name}"),
			new Statistic("Highest score in the last year", $"{_highestRecentMcrScore.Score} - {_highestRecentMcrScore.Name}"),
			new Statistic("Highest rating ever", $"{_highestMcrRating.NewRating} - {_highestMcrRating.Name}"),
			new Statistic("Highest rating in the last year", $"{_highestRecentMcrRating.NewRating} - {_highestRecentMcrRating.Name}")
		];
	}

	public override IEnumerable<Statistic> GetGlobalRiichiStatistics()
	{
		return [
			new Statistic("Highest score ever", $"{_highestRiichiScore.Score} - {_highestRiichiScore.Name}"),
			new Statistic("Highest score in the last year", $"{_highestRecentRiichiScore.Score} - {_highestRecentRiichiScore.Name}"),
			new Statistic("Highest rating ever", $"{_highestRiichiRating.NewRating} - {_highestRiichiRating.Name}"),
			new Statistic("Highest rating in the last year", $"{_highestRecentRiichiRating.NewRating} - {_highestRecentRiichiRating.Name}")
		];
	}

	private Player SelectHighestScore(Player current, Player candidate)
	{
		return candidate.Score > current.Score ? candidate : current;
	}

	private Player SelectHighestRating(Player current, Player candidate)
	{
		return candidate.NewRating > current.NewRating ? candidate : current;
	}
}
