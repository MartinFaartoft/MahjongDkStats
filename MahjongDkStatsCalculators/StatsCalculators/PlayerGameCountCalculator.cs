namespace MahjongDkStatsCalculators.StatsCalculators;

internal class PlayerGameCountCalculator : StatsCalculatorBase
{
    private Dictionary<string, int> GamesPlayed = [];

    public override void AppendGame(Game game, GameType gameType)
    {
        foreach (var player in game.Players)
        {
            if (!GamesPlayed.ContainsKey(player.Name))
            {
                GamesPlayed.Add(player.Name, 0);
            }
            GamesPlayed[player.Name]++;
        }
    }

    public record GameCount(int Count, string Name);
}
