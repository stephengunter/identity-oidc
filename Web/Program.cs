using ApplicationCore.Consts;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using ApplicationCore.DI;
using ApplicationCore.Settings;
using OpenIddict.Server.AspNetCore;
using ApplicationCore.Helpers;

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
   services.AddControllersWithViews();

   #region Add Configurations
   services.Configure<AppSettings>(Configuration.GetSection(SettingsKeys.App));
   services.Configure<AdminSettings>(Configuration.GetSection(SettingsKeys.Admin));
   services.Configure<AuthSettings>(Configuration.GetSection(SettingsKeys.Auth));
   services.Configure<CompanySettings>(Configuration.GetSection(SettingsKeys.Company));
   #endregion

  
   services.AddAuthentication(options =>
   {
      options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme; // Important!
   })
   .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
   {
      options.LoginPath = "/account/login"; 
   });

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
       options
         .AllowClientCredentialsFlow()
         .AllowAuthorizationCodeFlow()
           // .RequireProofKeyForCodeExchange() // PKCE for security
         .AllowRefreshTokenFlow();


       // Enable the authorization, introspection and token endpoints.
       options
         .SetTokenEndpointUris("/connect/token")
         .SetAuthorizationEndpointUris("/connect/authorize")
         .SetUserInfoEndpointUris("/connect/userinfo");

       //
       //options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.OpenId);


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
         .EnableTokenEndpointPassthrough()
         .EnableAuthorizationEndpointPassthrough()
         .EnableUserInfoEndpointPassthrough();

    })
   .AddValidation(options =>
   {
      // Import the configuration from the local OpenIddict server instance.
      options.UseLocalServer();

      // Register the ASP.NET Core host.
      options.UseAspNetCore();

   });
   #endregion
   string key = Configuration[$"{SettingsKeys.App}:Key"]!;
   if (String.IsNullOrEmpty(key))
   {
      throw new Exception("app key not been set.");
   }
   
   services.AddScoped<ICryptoService>(provider => new AesGcmCryptoService(key.DeriveKeyFromString()));

   services.AddCorsPolicy(Configuration);
   services.AddAuthorizationPolicy();
   services.AddDtoMapper();
   //services.AddControllers()
   //   .AddJsonOptions(options =>
   //   {
   //      options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
   //   });
   services.AddSwagger(Configuration);

   var app = builder.Build();
  
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

   //app.Use(async (context, next) =>
   //{
   //   if (context.Request.Path == "/")
   //   {
   //      context.Response.ContentType = "text/html";
   //      await context.Response.SendFileAsync("wwwroot/index.html");
   //      return;
   //   }

   //   await next();
   //});
   app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}")
      .WithStaticAssets();

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