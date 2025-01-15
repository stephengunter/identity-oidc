using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Settings;
using Microsoft.AspNetCore.Cors;
using Web.Models;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Authorization;
using ApplicationCore.Exceptions;
using Infrastructure.Helpers;
using OpenIddict.Validation.AspNetCore;

namespace Web.Controllers;

[ApiController]
[Route("[controller]")]
public abstract class BaseController : ControllerBase
{
   protected string RemoteIpAddress => HttpContext.Connection.RemoteIpAddress is null ? "" : HttpContext.Connection.RemoteIpAddress.ToString();
   
   protected void CheckCurrentUser(User user)
   {
      string id = User.Id();
      if(String.IsNullOrEmpty(id)) throw new CurrentUserIdNotFoundException();
      if(id != user.Id) throw new CurrentUserIdNotEqualToRequestUserIdException();
   }

   protected string TempPath(IWebHostEnvironment environment)
     => Path.Combine(environment.WebRootPath, "temp");

   protected string GetTempPath(IWebHostEnvironment environment, string folder)
     => Path.Combine(TempPath(environment), folder);

   protected string TemplatePath(IWebHostEnvironment environment, AppSettings appSettings)
     => Path.Combine(environment.WebRootPath, appSettings.TemplatePath.HasValue() ? appSettings.TemplatePath : "templates");

   protected string GetMailTemplate(IWebHostEnvironment environment, AppSettings appSettings, string name = "default")
   {
      var pathToFile = Path.Combine(TemplatePath(environment, appSettings), $"{name}.html");
      if (!System.IO.File.Exists(pathToFile)) throw new Exception("email template file not found: " + pathToFile);

      string body = "";
      using (StreamReader reader = System.IO.File.OpenText(pathToFile))
      {
         body = reader.ReadToEnd();
      }

      return body.Replace("APPNAME", appSettings.Title).Replace("APPURL", appSettings.ClientUrl);
   }

   protected void AddErrors(Dictionary<string, string> errors)
   {
      if (errors.Count > 0)
      {
         foreach (var kvp in errors)
         {
            ModelState.AddModelError(kvp.Key, kvp.Value);
         }
      }
   }
}


[EnableCors("Api")]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public abstract class BaseApiController : BaseController
{
   
}

[EnableCors("Admin")]
[Route("admin/[controller]")]
[Authorize(Policy = "Admin")]
public class BaseAdminController : BaseController
{
   
}


[EnableCors("Global")]
[Route("tests/[controller]")]
public abstract class BaseTestController : BaseController
{

}

[EnableCors("Open")]
[Route("open/[controller]")]
public abstract class BaseOpenController : BaseController
{

}





