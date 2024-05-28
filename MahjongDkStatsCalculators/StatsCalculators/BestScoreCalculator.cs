namespace MahjongDkStatsCalculators.StatsCalculators;

internal class BestScoreCalculator : StatisticsCalculatorBase
{
	private Player _bestMcrScore = new Player(string.Empty, int.MinValue);
	private Player _bestRecentMcrScore = new Player(string.Empty, int.MinValue);
	private Player _bestRiichiScore = new Player(string.Empty, int.MinValue);
	private Player _bestRecentRiichiScore = new Player(string.Empty, int.MinValue);

	private DateOnly RecentIfGameAfter = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1));

	public override void AppendGame(Game game, GameType gameType)
	{
		var best = game.Players.MaxBy(p => p.Score)!;

		if (gameType == GameType.Mcr)
		{
			_bestMcrScore = SelectBest(_bestMcrScore, best);

			if (game.DateOfGame > RecentIfGameAfter)
			{
				_bestRecentMcrScore = SelectBest(_bestRecentMcrScore, best);
			}
		}
		else if (gameType == GameType.Riichi)
		{
			_bestRiichiScore = SelectBest(_bestRiichiScore, best);

			if (game.DateOfGame > RecentIfGameAfter)
			{
				_bestRecentRiichiScore = SelectBest(_bestRecentRiichiScore, best);
			}
		}
	}

	public override IEnumerable<Statistic> GetGlobalMcrStatistics()
	{
		return [
			new Statistic("Best Score", $"{_bestMcrScore.Score} ({_bestMcrScore.Name})"),
			new Statistic("Best Score (last year)", $"{_bestRecentMcrScore.Score} ({_bestRecentMcrScore.Name})")
		];
	}

	public override IEnumerable<Statistic> GetGlobalRiichiStatistics()
	{
		return [
			new Statistic("Best Score", $"{_bestRiichiScore.Score} ({_bestRiichiScore.Name})"),
			new Statistic("Best Score (last year)", $"{_bestRecentRiichiScore.Score} ({_bestRecentRiichiScore.Name})")
		];
	}

	private Player SelectBest(Player current, Player candidate)
	{
		return candidate.Score > current.Score ? candidate : current;
	}
}
