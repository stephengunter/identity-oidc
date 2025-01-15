namespace Infrastructure.Helpers;

public static class ICollectionHelpers
{
	public static void AddIfNotExists<T>(this ICollection<T> list, T item)
	{
      list ??= new List<T>();
      if (!list.Contains(item)) list.Add(item);
   }
   public static void AddRangeIfNotExists<T>(this ICollection<T> list, IEnumerable<T> elements)
   {
      list ??= new List<T>();
      foreach (T element in elements) AddIfNotExists(list, element);
   }
}
