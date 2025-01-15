using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Api;
[Route("api/weather")]
[ApiController]
public class WeatherController : ControllerBase
{
   private static readonly string[] Summaries = new[]
   {
         "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
      };

   private readonly ILogger<WeatherController> _logger;

   public WeatherController(ILogger<WeatherController> logger)
   {
      _logger = logger;
   }

   [Authorize]
   [HttpGet("")]
   public async Task<ActionResult<ICollection<WeatherForecast>>> Get()
   {

      return Enumerable.Range(1, 5).Select(index => new WeatherForecast
      {
         Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
         TemperatureC = Random.Shared.Next(-20, 55),
         Summary = Summaries[Random.Shared.Next(Summaries.Length)]
      })
      .ToList();
   }
}
public class WeatherForecast
{
   public DateOnly Date { get; set; }

   public int TemperatureC { get; set; }

   public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

   public string? Summary { get; set; }
}
