using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;
using ApplicationCore.Consts;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models.Auth;
[Table("Auth.Token")]
public class AuthToken : EntityBase
{
   public AuthProvider Provider { get; set; }
   public string Token { get; set; } = String.Empty;
   public DateTime LastUpdated { get; set; } = DateTime.Now;
   public DateTime Expires { get; set; }

   public string? RemoteIpAddress { get; set; }
   public string UserName { get; set; } = String.Empty;
   public string AdListJson { get; set; } = String.Empty;

   public bool Active => DateTime.UtcNow <= Expires;
}