using MahjongDkStatsCalculators;
using System.Text.Json;

namespace MahjongDkStats.CLI;

public class GamesLoader
{
    public async Task<IEnumerable<Game>> LoadGamesAsync(string url)
    {
        var httpClient = new HttpClient();
        
        var json = await httpClient.GetStringAsync(url);
        
        return JsonSerializer.Deserialize<IEnumerable<Game>>(json)!.OrderBy(g => g.Id);
    }

    public async Task<IEnumerable<Game>> LoadGamesFromFileAsync(string path)
    {
        var json = await File.ReadAllTextAsync(path);

        return JsonSerializer.Deserialize<IEnumerable<Game>>(json)!.OrderBy(g => g.Id);
    }
}
