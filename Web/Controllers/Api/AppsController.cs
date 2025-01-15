using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Api;

public class AppsController : BaseApiController
{
   public AppsController()
   {
      
   }
   
   [HttpGet]   
   public async Task<IActionResult> Index()
   {
      return Ok("users");
   }
}
