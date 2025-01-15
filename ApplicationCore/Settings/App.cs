namespace ApplicationCore.Settings;
public class AppSettings
{
   public string Key { get; set; } = string.Empty;
   public string? Name { get; set; }
	public string? Title { get; set; }
	public string? Email { get; set; }
	public string? ClientUrl { get; set; }
	public string? AdminUrl { get; set; }
	public string? BackendUrl { get; set; }

   public string UploadPath { get; set; } = string.Empty;
   public string TemplatePath { get; set; } = string.Empty;
   public string? ApiVersion { get; set; }
}



public class AuthSettings
{
	public string SecurityKey { get; set; } = string.Empty;
	public int TokenValidHours { get; set; }
	public int RefreshTokenDaysToExpire { get; set; }

}

public class AdminSettings
{
	public string Key { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string Id { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public string Phone { get; set; } = string.Empty;
	public string BackupPath { get; set; } = string.Empty;
	public string DataPath { get; set; } = string.Empty;
}


