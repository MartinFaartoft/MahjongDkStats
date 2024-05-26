using MahjongDkStatsCalculators;
using System.Text.Json;

var url = "https://raw.githubusercontent.com/MartinFaartoft/MahjongDkScraper/main/data/mcr_games_full.json";
var httpClient = new HttpClient();
var json = await httpClient.GetStringAsync(url);
var games = JsonSerializer.Deserialize<IEnumerable<Game>>(json);
var calc = new GameCountCalculator();

foreach(var game in games)
{
    calc.AppendGame(game);
}

calc.Print();