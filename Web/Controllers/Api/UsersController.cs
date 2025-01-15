using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore;
using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Web.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Polly;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Validation.AspNetCore;

namespace Web.Controllers.Api;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
   public UsersController()
   {
      
   }

   [HttpGet]
   [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
   public async Task<IActionResult> Index()
   {
      return Ok(User.Identity!.Name);
   }
}
