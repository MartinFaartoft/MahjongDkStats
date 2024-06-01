﻿using MahjongDkStatsCalculators;

namespace MahjongDkStats.CLI;

public static class Helpers
{
	public static string CreatePlayerUrl(string name) 
		=> $"{NameSanitizer.SanitizeForUrlUsage(name)}.html";

	public static string RemoveDecimalMarker(decimal value) 
		=> value.ToString().Replace(",", "");

	public static string CreateMcrPlotUrl(PlayerStatistics p) 
		=> $"{NameSanitizer.SanitizeForUrlUsage(p.Name)}-mcr-rating.png";

	public static string CreatePlotUrl(string name, Ruleset ruleset)
	{
		var safeName = NameSanitizer.SanitizeForUrlUsage(name);
		return ruleset switch
		{
			Ruleset.Mcr => $"{safeName}-mcr-rating.png",
			Ruleset.Riichi => $"{safeName}-riichi-rating.png",
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
}