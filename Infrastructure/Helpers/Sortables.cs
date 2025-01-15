using Infrastructure.Entities;
using System.Reflection;

namespace Infrastructure.Helpers
{
   public static class ISortableHelpers
   {
      public static bool IsActive(this ISortable entity) => entity.Order >= 0;
      public static void SetActive(this ISortable entity, bool active)
      {
         if (active)
         {
            if (entity.Order < 0) entity.Order = 0;
         }
         else entity.Order = -1;
      }
   }
}
