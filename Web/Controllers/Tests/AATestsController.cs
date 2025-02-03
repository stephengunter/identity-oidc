using Microsoft.AspNetCore.Mvc;
using ApplicationCore.DataAccess;
using OpenIddict.Abstractions;
using ApplicationCore.Consts;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Permissions = OpenIddict.Abstractions.OpenIddictConstants.Permissions;

namespace Web.Controllers.Tests;

public class AATestsController : BaseTestController
{
   private readonly IOpenIddictApplicationManager _applicationManager;
   public AATestsController(IOpenIddictApplicationManager applicationManager)
   {
      _applicationManager = applicationManager;
   }
   [HttpGet]
   public async Task<ActionResult> Index()
   {
      var application = await _applicationManager.FindByClientIdAsync("test96967854");
      if (application == null) return NotFound();
      await _applicationManager.DeleteAsync(application);
      return Ok("test");
   }
}
