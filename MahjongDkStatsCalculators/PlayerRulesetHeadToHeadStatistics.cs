namespace MahjongDkStatsCalculators;

public record PlayerRulesetHeadToHeadStatistics(string OpponentName, int ScoreSumAgainst, int ScoreDeltaAgainst, decimal ScorePerWindAgainst, int WindsPlayedAgainst, int GamesPlayedAgainst);