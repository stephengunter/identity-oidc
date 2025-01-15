using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;

public static class BaseCategoriesHelpers
{
   public static void LoadSubItems<T>(this IBaseCategory<T> entity, IEnumerable<IBaseCategory<T>> categories) where T : IBaseCategory<T>
   {
      entity.SubItems = categories.Where(item => item.ParentId == entity.Id).Select(item => (T)item).ToList();
      if(entity.SubItems.HasItems()) entity.SubIds!.AddRangeIfNotExists(entity.SubItems.Select(c => c.Id));
      foreach (var item in entity.SubItems) item.LoadSubItems(categories);
   }

}
