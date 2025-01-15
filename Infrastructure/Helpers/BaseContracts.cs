using Infrastructure.Entities;
using System.Data;

namespace Infrastructure.Helpers;

public static class BaseContractHelpers
{
   public static bool IsValid(this IBaseContract entity, bool allowNullStartDate = false, bool allowNullEndDate = true)
   {
      if (!entity.StartDate.HasValue && !allowNullStartDate) return false;
      if (!entity.EndDate.HasValue && !allowNullEndDate) return false;

      return entity.Status != ContractStatus.NA;
   }
   public static bool HasConflict(this IBaseContract entity, IBaseContract other)
   {
      bool allowNullStateDate = false;
      bool allowNullEndDate = false;
      if (!entity.IsValid(allowNullStateDate, allowNullEndDate))
      {
         throw new Exception("entity IsNotValid");
      }
      if (!other.IsValid(allowNullStateDate, allowNullEndDate))
      {
         throw new Exception("other IsNotValid");
      }

      return entity.StartDate < other.EndDate && entity.EndDate > other.StartDate;
   }

   public static ContractStatus GetStatus(this IBaseContract entity)
   {
      if (entity.StartDate.HasValue && entity.EndDate.HasValue && entity.EndDate.Value <= entity.StartDate.Value) return ContractStatus.NA;

      if (!entity.StartDate.HasValue) return ContractStatus.Before;
      if (DateTime.Now > entity.StartDate.Value)
      {
         if (!entity.EndDate.HasValue) return ContractStatus.Active;
         if (DateTime.Now > entity.EndDate.Value) return ContractStatus.Ended;
         return ContractStatus.Active;
      }
      else return ContractStatus.Before; //DateTime.Now <= StartDate.Value
   }

   public static string ToText(this ContractStatus status)
   {
      if (status == ContractStatus.Before) return "未開始";
      if (status == ContractStatus.Active) return "進行中";
      if (status == ContractStatus.Ended) return "已結束";
      else return "";
   }
}
