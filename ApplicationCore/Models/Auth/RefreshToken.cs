using Infrastructure.Entities;
using Infrastructure.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models.Auth;
[Table("Auth.RefreshToken")]
public class RefreshToken : IAggregateRoot
{
   [Key]
   public string UserId { get; set; } = string.Empty;
   public string Token { get; set; } = string.Empty;
   public DateTime LastUpdated { get; set; } = DateTime.Now;
   public DateTime Expires { get; set; }

   public string? RemoteIpAddress { get; set; }

   [Required]
   public virtual User? User { get; set; }

   public bool Active => DateTime.UtcNow <= Expires;

   public object GetId() => UserId;
}
