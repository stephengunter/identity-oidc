using ApplicationCore.Consts;
using ApplicationCore.Models;
using Infrastructure.Helpers;
using OpenIddict.Abstractions;
using System.Runtime.CompilerServices;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Permissions = OpenIddict.Abstractions.OpenIddictConstants.Permissions;
namespace ApplicationCore.Helpers;

public static class OidcHelper
{
   public static IEnumerable<Uri> GetRedirectUris(string url)
   {
      if (!url.EndsWith("/")) url += "/";
      return new List<Uri>()
      {
         new Uri(url),
         new Uri($"{url}{AppUris.SigninCallback}"),
         new Uri($"{url}{AppUris.SigninSilentCallback}")
      };
   }
   public static IEnumerable<string> GetPublicPermissions(IEnumerable<string> apis)
	{
      //Permissions =
      //      {
      //   // 端點權限
      //   OpenIddictConstants.Permissions.Endpoints.Authorization,
      //         OpenIddictConstants.Permissions.Endpoints.Token,

      //         // 授權流程權限
      //         OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
      //         OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

      //         // 回應類型權限
      //         OpenIddictConstants.Permissions.ResponseTypes.Code,

      //         // 範圍權限
      //         OpenIddictConstants.Permissions.Scopes.Email,
      //         OpenIddictConstants.Permissions.Scopes.Profile,
      //         OpenIddictConstants.Permissions.Scopes.Roles,

      //         // 如果需要自定義範圍
      //         OpenIddictConstants.Permissions.Prefixes.Scope + "identity-api"
      //         //Permissions.Prefixes.Scope + "identity-api"
      //      },
      var permissions = new List<string>()
      {
         Permissions.Endpoints.Authorization,
         Permissions.Endpoints.Token,
         Permissions.GrantTypes.AuthorizationCode,
         Permissions.GrantTypes.RefreshToken,
         Permissions.ResponseTypes.Code,
         Permissions.Scopes.Email,
         Permissions.Scopes.Profile,
         Permissions.Scopes.Roles
      };
      foreach (var api in apis) 
      {
         permissions.Add(Permissions.Prefixes.Scope + api);
      }
      return permissions;
   }
   public static IEnumerable<string> GetApiPermissions()
   {
      var permissions = new List<string>()
      {
         Permissions.Endpoints.Introspection
      };
      return permissions;
   }
   public static IEnumerable<string> GetPublicRequirements()
   {
      return new List<string>()
      {
         Requirements.Features.ProofKeyForCodeExchange
      };
   }
   public static IEnumerable<string> GetApiRequirements()
   {
      return new List<string>()
      {
         Requirements.Features.ProofKeyForCodeExchange
      };
   }

   public static string GetAppType(this string? type)
   {
      if(string.IsNullOrEmpty(type)) return "";
      if (type.EqualTo(ClientTypes.Public)) return AppTypes.Spa;
      if (type.EqualTo(ClientTypes.Confidential)) return AppTypes.Api;
      return "";
   }
}