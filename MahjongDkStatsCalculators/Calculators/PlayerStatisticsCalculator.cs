﻿namespace MahjongDkStatsCalculators.Calculators;

internal class PlayerStatisticsCalculator : StatisticsCalculatorBase
{
	private readonly Dictionary<string, PlayerStats> Players = [];
	private readonly PlayerRatingListPositionCalculator _mcrRatingListPositionCalculator = new();
	private readonly PlayerRatingListPositionCalculator _riichiRatingListPositionCalculator = new();
	private readonly WinningStreakCalculator _winningStreakCalculator = new();

	public override void AppendGame(Game game, Ruleset ruleset)
	{
		foreach(var player in game.Players)
		{
			UpdatePlayer(player, game, ruleset);
		}

		if (ruleset == Ruleset.Mcr)
		{
			_mcrRatingListPositionCalculator.AddGame(game);
		}
		else
		{
			_riichiRatingListPositionCalculator.AddGame(game);
		}
	}

	public override IEnumerable<PlayerStatistics> GetPlayerStatistics()
	{
		return Players
			.Select(kv => new PlayerStatistics(
				kv.Key,
				[
					new Statistic("Games played", kv.Value.GameCount.ToString()),
					new Statistic("Most recent game", kv.Value.LatestGame.ToString("yyyy-MM-dd")),
				],
				GetPlayerRulesetStatistics(kv.Value, Ruleset.Mcr),
				GetPlayerRulesetStatistics(kv.Value, Ruleset.Riichi)));
	}

	private void UpdatePlayer(Player player, Game game, Ruleset ruleset)
	{
		if (!Players.ContainsKey(player.Name))
		{
			Players[player.Name] = new PlayerStats(player.Name);
		}

		var stats = Players[player.Name];
		var rulesetStats = ruleset == Ruleset.Mcr ? stats.McrStats : stats.RiichiStats;

		stats.GameCount++;
		stats.LatestGame = stats.LatestGame > game.DateOfGame ? stats.LatestGame : game.DateOfGame;

		UpdatePlayerRulesetStats(player, rulesetStats, ruleset, game);
		UpdatePlayerRulesetHeadToHeadStats(player, rulesetStats, game);
	}

	private void UpdatePlayerRulesetStats(Player player, PlayerRulesetStats stats, Ruleset ruleset, Game game)
	{
		stats.Rating[game.DateOfGame] = player.NewRating; // This overwrites previous games played on same day, potentially hiding peaks and troughs in the graph
		stats.MaxRating = stats.MaxRating > player.NewRating ? stats.MaxRating : player.NewRating;
		stats.GameCount++;
		stats.WindCount += game.NumberOfWinds;
		stats.ScoreSum += player.Score;
		_winningStreakCalculator.AddGame(player, ruleset, game);
		stats.RecordGameScore = player.Score > stats.RecordGameScore.RecordValue
			? new RecordGame<int>(game, player.Name, player.Score)
			: stats.RecordGameScore;
		stats.GameHistory.Add(game);	
	}

	private void UpdatePlayerRulesetHeadToHeadStats(Player player, PlayerRulesetStats stats, Game game)
	{
		var h = stats.HeadToHeadStats;

		foreach (var opponent in game.Players.Where(p => p.Name != player.Name))
		{
			if (!h.ContainsKey(opponent.Name))
			{
				h[opponent.Name] = new PlayerRulesetHeadToHeadStats { OpponentName = opponent.Name };
			}

			var s = h[opponent.Name];

			s.ScoreSumAgainst += player.Score;
			s.OpponentScoreSum += opponent.Score;
			s.ScoreDeltaAgainst = s.ScoreSumAgainst - s.OpponentScoreSum;
			s.WindsPlayedAgainst += game.NumberOfWinds;
			s.GamesPlayedAgainst += 1;
		}
	}

	private PlayerRulesetStatistics GetPlayerRulesetStatistics(PlayerStats stats, Ruleset ruleset)
	{
		var rulesetStats = ruleset == Ruleset.Mcr ? stats.McrStats : stats.RiichiStats;
		var rating = GetPlayerRatingHistory(rulesetStats.Rating);
		var ratingListPositionHistory = ruleset == Ruleset.Mcr
			? _mcrRatingListPositionCalculator.GetRatingListPositionHistory(stats.Name)
			: _riichiRatingListPositionCalculator.GetRatingListPositionHistory(stats.Name);
		var ratingListPosition = GetPlayerRatingListPositionChart(ratingListPositionHistory);
		var headToHeadStatistics = rulesetStats.HeadToHeadStats.Values
			.Select(h => new PlayerRulesetHeadToHeadStatistics(h.OpponentName, h.ScoreSumAgainst, h.ScoreDeltaAgainst, Math.Round(h.ScoreSumAgainst / (decimal)h.WindsPlayedAgainst, 2), h.WindsPlayedAgainst, h.GamesPlayedAgainst))
			.Where(h => h.WindsPlayedAgainst >= 25)
			.OrderByDescending(h => h.ScoreSumAgainst)
			.ToArray();
		var latestGame = rulesetStats.GameHistory.LastOrDefault();
		var currentRating = latestGame?.Players.First(p => p.Name == stats.Name).NewRating ?? 0;
		var scorePerWind = Math.Round(rulesetStats.WindCount > 0 ? (decimal)rulesetStats.ScoreSum / rulesetStats.WindCount : 0, 2);
		return new PlayerRulesetStatistics(
					ruleset,
					rating,
					ratingListPosition,
					rulesetStats.GameCount,
					rulesetStats.MaxRating,
					currentRating,
					latestGame?.DateOfGame ?? DateOnly.MinValue,
					rulesetStats.ScoreSum,
					_winningStreakCalculator.GetLongestWinningStreak(stats.Name, ruleset),
					rulesetStats.RecordGameScore,
					scorePerWind,
					headToHeadStatistics,
					rulesetStats.GameHistory,
					ratingListPositionHistory
					);
	}

	private DateTimeChart GetPlayerRatingHistory(Dictionary<DateOnly, decimal> rating)
	{
		EnsureRatingHistoryStartsWithZero(rating);

		var dates = rating.Keys.Order().ToArray();
		var values = dates.Select(d => rating[d]);

		return new DateTimeChart(
			dates.Select(d => d.ToDateTime(TimeOnly.MinValue)).ToArray(),
			values.Select(r => (double)r).ToArray());
	}

	private DateTimeChart GetPlayerRatingListPositionChart(List<PlayerRatingListPositionEntry> ratingListPositionHistory)
	{
		return new DateTimeChart(
			ratingListPositionHistory.Select(e => e.StartDate.ToDateTime(TimeOnly.MinValue)).ToArray(),
			ratingListPositionHistory.Select(e => (double)e.RatingListPosition).ToArray());
	}

	private void EnsureRatingHistoryStartsWithZero(Dictionary<DateOnly, decimal> rating)
	{
        if (rating.Count == 0)
        {
			return;
        }

        var minDate = rating.Keys.Min();
		rating[minDate.AddDays(-1)] = 0M;
	}

	internal class PlayerStats(string name)
	{
		public string Name { get; set; } = name;

        public int GameCount { get; set; }

		public DateOnly LatestGame { get; set; } = DateOnly.MinValue;

        public PlayerRulesetStats McrStats { get; set; } = new PlayerRulesetStats();

		public PlayerRulesetStats RiichiStats { get; set; } = new PlayerRulesetStats();
    }

	internal class PlayerRulesetStats()
	{
		public int GameCount { get; set; }

		public int WindCount { get; set; }

		public int ScoreSum { get; set; }

        public decimal MaxRating { get; set; } = decimal.MinValue;

		public int CurrentWinningStreak { get; set; }

		public int LongestWinningStreak { get; set; }

		public RecordGame<int> RecordGameScore { get; set; } = RecordGame<int>.None(int.MinValue);

		public Dictionary<DateOnly, decimal> Rating { get; set; } = [];

		public Dictionary<string, PlayerRulesetHeadToHeadStats> HeadToHeadStats { get; set; } = [];

        public List<Game> GameHistory { get; } = [];
	}

	internal class PlayerRulesetHeadToHeadStats()
	{
		public string OpponentName { get; set; } = string.Empty;

        public int ScoreSumAgainst { get; set; }

		public int OpponentScoreSum { get; set; }
		
		public int ScoreDeltaAgainst { get; set; }

		public int GamesPlayedAgainst { get; set; }

        public int WindsPlayedAgainst { get; set; }
    }
}
