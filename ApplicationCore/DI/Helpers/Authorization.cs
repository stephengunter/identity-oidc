using ApplicationCore.Authorization;
using ApplicationCore.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationCore.DI;
public static class AuthorizationDI
{
   public static void AddAuthorizationPolicy(this IServiceCollection services)
   {
      services.AddAuthorization(options =>
      {
         options.AddPolicy(Permissions.Admin.ToString(), policy =>
            policy.Requirements.Add(new HasPermissionRequirement(Permissions.Admin)));

         options.AddPolicy(Permissions.JudgebookFiles.ToString(), policy =>
            policy.Requirements.Add(new HasPermissionRequirement(Permissions.JudgebookFiles)));
      });

      services.AddSingleton<IAuthorizationHandler, HasPermissionHandler>();
   }
}
