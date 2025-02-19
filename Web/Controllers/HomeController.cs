using System.Diagnostics;
using System.Security.Claims;
using ApplicationCore.Consts;
using ApplicationCore.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Services;
using ApplicationCore.Settings;
using AutoMapper;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.Models;

namespace Web.Controllers;

[Authorize]
public class HomeController : Controller
{
   private readonly IAppService _appService;
   private readonly AppSettings _appSettings;
   private readonly IMapper _mapper;

   public HomeController(IAppService appService, IOptions<AppSettings> appSettings, IMapper mapper)
   {
      _appService = appService;
      _appSettings = appSettings.Value;
      _mapper = mapper;
   }

   public async Task<IActionResult> Index()
   {
      var apps = await _appService.FetchByTypeAsync(AppTypes.Spa);
      var availableApps = new List<App>();
      foreach (var app in apps)
      { 
         if(IsAvailable(app)) availableApps.Add(app);
      }
      string clientId = _appSettings.ClientId;
      var appViews = availableApps.MapViewModelList(clientId, _mapper);
      var model = new HomeModel(appViews);

      return View(model);
   }

   private bool IsAvailable(App app)
   {
      if (string.IsNullOrEmpty(app.Roles)) return true;
      if(UserIsAdmin) return true;

      return app.Roles.SplitToList().Any(x => UserRoles.Contains(x));
   }

   private List<string> AdminRole => new List<string> { AdminRoles.Dev, AdminRoles.Boss };

   private bool UserIsAdmin => UserRoles.Any(x => AdminRole.Contains(x));

   private List<string> UserRoles => User.Claims
                                    .Where(c => c.Type == ClaimTypes.Role)
                                    .Select(c => c.Value)
                                    .ToList();

}
