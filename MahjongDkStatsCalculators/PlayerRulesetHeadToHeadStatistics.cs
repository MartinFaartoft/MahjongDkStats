namespace MahjongDkStatsCalculators;

public record PlayerRulesetHeadToHeadStatistics(string OpponentName, int ScoreSumAgainst, decimal ScorePerWindAgainst, int ScoreDeltaAgainst, decimal ScoreDeltaPerWindAgainst, int WindsPlayedAgainst, int GamesPlayedAgainst);