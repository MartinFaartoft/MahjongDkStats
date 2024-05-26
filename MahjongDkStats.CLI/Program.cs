using MahjongDkStatsCalculators;
using MahjongDkStats.CLI.Pages;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

IServiceCollection services = new ServiceCollection();
services.AddLogging();

IServiceProvider serviceProvider = services.BuildServiceProvider();
ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

await using var htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);

var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
{
    var dictionary = new Dictionary<string, object?>
    {
        { "Message", "Hello from the Render Message component!" }
    };

    var parameters = ParameterView.FromDictionary(dictionary);
    var output = await htmlRenderer.RenderComponentAsync<IndexPage>(parameters);

    return output.ToHtmlString();
});

await File.WriteAllTextAsync("dist/index.html", html);


var url = "https://raw.githubusercontent.com/MartinFaartoft/MahjongDkScraper/main/data/mcr_games_full.json";
var httpClient = new HttpClient();
var json = await httpClient.GetStringAsync(url);
var games = JsonSerializer.Deserialize<IEnumerable<Game>>(json);
var calc = new GameCountCalculator();

foreach(var game in games)
{
    calc.AppendGame(game);
}

//calc.Print();