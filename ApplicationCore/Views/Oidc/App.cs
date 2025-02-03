namespace ApplicationCore.Views.Oidc;

public class AppViewModel
{
   public string Id { get; set; } = string.Empty;
   public string ClientId { get; set; } = string.Empty;
   public string ClientType { get; set; } = string.Empty;
   public string DisplayName { get; set; } = string.Empty;
   public List<string> RedirectUris { get; set; } = new List<string>();
   public List<string> PostLogoutRedirectUris { get; set; } = new List<string>();
   public bool Enabled { get; set; }

}
