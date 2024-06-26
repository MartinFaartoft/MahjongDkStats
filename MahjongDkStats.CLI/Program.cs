using MahjongDkStatsCalculators;
using MahjongDkStats.CLI.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MahjongDkStats.CLI;
using System.Globalization;
using System.Diagnostics;

public class Program
{
    const string McrGamesUrl = "https://raw.githubusercontent.com/MartinFaartoft/MahjongDkScraper/main/data/mcr_games_full.json";
    const string RiichiGamesUrl = "https://raw.githubusercontent.com/MartinFaartoft/MahjongDkScraper/main/data/riichi_games_full.json";
    const string LocalMcrGamesUrl = "../MahjongDkScraper/data/mcr_games_full.json";
    const string LocalRiichiGamesUrl = "../MahjongDkScraper/data/riichi_games_full.json";
    const string MembersFilePath = "mjdk-members.txt";
    const bool _generatePlots = true;
    const bool _useLocalGamesData = false;

    private static async Task Main(string[] args)
    {
        FileSystemHelper.PrepareFoldersAndAssets();
		CultureInfo.CurrentCulture = new CultureInfo("da-DK", false);
		IServiceCollection services = new ServiceCollection();
        services.AddLogging();
        services.AddStatsCalculators();

        IServiceProvider serviceProvider = services.BuildServiceProvider();
        ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        await using var htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);

        var statsCalculator = serviceProvider.GetRequiredService<IStatsCalculator>();
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var gamesLoader = new GamesLoader();
        
        var mcrGames = await (_useLocalGamesData ? gamesLoader.LoadGamesFromFileAsync(LocalMcrGamesUrl) : gamesLoader.LoadGamesAsync(McrGamesUrl));
        var riichiGames = await (_useLocalGamesData ? gamesLoader.LoadGamesFromFileAsync(LocalRiichiGamesUrl) : gamesLoader.LoadGamesAsync(RiichiGamesUrl));
        var members = await File.ReadAllLinesAsync(MembersFilePath);
        var membersLookup = new HashSet<string>(members);
        Console.WriteLine($"Loaded game data in {stopwatch.ElapsedMilliseconds}ms");
        stopwatch.Restart();

        foreach (var game in mcrGames)
        {
            statsCalculator.AppendGame(game, Ruleset.Mcr);
        }

        foreach (var game in riichiGames)
        {
            statsCalculator.AppendGame(game, Ruleset.Riichi);
        }

        var globalStatistics = statsCalculator.GetGlobalStatistics();
        var mcrRecords = statsCalculator.GetMcrRecords();
        var riichiRecords = statsCalculator.GetRiichiRecords();
        var playerStatistics = statsCalculator.GetPlayerStatistics().OrderBy(ps => ps.Name).ToArray();
        var memberStatistics = playerStatistics.Where(p => membersLookup.Contains(p.Name)).ToArray();

		EnsureMemberNamesMatch(memberStatistics, membersLookup);
        
        RatingEntry[] mcrRatingList = CreateMcrRatingList(playerStatistics);
		RatingEntry[] riichiRatingList = CreateRiichiRatingList(playerStatistics);
        var newestGameDate = mcrGames.Concat(riichiGames).MaxBy(g => g.DateOfGame)!.DateOfGame;
		Console.WriteLine($"Calculated statistics in {stopwatch.ElapsedMilliseconds}ms");
		stopwatch.Restart();


		await RenderHtmlSite(new StatisticsResult(globalStatistics, mcrRecords, riichiRecords, playerStatistics, memberStatistics, mcrRatingList, riichiRatingList), newestGameDate, htmlRenderer);
		Console.WriteLine($"Built static site in {stopwatch.ElapsedMilliseconds}ms");
    }

	private static void EnsureMemberNamesMatch(PlayerStatistics[] memberStatistics, HashSet<string> membersLookup)
	{
        foreach (var member in memberStatistics)
        {
            if(!membersLookup.Contains(member.Name))
            {
                Console.WriteLine("Missing member: {}", member.Name);
            }
        }
	}

	private static RatingEntry[] CreateMcrRatingList(IEnumerable<PlayerStatistics> playerStatistics)
    {
        return playerStatistics
            .Where(ps => ps.McrStatistics.GameCount > 0 && ps.McrStatistics.LatestGame > Constants.ActiveThreshold)
            .OrderByDescending(ps => ps.McrStatistics.CurrentRating)
            .Select((ps, i) => new RatingEntry(ps.Name, i+1, ps.McrStatistics.CurrentRating, ps.McrStatistics.GameCount))
            .ToArray();
	}

	private static RatingEntry[] CreateRiichiRatingList(IEnumerable<PlayerStatistics> playerStatistics)
	{
		return playerStatistics
			.Where(ps => ps.RiichiStatistics.GameCount > 0 && ps.RiichiStatistics.LatestGame > Constants.ActiveThreshold)
			.OrderByDescending(ps => ps.RiichiStatistics.CurrentRating)
			.Select((ps, i) => new RatingEntry(ps.Name, i + 1, ps.RiichiStatistics.CurrentRating, ps.RiichiStatistics.GameCount))
			.ToArray();
	}

    private const int _plotWidth = 1000;
    private const int _plotHeight = 563;

	private static async Task RenderHtmlSite(StatisticsResult result, DateOnly newestGameDate, HtmlRenderer htmlRenderer)
    {
		Dictionary<string, object?> parameters = new Dictionary<string, object?> { { "Stats", result }, { "NewestGameDate", newestGameDate } };
        var html = await RenderPageToHtml<IndexPage>(parameters, htmlRenderer);
        await File.WriteAllTextAsync("dist/index.html", html);

        var aboutHtml = await RenderPageToHtml<AboutPage>([], htmlRenderer);
        await File.WriteAllTextAsync("dist/about.html", aboutHtml);


        var tasks = new List<Task>();

        foreach (var player in result.PlayerStatistics)
        {
			Dictionary<string, object?> playerParameters = new Dictionary<string, object?> { { "PlayerStats", player } };
            tasks.Add(
                RenderPageToHtml<PlayerPage>(playerParameters, htmlRenderer)
                .ContinueWith(a => File.WriteAllTextAsync($"dist/{NameSanitizer.SanitizeForUrlUsage(player.Name)}.html", a.Result))
                );
            if (_generatePlots)
            {
                tasks.Add(Task.Run(()
                    => PlotHelper.CreateDateTimePlot(player.McrStatistics.Rating, $"MCR rating - {player.Name}")
                    .SavePng($"dist/img/{NameSanitizer.SanitizeForUrlUsage(player.Name)}-mcr-rating.png", _plotWidth, _plotHeight)));
                tasks.Add(Task.Run(()
                    => PlotHelper.CreatingInvertedYDateTimePlot(player.McrStatistics.RatingListPosition, $"MCR ratinglist position - {player.Name}")
                    .SavePng($"dist/img/{NameSanitizer.SanitizeForUrlUsage(player.Name)}-mcr-position.png", _plotWidth, _plotHeight)));
                tasks.Add(Task.Run(()
                    => PlotHelper.CreateDateTimePlot(player.RiichiStatistics.Rating, $"Riichi rating - {player.Name}")
                    .SavePng($"dist/img/{NameSanitizer.SanitizeForUrlUsage(player.Name)}-riichi-rating.png", _plotWidth, _plotHeight)));
                tasks.Add(Task.Run(()
                    => PlotHelper.CreatingInvertedYDateTimePlot(player.RiichiStatistics.RatingListPosition, $"Riichi ratinglist position - {player.Name}")
                    .SavePng($"dist/img/{NameSanitizer.SanitizeForUrlUsage(player.Name)}-riichi-position.png", _plotWidth, _plotHeight)));
            }
		}

        Task.WaitAll(tasks.ToArray());
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