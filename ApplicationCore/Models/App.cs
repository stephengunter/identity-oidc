using Infrastructure.Entities;
using Infrastructure.Helpers;

namespace ApplicationCore.Models;

public class App : EntityBase, IBaseRecord, IRemovable, ISortable
{
   public string Name { get; set; } = String.Empty;
   public string Url { get; set; } = String.Empty;
   public string Icon { get; set; } = String.Empty;
   public string Type { get; set; } = String.Empty;
   public string Roles { get; set; } = String.Empty;
   public string ClientId { get; set; } = String.Empty;
   public string? Encrypt { get; set; }
   public string? Ps { get; set; }

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);

}
