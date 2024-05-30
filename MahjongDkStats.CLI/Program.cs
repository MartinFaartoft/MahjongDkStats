﻿using MahjongDkStatsCalculators;
using MahjongDkStats.CLI.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MahjongDkStats.CLI;
using ScottPlot;

public class Program
{
    const string McrGamesUrl = "https://raw.githubusercontent.com/MartinFaartoft/MahjongDkScraper/main/data/mcr_games_full.json";
    const string RiichiGamesUrl = "https://raw.githubusercontent.com/MartinFaartoft/MahjongDkScraper/main/data/riichi_games_full.json";

    private static async Task Main(string[] args)
    {
        FileSystemHelper.PrepareFoldersAndAssets();

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
        IEnumerable<RatingEntry> mcrRatingList = CreateMcrRatingList(playerStatistics);
		IEnumerable<RatingEntry> riichiRatingList = CreateRiichiRatingList(playerStatistics);

		await RenderHtmlSite(new StatisticsResult(globalStatistics, mcrStatistics, riichiStatistics, playerStatistics, mcrRatingList, riichiRatingList), htmlRenderer);

        Console.WriteLine("SSG Rebuild Complete");
    }

    private static IEnumerable<RatingEntry> CreateMcrRatingList(IEnumerable<PlayerStatistics> playerStatistics)
    {
        return playerStatistics
            .Where(ps => ps.McrStatistics.GameCount > 0)
            .OrderByDescending(ps => ps.McrStatistics.CurrentRating)
            .Select((ps, i) => new RatingEntry(ps.Name, i+1, ps.McrStatistics.CurrentRating, ps.McrStatistics.GameCount))
            .ToArray();
	}

	private static IEnumerable<RatingEntry> CreateRiichiRatingList(IEnumerable<PlayerStatistics> playerStatistics)
	{
		return playerStatistics
			.Where(ps => ps.RiichiStatistics.GameCount > 0)
			.OrderByDescending(ps => ps.RiichiStatistics.CurrentRating)
			.Select((ps, i) => new RatingEntry(ps.Name, i + 1, ps.RiichiStatistics.CurrentRating, ps.RiichiStatistics.GameCount))
			.ToArray();
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

			var mcrRatingPlot = PlotHelper.CreateRatingPlot(player.McrStatistics.Rating, $"MCR Rating - {player.Name}");
            mcrRatingPlot.SavePng($"dist/img/{NameSanitizer.SanitizeForUrlUsage(player.Name)}-mcr-rating.png", 600, 338);

			var riichiRatingPlot = PlotHelper.CreateRatingPlot(player.RiichiStatistics.Rating, $"Riichi Rating - {player.Name}");
			riichiRatingPlot.SavePng($"dist/img/{NameSanitizer.SanitizeForUrlUsage(player.Name)}-riichi-rating.png", 600, 338);
		}
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