namespace MahjongDkStatsCalculators.Calculators;

internal class WinningStreakCalculator : PlayerRulesetStatisticCalculatorBase<WinningStreak>
{
	protected override void AddGame(Player player, Game game, WinningStreak ws)
	{
		if (player.Score > 0)
		{
			if (ws.CurrentStreakLength == 0)
			{
				ws.FirstGameInCurrentStreak = game;
			}

			ws.CurrentStreakLength += 1;

			if (ws.CurrentStreakLength > ws.LongestStreakLength)
			{
				ws.FirstGameInLongestStreak = ws.FirstGameInCurrentStreak;
				ws.LongestStreakLength = ws.CurrentStreakLength;
			}
		}
		else
		{
			ws.CurrentStreakLength = 0;
			ws.FirstGameInCurrentStreak = Game.None;
		}
	}

	internal RecordGame<int> GetLongestWinningStreak(string name, Ruleset ruleset)
	{
		var key = new PlayerRulesetKey(name, ruleset);
		return _dict.TryGetValue(key, out WinningStreak? streak)
			? new RecordGame<int>(streak.FirstGameInLongestStreak, name, streak.LongestStreakLength)
			: new RecordGame<int>(Game.None, name, 0);
	}
}

internal class WinningStreak()
{
	public int CurrentStreakLength { get; set; } = 0;

	public int LongestStreakLength { get; set; } = 0;

	public Game FirstGameInCurrentStreak { get; set; } = Game.None;

	public Game FirstGameInLongestStreak { get; set; } = Game.None;
}
