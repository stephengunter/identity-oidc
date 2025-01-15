using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Consts;

namespace ApplicationCore.Authorization;

public class HasPermissionHandler : AuthorizationHandler<HasPermissionRequirement>
{
   
   protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermissionRequirement requirement)
   {
      if (requirement.Permission == Permissions.Admin)
      {
         if (context.User.IsBoss() || context.User.IsDev() || context.User.IsIT())
         {
            context.Succeed(requirement);
            return Task.CompletedTask;
         }
      }
      else if (requirement.Permission == Permissions.JudgebookFiles)
      {
         if (context.User.IsBoss() || context.User.IsDev() || context.User.IsIT())
         {
            context.Succeed(requirement);
            return Task.CompletedTask;
         }
         if (context.User.IsFileManager() || context.User.IsClerk() || context.User.IsRecorder())
         {
            context.Succeed(requirement);
            return Task.CompletedTask;
         }
      }

      context.Fail();
      return Task.CompletedTask;
   }


}
