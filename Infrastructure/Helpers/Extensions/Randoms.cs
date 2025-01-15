using System.Text;

namespace Infrastructure.Helpers;

public static class RandomHelper
{
   private static readonly char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@#%&!".ToCharArray();

   private static readonly Random _random = new Random(DateTime.Now.Millisecond);

   public static T GetRandomEnumValue<T>() where T : Enum
   {
      Array values = Enum.GetValues(typeof(T));
      return (T)values.GetValue(_random.Next(values.Length));
   }

   public static T GetRandomItem<T>(this IList<T> list) => list[_random.Next(0, list.Count)];

	public static IList<T> Shuffle<T>(this IList<T> inputList, int take = 0)
	{
		var randomList = new List<T>();

		Random r = new Random();
		int randomIndex = 0;
		while (inputList.Count > 0)
		{
			randomIndex = r.Next(0, inputList.Count); //Choose a random object in the list
			randomList.Add(inputList[randomIndex]); //add it to the new, random list
			inputList.RemoveAt(randomIndex); //remove to avoid duplicates
		}

		if(take < 1) return randomList;

		return randomList.Take(take).ToList(); //return the new random list
	}

   public static string GenerateRandomString(int length)
   {
      var stringBuilder = new StringBuilder();
      var random = new Random();

      for (int i = 0; i < length; i++)
      {
         stringBuilder.Append(chars[random.Next(chars.Length)]);
      }

      return stringBuilder.ToString();
   }
}
