using System.Text.RegularExpressions;

namespace MahjongDkStats.CLI;

public static class NameSanitizer
{
	public static string SanitizeForUrlUsage(string s)
	{
		string pattern = @"[^A-Za-z0-9_\-]";
		return Regex.Replace(s, pattern, string.Empty);
	}
}
