namespace MahjongDkStatsCalculators;

public class GameCountCalculator : IStatsCalculator
{
    private Dictionary<string, int> GamesPlayed = [];

    public void AppendGame(Game game)
    {
        foreach(var player in game.Players)
        {
            if (!GamesPlayed.ContainsKey(player.Name))
            {
                GamesPlayed.Add(player.Name, 0);
            }
            GamesPlayed[player.Name]++;
        }
    }

    public void Print()
    {

        var sortedList = GamesPlayed.Select(kv => new GameCount(kv.Value, kv.Key)).OrderByDescending(gc => gc.Count);
        Console.WriteLine("Most games played");
        Console.WriteLine("-----------------");
        foreach(var gc in sortedList)
        {
            Console.WriteLine(gc.Name + " - " + gc.Count);
        }

    }

    public record GameCount(int Count, string Name);
}
