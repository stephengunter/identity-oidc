using Infrastructure.Entities;

namespace Infrastructure.Views;


public interface IBaseCategoryView<T> where T : IBaseCategoryView<T>
{
   T? Parent { get; set; }

   int? ParentId { get; set; }

   bool IsRootItem { get; set; }

   ICollection<T>? SubItems { get; set; }

   ICollection<int>? SubIds { get; set; }
}

