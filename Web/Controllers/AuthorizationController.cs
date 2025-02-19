using Microsoft.AspNetCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using ApplicationCore.Models;
using Infrastructure.Helpers;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;


namespace Web.Controllers;


[ApiController]
public class AuthorizationController : ControllerBase
{
   private readonly IUsersService _usersService;
   public AuthorizationController(IUsersService usersService)
   {
      _usersService = usersService;
   }



   [HttpPost]
   [HttpGet]
   [Route("connect/authorize")]
   public async Task<IActionResult> Authorize()
   {
      var request = HttpContext.GetOpenIddictServerRequest();
      if (request is null)
      {
         return BadRequest("Invalid authorization request.");
      }

      // Retrieve the user principal stored in the authentication cookie.
      var result = await HttpContext.AuthenticateAsync();
      // If the user principal can't be extracted, redirect the user to the login page.
      if (!result.Succeeded)
      {
         //return Challenge(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
         return Challenge(
             authenticationSchemes: CookieAuthenticationDefaults.AuthenticationScheme,
             properties: new AuthenticationProperties
             {
                RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                     Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
             });
      }

      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (string.IsNullOrEmpty(userId))
      {
         throw new Exception("User Id not found in cookie.");
      }
      var user = await _usersService.FindByIdAsync(userId);
      if (user == null)
      {
         ModelState.AddModelError("", "身分驗證失敗, 請重新登入.");
         return BadRequest(ModelState);
      }
      var roles = await _usersService.GetRolesAsync(user);

      var scopes = request.GetScopes();
      var identity = CreateClaimsIdentity(user, scopes, roles);

      return SignIn(new ClaimsPrincipal(identity), properties: null, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
   }

   ClaimsIdentity CreateClaimsIdentity(User user, IList<string> scopes, IList<string>? roles)
   {
      var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
      identity.AddClaim(Claims.Subject, user.Id);
      identity.AddClaim(Claims.Name, user.UserName!);
      identity.AddClaim("email", user.Email!);
      identity.AddClaim("roles", roles.JoinToString());

      identity.SetScopes(scopes);
      // Allow all claims to be added in the access tokens.
      identity.SetDestinations(claim => [Destinations.AccessToken, Destinations.IdentityToken]);
      return identity;
   }

   [HttpPost("~/connect/token")]
   public async Task<IActionResult> Exchange()
   {
      var request = HttpContext.GetOpenIddictServerRequest() ??
                          throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

      ClaimsPrincipal claimsPrincipal;

      if (request.IsClientCredentialsGrantType())
      {
         // Note: the client credentials are automatically validated by OpenIddict:
         // if client_id or client_secret are invalid, this action won't be invoked.

         var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

         // Subject (sub) is a required field, we use the client id as the subject identifier here.
         identity.AddClaim(OpenIddictConstants.Claims.Subject, request.ClientId ?? throw new InvalidOperationException());

         // Add some claim, don't forget to add destination otherwise it won't be added to the access token.
         identity.AddClaim("some-claim", "some-value", OpenIddictConstants.Destinations.AccessToken);

         claimsPrincipal = new ClaimsPrincipal(identity);

         claimsPrincipal.SetScopes(request.GetScopes());
      }
      else if (request.IsAuthorizationCodeGrantType())
      {
         // Retrieve the claims principal stored in the authorization code
         claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
      }
      else if (request.IsRefreshTokenGrantType())
      {
         // Retrieve the claims principal stored in the refresh token.
         claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
      }
      else
      {
         throw new InvalidOperationException("The specified grant type is not supported.");
      }

      // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
      return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
   }
}
