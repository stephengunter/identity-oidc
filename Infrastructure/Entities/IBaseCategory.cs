using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

public interface IBaseCategory<T> where T : IBaseCategory<T>
{
   int Id { get; set; }

   T? Parent { get; set; }

   int? ParentId { get; set; }

	bool IsRootItem { get; }

   ICollection<T>? SubItems { get; set; }

   ICollection<int>? SubIds { get; set; }

   void LoadSubItems(IEnumerable<IBaseCategory<T>> categories);
}


