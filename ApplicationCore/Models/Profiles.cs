using Infrastructure.Entities;
using Infrastructure.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Models;

public class Profiles : IAggregateRoot, IBaseRecord
{
   [Key]
	public string UserId { get; set; } = String.Empty;

   public string Name { get; set; } = String.Empty;

   public string? Ps { get; set; }

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public int? DepartmentId { get; set; }

   [Required]
	public virtual User? User { get; set; }

   public object GetId() => UserId;
}
