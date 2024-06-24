namespace MahjongDkStatsCalculators;

public record PlayerStatistics(
	string Name,
	int GamesPlayed,
	Game MostRecentGame,
	PlayerRulesetStatistics McrStatistics, 
	PlayerRulesetStatistics RiichiStatistics
	);
