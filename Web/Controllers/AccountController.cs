using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using ApplicationCore.Models;
using OpenIddict.Abstractions;
using Azure.Core;
using Infrastructure.Helpers;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Identity;

namespace Web.Controllers;

public class AccountController : Controller
{
   private readonly IUsersService _usersService;
   private readonly SignInManager<User> _signInManager;
   public AccountController(IUsersService usersService, SignInManager<User> signInManager)
   {
      _usersService = usersService;
      _signInManager = signInManager;
   }

   [HttpGet]
   [AllowAnonymous]
   public IActionResult Login(string? returnUrl = null)
   {
      ViewData["ReturnUrl"] = returnUrl;
      return View();
   }

   [HttpPost]
   [AllowAnonymous]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> Login(PasswordLoginRequest model)
   {
      ViewData["ReturnUrl"] = model.ReturnUrl;
      ValidateRequest(model);
      if (!ModelState.IsValid) return View(model);

      var user = await _usersService.FindByUsernameAsync(model.Username);
      if (user == null)
      {
         ModelState.AddModelError("Password", "身分驗證失敗, 請重新登入.");
         return View(model);
      }
      var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
      if (result.Succeeded)
      {
         var userPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
         await HttpContext.SignInAsync(userPrincipal);
      }

      if (Url.IsLocalUrl(model.ReturnUrl)) return Redirect(model.ReturnUrl);

      return RedirectToAction(nameof(HomeController.Index), "Home");
   }
   void ValidateRequest(PasswordLoginRequest request)
   {
      if (String.IsNullOrEmpty(request.Username)) ModelState.AddModelError("Username", ValidationMessages.Required("Username"));
      if (String.IsNullOrEmpty(request.Password)) ModelState.AddModelError("Password", ValidationMessages.Required("Password"));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> Logout()
   {
      await _signInManager.SignOutAsync();
      return RedirectToAction("Login", "Account");
   }

}
