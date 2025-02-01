namespace MahjongDkStatsCalculators;

public record YearStatistics(int Year, int McrGames, int RiichiGames, int McrActivePlayers, PlayerYearWindCount McrMostActivePlayer, int RiichiActivePlayers, PlayerYearWindCount RiichiMostActivePlayer, PlayerYearWindCount TotalMostActivePlayer);
