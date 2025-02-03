using ApplicationCore.Consts;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using System.Text.Json.Serialization;
using Infrastructure.Helpers;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using ApplicationCore.DI;
using ApplicationCore.Settings;
using OpenIddict.Validation.AspNetCore;
using OpenIddict.Server.AspNetCore;
using Polly;

Log.Logger = new LoggerConfiguration()
   .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
   .Enrich.FromLogContext()
   .WriteTo.Console()
   .CreateBootstrapLogger();

try
{
   Log.Information("Starting web application");
   var builder = WebApplication.CreateBuilder(args);
   var Configuration = builder.Configuration;
   builder.Host.UseSerilog((context, services, configuration) => configuration
         .ReadFrom.Configuration(context.Configuration)
         .ReadFrom.Services(services)
         .Enrich.FromLogContext());

   #region Autofac
   builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
   builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
   {
      builder.RegisterModule<ApplicationCoreModule>();
   });

   #endregion
   var services = builder.Services;

   #region Add Configurations
   services.Configure<AppSettings>(Configuration.GetSection(SettingsKeys.App));
   services.Configure<AdminSettings>(Configuration.GetSection(SettingsKeys.Admin));
   services.Configure<AuthSettings>(Configuration.GetSection(SettingsKeys.Auth));
   services.Configure<CompanySettings>(Configuration.GetSection(SettingsKeys.Company));
   #endregion

   string connectionString = Configuration.GetConnectionString("Default")!;
   services.AddDbContext<DefaultContext>(options =>
   {
      options.UseSqlServer(connectionString);
      options.UseOpenIddict();
   });
   

   #region AddIdentity
   services.AddIdentity<User, Role>(options =>
   {
      options.User.RequireUniqueEmail = false;
      options.Password.RequireNonAlphanumeric = false;
      options.Password.RequireUppercase = false;
      options.Password.RequiredLength = 3;
      options.Password.RequireDigit = false;
   })
   .AddEntityFrameworkStores<DefaultContext>()
   .AddDefaultTokenProviders();
   #endregion

   #region OpenIddict
   string securityKey = Configuration[$"{SettingsKeys.Auth}:SecurityKey"] ?? "";
   if (String.IsNullOrEmpty(securityKey))
   {
      throw new Exception("Failed Add AddJwtBearer. Empty SecurityKey.");
   }
   services.AddOpenIddict()
    .AddCore(options =>
    {
       options.UseEntityFrameworkCore()
              .UseDbContext<DefaultContext>();
    })
    .AddServer(options =>
    {
       // Enable the authorization, introspection and token endpoints.
       options.SetAuthorizationEndpointUris("authorize")
             .SetTokenEndpointUris("token");

       // Note: this sample only uses the authorization code and refresh token
       // flows but you can enable the other flows if you need to support implicit,
       // password or client credentials.
       options.AllowAuthorizationCodeFlow()
          .AllowRefreshTokenFlow();

       options.AddEncryptionKey(new SymmetricSecurityKey(
          Convert.FromBase64String(securityKey)));

       // Register the signing credentials.
       options.AddDevelopmentSigningCertificate();

       // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
       //
       // Note: unlike other samples, this sample doesn't use token endpoint pass-through
       // to handle token requests in a custom MVC action. As such, the token requests
       // will be automatically handled by OpenIddict, that will reuse the identity
       // resolved from the authorization code to produce access and identity tokens.
       //
       options.UseAspNetCore()
             .EnableAuthorizationEndpointPassthrough();
    })
    .AddValidation(options =>
    {
       // Import the configuration from the local OpenIddict server instance.
       options.UseLocalServer();

       // Register the ASP.NET Core host.
       options.UseAspNetCore();

       
    });
   #endregion

   services.AddCorsPolicy(Configuration);
   builder.Services.AddAuthorizationPolicy();
   services.AddDtoMapper();
   services.AddControllers()
      .AddJsonOptions(options =>
      {
         options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      });
   services.AddSwagger(Configuration);

   var app = builder.Build();
   //app.UseDefaultFiles();
  
   app.UseSerilogRequestLogging();

   if (app.Environment.IsDevelopment())
   {
      if (Configuration[$"{SettingsKeys.Developing}:SeedDatabase"].ToBoolean())
      {
         // Seed Database
         using (var scope = app.Services.CreateScope())
         {
            try
            {
               await SeedData.EnsureSeedData(scope.ServiceProvider, Configuration);
            }
            catch (Exception ex)
            {
               Log.Fatal(ex, "SeedData Error");
            }
         }
      }
      app.UseSwagger();
      app.UseSwaggerUI();
   }
   else
   {

   }

   app.UseHttpsRedirection();

   app.UseStaticFiles(); 
   app.UseRouting();

   app.UseCors("Api");
   
   app.UseAuthentication();
   app.UseAuthorization();

   app.Use(async (context, next) =>
   {
      if (context.Request.Path == "/")
      {
         context.Response.ContentType = "text/html";
         await context.Response.SendFileAsync("wwwroot/index.html");
         return;
      }

      await next();
   });
   app.MapControllers();

   app.Run();
}
catch (Exception ex)
{
   Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
   Log.Information("finally");
   Log.CloseAndFlush();
}