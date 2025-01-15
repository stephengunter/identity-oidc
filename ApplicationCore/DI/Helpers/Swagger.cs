using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using ApplicationCore.Consts;

namespace ApplicationCore.DI;
public static class SwaggerDI
{
	public static void AddSwagger(this IServiceCollection services, ConfigurationManager Configuration)
	{
		string title = Configuration[$"{SettingsKeys.App}:Name"] ?? "";
    	string version = Configuration[$"{SettingsKeys.App}:ApiVersion"] ?? "";
		services.AddSwaggerGen(options =>
		{
			options.SwaggerDoc(version, new OpenApiInfo
			{
				Version = version,
				Title = title
			});
			options.EnableAnnotations();
			options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				In = ParameterLocation.Header,
				Description = "Please insert JWT with Bearer into field",
				Name = "Authorization",
				Type = SecuritySchemeType.ApiKey,
			});

			options.AddSecurityRequirement(new OpenApiSecurityRequirement()
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						},
						Scheme = "oauth2",
						Name = "Bearer",
						In = ParameterLocation.Header
					},
					new List<string>()
				}
			});
		});
	}
}
