namespace Web.Models;

public class LoginRequest
{
   public string AppId { get; set; } = string.Empty;
   public string Scope { get; set; } = string.Empty;
}
