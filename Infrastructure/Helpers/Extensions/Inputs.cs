using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Infrastructure.Helpers;
public static class InputHelpers
{
   public static IList<string> GetKeywords(this string? input)
	{
		if (String.IsNullOrWhiteSpace(input) || String.IsNullOrEmpty(input)) return new List<string>();

		return input.Split(new string[] { "-", " ", "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
	}

	public static bool IsAlphaNumeric(this string input)
	{
      string pattern = "^[a-zA-Z0-9]*$";
      return Regex.IsMatch(input, pattern);
   }
   public static bool IsValidUserName(this string input)
   {
      string pattern = "^[a-zA-Z0-9_.@]*$";
      return Regex.IsMatch(input, pattern);
   }
   public static bool IsValidEmail(this string input)
   {
      try
      {
         // This will throw an exception if the email is not in a valid format
         var mailAddress = new MailAddress(input);
         return true;
      }
      catch (FormatException)
      {
         // Email is not in a valid format
         return false;
      }
   }
   public static bool IsValidPhoneNumber(this string input)
   {
      // Check if input length is exactly 10
      if (input.Length != 10) return false;
      // Check if the first character is '0'
      if (input[0] != '0') return false;
      // Check if all characters are digits
      foreach (char c in input) if (!char.IsDigit(c)) return false;

      return true;
   }

   public static string FormatNumberWithLeadingZeros(this int num, int len)
   {
      string strNum = num.ToString(); // Convert the number to a string
      string paddedStr = strNum.PadLeft(len, '0'); // Pad with leading zeros to make the length 'len'
      return paddedStr;
   }
   public static int[] ResolveDate(this string input)
   {
      // Trim the input and split the date string by the '/' character
      string[] parts = input.Trim().Split('/');

      // Parse each part of the date into an integer and return as an array
      return new int[]
      {
        int.Parse(parts[0]),  // Year (113)
        int.Parse(parts[1]),  // Month (08 -> 8)
        int.Parse(parts[2])   // Day (01 -> 1)
      };
   }


}