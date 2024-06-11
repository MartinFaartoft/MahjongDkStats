using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MahjongDkStats.CLI;

public static class NameSanitizer
{
	public static string SanitizeForUrlUsage(string s)
	{
		var cleaned = RemoveDiacritics(ReplaceNordicChars(s.ToLower()));

		string pattern = @"[^A-Za-z0-9_\-]";
		return Regex.Replace(cleaned, pattern, "-");
	}


	private static string ReplaceNordicChars(string s)
	{
		return s
			.Replace("æ", "ae", StringComparison.OrdinalIgnoreCase)
			.Replace("ø", "oe", StringComparison.OrdinalIgnoreCase)
			.Replace("å", "aa", StringComparison.OrdinalIgnoreCase);
	}

	private static string RemoveDiacritics(string text)
	{
		var normalizedString = text.Normalize(NormalizationForm.FormD);
		var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

		for (int i = 0; i < normalizedString.Length; i++)
		{
			char c = normalizedString[i];
			var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
			if (unicodeCategory != UnicodeCategory.NonSpacingMark)
			{
				stringBuilder.Append(c);
			}
		}

		return stringBuilder
			.ToString()
			.Normalize(NormalizationForm.FormC);
	}
}
