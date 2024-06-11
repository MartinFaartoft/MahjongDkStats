namespace MahjongDkStatsCalculators.Calculators;

internal class PlayerRatingListPositionCalculator
{
	private readonly Dictionary<string, List<PlayerRatingListPositionEntry>> _ratingListPositionHistory = [];

	private readonly Dictionary<string, decimal> _currentPlayerRatings = [];

	private record PlayerRating(string Name, decimal Rating);

	internal void AddGame(Game game)
	{
		foreach(var player in game.Players)
		{
			_currentPlayerRatings[player.Name] = player.NewRating;
		}

		var currentRatingList = _currentPlayerRatings.Select(kv => new PlayerRating(kv.Key, kv.Value)).OrderByDescending(pr => pr.Rating);
		int pos = 1;
		foreach (var pr in currentRatingList)
		{
			if (!_ratingListPositionHistory.TryGetValue(pr.Name, out List<PlayerRatingListPositionEntry>? value))
			{
				value = new List<PlayerRatingListPositionEntry>();
				_ratingListPositionHistory[pr.Name] = value;
			}

			var rlph = value;
			rlph.Add(new PlayerRatingListPositionEntry(game.DateOfGame, pos)); // TODO: if nothing better to do, don't add points between start and end if position unchanged
			pos++;
		}
	}

	internal List<PlayerRatingListPositionEntry> GetRatingListPositionHistory(string name)
	{
		return _ratingListPositionHistory.TryGetValue(name, out List<PlayerRatingListPositionEntry>? value) ? value : [];
	}
}
