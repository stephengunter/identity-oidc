using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using ApplicationCore.Services;

namespace Web.Controllers.Tests;

public class AATestsController : BaseTestController
{
   private readonly IUsersService _usersService;
   public AATestsController(IUsersService usersService)
   {
      _usersService = usersService;
   }
   [HttpGet]
   public async Task<ActionResult> Index()
   {
      string url = "http://localhost:3000/";
      if(!url.EndsWith("/")) url += "/";
      string src = "identity-api";
      string path = $"{url}login?source={src}";
      return Ok(path);   
   }
}
