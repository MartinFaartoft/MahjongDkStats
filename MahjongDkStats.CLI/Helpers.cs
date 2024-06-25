using MahjongDkStatsCalculators;

namespace MahjongDkStats.CLI;

public static class Helpers
{
	public static string CreatePlayerUrl(string name) 
		=> $"{NameSanitizer.SanitizeForUrlUsage(name)}.html";

	public static string RemoveDecimalMarker(decimal value) 
		=> Math.Round(value*100).ToString();

	public static string CreateMcrPlotUrl(PlayerStatistics p) 
		=> $"{NameSanitizer.SanitizeForUrlUsage(p.Name)}-mcr-rating.png";

	public static string CreateRatingPlotUrl(string name, Ruleset ruleset)
	{
		var safeName = NameSanitizer.SanitizeForUrlUsage(name);
		return ruleset switch
		{
			Ruleset.Mcr => $"{safeName}-mcr-rating.png",
			Ruleset.Riichi => $"{safeName}-riichi-rating.png",
			_ => throw new NotImplementedException(),
		};
	}

	public static string CreateRatingPositionPlotUrl(string name, Ruleset ruleset)
	{
		var safeName = NameSanitizer.SanitizeForUrlUsage(name);
		return ruleset switch
		{
			Ruleset.Mcr => $"{safeName}-mcr-position.png",
			Ruleset.Riichi => $"{safeName}-riichi-position.png",
			_ => throw new NotImplementedException(),
		};
	}

	public static string CreateRiichiPlotUrl(PlayerStatistics p) 
		=> $"{NameSanitizer.SanitizeForUrlUsage(p.Name)}-riichi-rating.png";

	public static string GetRulesetName(Ruleset ruleset)
		=> ruleset switch
		{
			Ruleset.Riichi => "Riichi",
			Ruleset.Mcr => "MCR",
			_ => throw new NotImplementedException()
		};

	public static string ShortenName(string name)
		=> name.Trim()
		.Split(' ', StringSplitOptions.RemoveEmptyEntries)
		.Select((v, i) => i == 0 ? v : v.First().ToString())
		.Aggregate((a, b) => a + " " + b);
}
