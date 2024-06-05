using static MahjongDkStatsCalculators.StatsCalculators.WinningStreakCalculator;

namespace MahjongDkStatsCalculators.StatsCalculators;

internal abstract class PlayerRulesetStatisticCalculatorBase<T> where T : new()
{
	protected readonly Dictionary<PlayerRulesetKey, T> _dict = [];

	protected abstract void AddGame(Player player, Game game, T t);

	public void AddGame(Player player, Ruleset ruleset, Game game)
	{
		var key = new PlayerRulesetKey(player.Name, ruleset);
		if (!_dict.TryGetValue(key, out T? value))
		{
			value = new T();
			_dict[key] = value;
		}

		AddGame(player, game, value);
	}
}

internal record PlayerRulesetKey(string PlayerName, Ruleset Ruleset);