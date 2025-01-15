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
using Microsoft.EntityFrameworkCore;
using Polly;
using ApplicationCore.Services;
using ApplicationCore.Models;
using Infrastructure.Helpers;
using static System.Net.Mime.MediaTypeNames;

namespace Web.Controllers;

[Route("login")]
[ApiController]
public class LoginController : ControllerBase
{
   private readonly IOpenIddictApplicationManager _appManager;
   private readonly IOpenIddictScopeManager _scopeManager;
   private readonly IUsersService _usersService;
   public LoginController(IOpenIddictApplicationManager appManager, IOpenIddictScopeManager scopeManager,
      IUsersService usersService)
   {
      _appManager = appManager;
      _scopeManager = scopeManager;
      _usersService = usersService;
   }

   [HttpPost]
   public async Task<IActionResult> Login([FromBody] LoginRequest request)
   {
      if (request == null || string.IsNullOrEmpty(request.AppId) || string.IsNullOrEmpty(request.Scope))
      {
         return BadRequest(new { message = "Invalid login request." });
      }
      var client = await _appManager.FindByClientIdAsync(request.AppId);
      if (client == null)
      {
         return Unauthorized(new { message = "Invalid client." });
      }

      var scopes = request.Scope.Split(" ");

      //find user 
      var user = await _usersService.FindByUsernameAsync("traders.com.tw@gmail.com");
      if (user == null)
      {
         ModelState.AddModelError("", "身分驗證失敗, 請重新登入.");
         return BadRequest(ModelState);
      }
      var roles = await _usersService.GetRolesAsync(user);
      var identity = CreateClaimsIdentity(user, scopes, roles);

      return SignIn(new ClaimsPrincipal(identity), properties: null, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
   }

   ClaimsIdentity CreateClaimsIdentity(User user, IList<string> scopes, IList<string>? roles)
   {
      var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
      identity.AddClaim(Claims.Subject, user.Id);
      identity.AddClaim(Claims.Name, user.Name);
      identity.AddClaim("email", user.Email!); // Add email as a custom claim
      identity.AddClaim("roles", roles.JoinToString()); // Add roles as a custom claim

      identity.SetScopes(scopes);
      // Allow all claims to be added in the access tokens.
      identity.SetDestinations(claim => [Destinations.AccessToken]);
      return identity;
   }

}
