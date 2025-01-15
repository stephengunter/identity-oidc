namespace Infrastructure.Helpers;
public static class CommonHelpers
{
	public static int ToInt(this string str)
	{
		int value = 0;
		if (!int.TryParse(str, out value)) value = 0;

		return value;
	}
   public static double ToDouble(this string str)
   {
      double value;
      if (!Double.TryParse(str, out value)) value = 0;

      return value;
   }
   public static decimal ToDecimal(this string str)
	{
		decimal value;
		if (!Decimal.TryParse(str, out value)) value = 0;

		return value;
	}

	public static bool ToBoolean(this string? str)
	{
		if (String.IsNullOrEmpty(str)) return false;

		return (str.ToLower() == "true");
	}
	public static bool ToBoolean(this int val) => val > 0;

	public static int ToInt(this bool val) => val ? 1 : 0;
	
	public static bool HasDuplicate(this string[] vals) => vals.Length != vals.Distinct().Count();
}
