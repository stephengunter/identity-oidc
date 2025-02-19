using ApplicationCore.Consts;
using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class PasswordLoginRequest
{
   [Required(ErrorMessage = "必須填寫使用者名稱")]
   public string Username { get; set; } = String.Empty;

   [Required(ErrorMessage = "必須填寫密碼")]
   [DataType(DataType.Password)]
   public string Password { get; set; } = String.Empty;

   public string? ReturnUrl { get; set; }
}
public class LoginRequest
{
   public string AppId { get; set; } = string.Empty;
   public string Scope { get; set; } = string.Empty;
}