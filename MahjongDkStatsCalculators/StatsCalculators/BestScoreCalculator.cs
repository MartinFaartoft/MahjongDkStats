namespace MahjongDkStatsCalculators.StatsCalculators;

internal class BestScoreCalculator : StatisticsCalculatorBase
{
	private Player _bestMcrScore = new Player(string.Empty, int.MinValue, decimal.MinValue, decimal.MinValue);
	private Player _bestRecentMcrScore = new Player(string.Empty, int.MinValue, decimal.MinValue, decimal.MinValue);
	private Player _bestRiichiScore = new Player(string.Empty, int.MinValue, decimal.MinValue, decimal.MinValue);
	private Player _bestRecentRiichiScore = new Player(string.Empty, int.MinValue, decimal.MinValue, decimal.MinValue);

	private Player _bestMcrRating = new Player(string.Empty, int.MinValue, decimal.MinValue, decimal.MinValue);
	private Player _bestRecentMcrRating = new Player(string.Empty, int.MinValue, decimal.MinValue, decimal.MinValue);
	private Player _bestRiichiRating = new Player(string.Empty, int.MinValue, decimal.MinValue, decimal.MinValue);
	private Player _bestRecentRiichiRating = new Player(string.Empty, int.MinValue, decimal.MinValue, decimal.MinValue);

	private DateOnly RecentIfGameAfter = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1));

	public override void AppendGame(Game game, GameType gameType)
	{
		var best = game.Players.MaxBy(p => p.Score)!;
		var bestRating = game.Players.MaxBy(p => p.NewRating)!;

		if (gameType == GameType.Mcr)
		{
			_bestMcrScore = SelectBestScore(_bestMcrScore, best);
			_bestMcrRating = SelectBestRating(_bestMcrRating, bestRating);

			if (game.DateOfGame > RecentIfGameAfter)
			{
				_bestRecentMcrScore = SelectBestScore(_bestRecentMcrScore, best);
				_bestRecentMcrRating = SelectBestRating(_bestRecentMcrRating, bestRating);
			}
		}
		else if (gameType == GameType.Riichi)
		{
			_bestRiichiScore = SelectBestScore(_bestRiichiScore, best);
			_bestRiichiRating = SelectBestRating(_bestRiichiRating, bestRating);

			if (game.DateOfGame > RecentIfGameAfter)
			{
				_bestRecentRiichiScore = SelectBestScore(_bestRecentRiichiScore, best);
				_bestRecentRiichiRating = SelectBestRating(_bestRecentRiichiRating, bestRating);
			}
		}
	}

	public override IEnumerable<Statistic> GetGlobalMcrStatistics()
	{
		return [
			new Statistic("Best Score", $"{_bestMcrScore.Score} ({_bestMcrScore.Name})"),
			new Statistic("Best Score (last year)", $"{_bestRecentMcrScore.Score} ({_bestRecentMcrScore.Name})"),
			new Statistic("Best Rating", $"{_bestMcrRating.NewRating} ({_bestMcrRating.Name})"),
			new Statistic("Best Rating (last year)", $"{_bestRecentMcrRating.NewRating} ({_bestRecentMcrRating.Name})")
		];
	}

	public override IEnumerable<Statistic> GetGlobalRiichiStatistics()
	{
		return [
			new Statistic("Best Score", $"{_bestRiichiScore.Score} ({_bestRiichiScore.Name})"),
			new Statistic("Best Score (last year)", $"{_bestRecentRiichiScore.Score} ({_bestRecentRiichiScore.Name})"),
			new Statistic("Best Rating", $"{_bestRiichiRating.NewRating} ({_bestRiichiRating.Name})"),
			new Statistic("Best Rating (last year)", $"{_bestRecentRiichiRating.NewRating} ({_bestRecentRiichiRating.Name})")
		];
	}

	private Player SelectBestScore(Player current, Player candidate)
	{
		return candidate.Score > current.Score ? candidate : current;
	}

	private Player SelectBestRating(Player current, Player candidate)
	{
		return candidate.NewRating > current.NewRating ? candidate : current;
	}
}
