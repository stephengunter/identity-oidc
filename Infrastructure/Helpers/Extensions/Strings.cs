using static System.Net.Mime.MediaTypeNames;

namespace Infrastructure.Helpers;

public static class StringHelpers
{
   public static string GetString(this string? text) => String.IsNullOrEmpty(text) ? "" : text.ToString();
	public static bool HasValue(this string? text) => !String.IsNullOrEmpty(text);
	public static bool EqualTo(this string? val, string other)
	{
		if (String.IsNullOrEmpty(val)) return false;
		return String.Compare(val, other, true) == 0;
   }
	public static bool CaseInsensitiveContains(this string text, string value)
	{
		StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase;
		if (text == null) return false;
		if (value == null) return false;
		return text.IndexOf(value, stringComparison) >= 0;
	}
   public static string ReverseString(this string str)
		=> String.IsNullOrEmpty(str) ? string.Empty : new string(str.ToCharArray().Reverse().ToArray());
}