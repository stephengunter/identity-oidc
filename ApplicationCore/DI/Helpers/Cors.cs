using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ApplicationCore.Consts;

namespace ApplicationCore.DI;
public static class CorsDI
{
   public static void AddCorsPolicy(this IServiceCollection services, ConfigurationManager Configuration)
   {
      string clientUrl = Configuration[$"{SettingsKeys.App}:ClientUrl"] ?? "";
      string adminUrl = Configuration[$"{SettingsKeys.App}:AdminUrl"] ?? "";
      if(String.IsNullOrEmpty(clientUrl))
      {
         throw new Exception("Failed Add Cors. Empty ClientUrl.");
      }
      if(String.IsNullOrEmpty(adminUrl))
      {
         throw new Exception("Failed Add Cors. Empty AdminUrl.");
      }

      services.AddCors(options =>
      {
         options.AddPolicy("Api",
         builder =>
         {
            builder.WithOrigins(clientUrl, adminUrl)
                 .AllowAnyHeader()
						.AllowAnyMethod();//.AllowCredentials();
         });

         options.AddPolicy("Admin",
         builder =>
         {
				builder.WithOrigins(adminUrl)
						.AllowAnyHeader()
						.AllowAnyMethod();
         });

         options.AddPolicy("Global",
         builder =>
         {
               builder.WithOrigins(clientUrl, adminUrl)
                     .AllowAnyHeader()
                     .AllowAnyMethod();
         });

         options.AddPolicy("Open",
         builder =>
         {
               builder.AllowAnyOrigin()
                     .AllowAnyHeader()
                     .AllowAnyMethod();
         });
      });
	}
}
