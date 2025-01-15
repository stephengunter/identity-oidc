using Infrastructure.Consts;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Views;
using System.Runtime.Serialization;

namespace Infrastructure.Helpers;
public static class BaseRecordHelpers
{
   public static void SetCreated(this IBaseRecord entity, string createdBy)
   {
      entity.CreatedAt = DateTime.Now;
      entity.CreatedBy = createdBy;
   }
   public static void SetUpdated(this IBaseRecord entity, string updatedBy)
   {
      entity.LastUpdated = DateTime.Now;
      entity.UpdatedBy = updatedBy;
   }
}
