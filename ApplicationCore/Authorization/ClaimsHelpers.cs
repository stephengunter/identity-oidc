using Infrastructure.Helpers;
using System.Security.Claims;
using System.Security.Principal;
using ApplicationCore.Models;
using ApplicationCore.Consts;
using System.Data;


namespace ApplicationCore.Authorization;
public static class ClaimsHelpers
{
   public static string UserName(this ClaimsPrincipal user)
   {
      var claim = user.FindFirst(ClaimTypes.NameIdentifier);
      if (claim != null) return claim.Value;
      return user.Claims.Find(JwtClaimIdentifiers.Sub)?.Value ?? string.Empty;
   }

   public static string Id(this ClaimsPrincipal user)
   {
      var claim = user.FindFirst(JwtClaimIdentifiers.Id);
      if (claim != null) return claim.Value;
      return user.Claims.Find(JwtClaimIdentifiers.Id)?.Value ?? string.Empty;
   }

   public static IEnumerable<string> Roles(this ClaimsPrincipal user)
   {
      var claim = user.FindFirst(ClaimTypes.Role);
      if (claim != null) return claim.Value.SplitToList();
      return user.Claims.Find(JwtClaimIdentifiers.Roles)?.Value.SplitToList() ?? new List<string>();
   }

   public static bool IsDev(this ClaimsPrincipal user)
   {
      if (Roles(user).IsNullOrEmpty()) return false;
      var dev = Roles(user).FirstOrDefault(r => r.EqualTo(AppRoles.Dev.ToString()));
      return dev != null;
   }
   public static bool IsBoss(this ClaimsPrincipal user)
   {
      if (Roles(user).IsNullOrEmpty()) return false;
      var boss = Roles(user).FirstOrDefault(r => r.EqualTo(AppRoles.Boss.ToString()));
      return boss != null;
   }
   public static bool IsIT(this ClaimsPrincipal user)
   {
      if (Roles(user).IsNullOrEmpty()) return false;
      var it = Roles(user).FirstOrDefault(r => r.EqualTo(AppRoles.IT.ToString()));
      return it != null;
   }
   public static bool IsClerk(this ClaimsPrincipal user)
   {
      if (Roles(user).IsNullOrEmpty()) return false;
      var clerk = Roles(user).FirstOrDefault(r => r.EqualTo(AppRoles.Clerk.ToString()));
      return clerk != null;
   }
   public static bool IsRecorder(this ClaimsPrincipal user)
   {
      if (Roles(user).IsNullOrEmpty()) return false;
      var recorder = Roles(user).FirstOrDefault(r => r.EqualTo(AppRoles.Recorder.ToString()));
      return recorder != null;
   }
   public static bool IsFileManager(this ClaimsPrincipal user)
   {
      if (Roles(user).IsNullOrEmpty()) return false;
      var file = Roles(user).FirstOrDefault(r => r.EqualTo(AppRoles.Files.ToString()));
      return file != null;
   }
   public static OAuthProvider Provider(this ClaimsPrincipal user)
   {
      string providerName = user.Claims.Find(JwtClaimIdentifiers.Provider)?.Value ?? string.Empty;
      OAuthProvider provider;
      if (!Enum.TryParse(providerName, true, out provider)) return OAuthProvider.Unknown;
      return provider;
   }

   static Claim? Find(this IEnumerable<Claim> claims, string val)
         => claims.FirstOrDefault(c => c.Type.EqualTo(val));
   
}