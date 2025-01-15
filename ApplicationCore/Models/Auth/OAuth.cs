using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;
using ApplicationCore.Consts;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models.Auth;
[Table("Auth.OAuth")]
public class OAuth : EntityBase
{
   public string UserId { get; set; } = string.Empty;

   public string? Name { get; set; }

   public string? FamilyName { get; set; }

   public string? GivenName { get; set; }

   public string? OAuthId { get; set; }

   public OAuthProvider Provider { get; set; }

   public string? PictureUrl { get; set; }

   [Required]
   public virtual User? User { get; set; }
}
