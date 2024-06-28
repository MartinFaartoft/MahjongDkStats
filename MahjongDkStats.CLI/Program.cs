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

        var (mcrGames, riichiGames, membersLookup) = await LoadData();
        Console.WriteLine($"Loaded game data in {stopwatch.ElapsedMilliseconds}ms");
        stopwatch.Restart();

        CalculateStatistics(statsCalculator, mcrGames, riichiGames);
        Console.WriteLine($"Calculated statistics in {stopwatch.ElapsedMilliseconds}ms");
        stopwatch.Restart();
        
        var res = PrepareStatistics(statsCalculator, mcrGames, riichiGames, membersLookup);

        FileSystemHelper.PrepareFoldersAndAssets();
        await RenderSite(res, htmlRenderer);
        Console.WriteLine($"Rendered site in {stopwatch.ElapsedMilliseconds}ms");
    }

    private static async Task<(IEnumerable<Game> mcrGames, IEnumerable<Game> riichiGames, HashSet<string> membersLookup)> LoadData()
    {
        var gamesLoader = new GamesLoader();

        var mcrGamesTask = _useLocalGamesData ? gamesLoader.LoadGamesFromFileAsync(LocalMcrGamesUrl) : gamesLoader.LoadGamesAsync(McrGamesUrl);
        var riichiGamesTask = _useLocalGamesData ? gamesLoader.LoadGamesFromFileAsync(LocalRiichiGamesUrl) : gamesLoader.LoadGamesAsync(RiichiGamesUrl);
        var membersTask = File.ReadAllLinesAsync(MembersFilePath);

        await Task.WhenAll(mcrGamesTask, riichiGamesTask, membersTask);

        var membersLookup = new HashSet<string>(membersTask.Result);

        return (mcrGamesTask.Result, riichiGamesTask.Result, membersLookup);
    }

    private static StatisticsResult PrepareStatistics(IStatsCalculator statsCalculator, IEnumerable<Game> mcrGames, IEnumerable<Game> riichiGames, HashSet<string> membersLookup)
    {
        var playerStatistics = statsCalculator.GetPlayerStatistics().OrderBy(ps => ps.Name).ToArray();
        var memberStatistics = playerStatistics.Where(p => membersLookup.Contains(p.Name)).ToArray();
        var activeMemberStatistics = memberStatistics.Where(p => p.IsActive).ToArray();

        EnsureMemberNamesMatch(memberStatistics, membersLookup);

        RatingEntry[] mcrRatingList = CreateMcrRatingList(activeMemberStatistics);
        RatingEntry[] riichiRatingList = CreateRiichiRatingList(activeMemberStatistics);
        var newestGameDate = mcrGames.Concat(riichiGames).MaxBy(g => g.DateOfGame)!.DateOfGame;


        return new StatisticsResult(
            statsCalculator.GetGlobalStatistics(),
            statsCalculator.GetMcrRecords(),
            statsCalculator.GetRiichiRecords(),
            playerStatistics,
            memberStatistics,
            mcrRatingList,
            riichiRatingList,
            statsCalculator.GetYearStatistics().ToArray(),
            newestGameDate);
    }

    private static void CalculateStatistics(IStatsCalculator statsCalculator, IEnumerable<Game> mcrGames, IEnumerable<Game> riichiGames)
    {
        foreach (var game in mcrGames)
        {
            statsCalculator.AppendGame(game, Ruleset.Mcr);
        }

        foreach (var game in riichiGames)
        {
            statsCalculator.AppendGame(game, Ruleset.Riichi);
        }
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
            .Select((ps, i) => new RatingEntry(ps.Name, i+1, ps.McrStatistics.CurrentRating, ps.McrStatistics.ScorePerWind, ps.McrStatistics.GameCount, ps.McrStatistics.WindCount))
            .ToArray();
	}

	private static RatingEntry[] CreateRiichiRatingList(IEnumerable<PlayerStatistics> playerStatistics)
	{
		return playerStatistics
			.Where(ps => ps.RiichiStatistics.GameCount > 0 && ps.RiichiStatistics.LatestGame > Constants.ActiveThreshold)
			.OrderByDescending(ps => ps.RiichiStatistics.CurrentRating)
			.Select((ps, i) => new RatingEntry(ps.Name, i + 1, ps.RiichiStatistics.CurrentRating, ps.RiichiStatistics.ScorePerWind, ps.RiichiStatistics.GameCount, ps.RiichiStatistics.WindCount))
			.ToArray();
	}

    private const int _plotWidth = 1000;
    private const int _plotHeight = 563;

	private static async Task RenderSite(StatisticsResult result, HtmlRenderer htmlRenderer)
    {
		Dictionary<string, object?> parameters = new Dictionary<string, object?> { { "Stats", result } };
        var html = await RenderPageToHtml<IndexPage>(parameters, htmlRenderer);
        await File.WriteAllTextAsync($"{FileSystemHelper.OutputFolderName}/index.html", html);

        var aboutHtml = await RenderPageToHtml<AboutPage>([], htmlRenderer);
        await File.WriteAllTextAsync($"{FileSystemHelper.OutputFolderName}/about.html", aboutHtml);


        var tasks = new List<Task>();


        tasks.Add(Task.Run(()
        => PlotHelper.CreateGamesPerYearByRulesetPlot(result.YearStatistics)
                    .SavePng($"{FileSystemHelper.ImagesPath}/games-per-ruleset-plot.png", _plotWidth, _plotHeight)));

        tasks.Add(Task.Run(()
        => PlotHelper.CreateActivePlayersPerYearByRulesetPlot(result.YearStatistics)
                    .SavePng($"{FileSystemHelper.ImagesPath}/players-per-ruleset-plot.png", _plotWidth, _plotHeight)));

        foreach (var player in result.PlayerStatistics)
        {
			Dictionary<string, object?> playerParameters = new Dictionary<string, object?> { { "PlayerStats", player } };
            tasks.Add(
                RenderPageToHtml<PlayerPage>(playerParameters, htmlRenderer)
                .ContinueWith(a => File.WriteAllTextAsync($"{FileSystemHelper.OutputFolderName}/{NameSanitizer.SanitizeForUrlUsage(player.Name)}.html", a.Result))
                );
            if (_generatePlots)
            {
                tasks.Add(Task.Run(()
                    => PlotHelper.CreateDateTimePlot(player.McrStatistics.Rating, $"MCR rating - {player.Name}")
                    .SavePng($"{FileSystemHelper.ImagesPath}/{NameSanitizer.SanitizeForUrlUsage(player.Name)}-mcr-rating.png", _plotWidth, _plotHeight)));
                tasks.Add(Task.Run(()
                    => PlotHelper.CreateInvertedYDateTimePlot(player.McrStatistics.RatingListPosition, $"MCR ratinglist position - {player.Name}")
                    .SavePng($"{FileSystemHelper.ImagesPath}/{NameSanitizer.SanitizeForUrlUsage(player.Name)}-mcr-position.png", _plotWidth, _plotHeight)));
                tasks.Add(Task.Run(()
                    => PlotHelper.CreateDateTimePlot(player.RiichiStatistics.Rating, $"Riichi rating - {player.Name}")
                    .SavePng($"{FileSystemHelper.ImagesPath}/{NameSanitizer.SanitizeForUrlUsage(player.Name)}-riichi-rating.png", _plotWidth, _plotHeight)));
                tasks.Add(Task.Run(()
                    => PlotHelper.CreateInvertedYDateTimePlot(player.RiichiStatistics.RatingListPosition, $"Riichi ratinglist position - {player.Name}")
                    .SavePng($"{FileSystemHelper.ImagesPath}/{NameSanitizer.SanitizeForUrlUsage(player.Name)}-riichi-position.png", _plotWidth, _plotHeight)));
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