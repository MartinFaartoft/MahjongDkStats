using MahjongDkStatsCalculators;
using MahjongDkStats.CLI.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MahjongDkStats.CLI;

public class Program
{
    const string McrGamesUrl = "https://raw.githubusercontent.com/MartinFaartoft/MahjongDkScraper/main/data/mcr_games_full.json";
    const string RiichiGamesUrl = "https://raw.githubusercontent.com/MartinFaartoft/MahjongDkScraper/main/data/riichi_games_full.json";

    private static async Task Main(string[] args)
    {
        if (Directory.Exists("dist"))
        {
            Directory.Delete("dist", true);
        }
        Directory.CreateDirectory("dist");
		Directory.CreateDirectory("dist/img");

		IServiceCollection services = new ServiceCollection();
        services.AddLogging();
        services.AddStatsCalculators();

        IServiceProvider serviceProvider = services.BuildServiceProvider();
        ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        await using var htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);

        var statsCalculators = serviceProvider.GetRequiredService<IEnumerable<IStatsCalculator>>();
        
        var gamesLoader = new GamesLoader();
        var mcrGames = await gamesLoader.LoadGamesAsync(McrGamesUrl);
        var riichiGames = await gamesLoader.LoadGamesAsync(RiichiGamesUrl);

        foreach (var game in mcrGames)
        {
            foreach (var calc in statsCalculators)
            {
                calc.AppendGame(game, GameType.Mcr);
            }
        }

        foreach (var game in riichiGames)
        {
            foreach (var calc in statsCalculators)
            {
                calc.AppendGame(game, GameType.Riichi);
            }
        }

        var globalStatistics = statsCalculators.SelectMany(calc => calc.GetGlobalStatistics()).ToList();
        var mcrStatistics = statsCalculators.SelectMany(calc => calc.GetGlobalMcrStatistics()).ToList();
        var riichiStatistics = statsCalculators.SelectMany(calc => calc.GetGlobalRiichiStatistics()).ToList();
        var playerStatistics = statsCalculators.SelectMany(calc => calc.GetPlayerStatistics()).ToList().OrderBy(ps => ps.Name);

		await RenderHtmlSite(new StatisticsResult(globalStatistics, mcrStatistics, riichiStatistics, playerStatistics), htmlRenderer);

        Console.WriteLine("SSG Rebuild Complete");
    }

    private static async Task RenderHtmlSite(StatisticsResult result, HtmlRenderer htmlRenderer)
    {
		Dictionary<string, object?> parameters = new Dictionary<string, object?> { { "Stats", result } };
        var html = await RenderPageToHtml<IndexPage>(parameters, htmlRenderer);
        await File.WriteAllTextAsync("dist/index.html", html);

        foreach (var player in result.PlayerStatistics)
        {
			Dictionary<string, object?> playerParameters = new Dictionary<string, object?> { { "PlayerStats", player } };
			var playerHtml = await RenderPageToHtml<PlayerPage>(playerParameters, htmlRenderer);
			await File.WriteAllTextAsync($"dist/{NameSanitizer.SanitizeForUrlUsage(player.Name)}.html", playerHtml);

			var mcrRatingPlot = CreateRatingPlot(player.McrRating, $"MCR Rating - {player.Name}");
            mcrRatingPlot.SavePng($"dist/img/{NameSanitizer.SanitizeForUrlUsage(player.Name)}-mcr-rating.png", 600, 338);

			var riichiRatingPlot = CreateRatingPlot(player.RiichiRating, $"Riichi Rating - {player.Name}");
			riichiRatingPlot.SavePng($"dist/img/{NameSanitizer.SanitizeForUrlUsage(player.Name)}-riichi-rating.png", 600, 338);
		}
	}

	private static ScottPlot.Plot CreateRatingPlot(DateTimeChart mcrRating, string title)
	{
		ScottPlot.Plot plot = new();
		plot.Add.Scatter(mcrRating.X, mcrRating.Y);
        plot.Title(title);
		plot.FigureBackground.Color = ScottPlot.Color.FromHex("#F9F9F9");
		plot.Axes.DateTimeTicksBottom();

        return plot;
	}

	private static async Task<string> RenderPageToHtml<T>(Dictionary<string, object?> parameters, HtmlRenderer htmlRenderer) where T : IComponent
	{
		var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
		{
			var output = await htmlRenderer.RenderComponentAsync<T>(ParameterView.FromDictionary(parameters));
			return output.ToHtmlString();
		});

        return html;
	}


}