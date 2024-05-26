namespace MahjongDkStatsCalculators.StatsCalculators;

internal class BestScoreCalculator : StatsCalculatorBase
{
    private Player _bestMcrScore = new Player(string.Empty, int.MinValue);
    private Player _bestRiichiScore = new Player(string.Empty, int.MinValue);
    
    public override void AppendGame(Game game, GameType gameType)
    {
        var best = game.Players.MaxBy(p => p.Score)!;

        if (gameType == GameType.Mcr)
        {
            _bestMcrScore = SelectBest(_bestMcrScore, best);
        }
        else if (gameType == GameType.Riichi)
        {
            _bestRiichiScore = SelectBest(_bestRiichiScore, best);
        }
    }

    public override IEnumerable<Statistic> GetGlobalMcrStatistics()
    {
        return [ new Statistic("Best Score", $"{_bestMcrScore.Score} ({_bestMcrScore.Name})") ];
    }

    public override IEnumerable<Statistic> GetGlobalRiichiStatistics()
    {
        return [new Statistic("Best Score", $"{_bestRiichiScore.Score} ({_bestRiichiScore.Name})")];
    }

    private Player SelectBest(Player current, Player candidate) {
        return candidate.Score > current.Score ? candidate : current;
    }
}
