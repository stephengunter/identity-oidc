﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ApplicationCore.Models;
using ApplicationCore.Consts;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Permissions = OpenIddict.Abstractions.OpenIddictConstants.Permissions;

namespace ApplicationCore.DataAccess;

public static class SeedData
{
	
	static string DevRoleName = AppRoles.Dev.ToString();
	static string BossRoleName = AppRoles.Boss.ToString();
   static string ITRoleName = AppRoles.IT.ToString();
   static string RecorderRoleName = AppRoles.Recorder.ToString();
   static string ClerkRoleName = AppRoles.Clerk.ToString();
   static string FilesRoleName = AppRoles.Files.ToString();
   static string DriverRoleName = AppRoles.Driver.ToString();
   static string CarManagerRoleName = AppRoles.CarManager.ToString();

   public static async Task EnsureSeedData(IServiceProvider serviceProvider, ConfigurationManager configuration)
	{
		string adminEmail = configuration[$"{SettingsKeys.Admin}:Email"] ?? "";
		string adminPhone = configuration[$"{SettingsKeys.Admin}:Phone"] ?? "";
		string adminName = configuration[$"{SettingsKeys.Admin}:Name"] ?? "";

		if(String.IsNullOrEmpty(adminEmail) || String.IsNullOrEmpty(adminPhone))
		{
			throw new Exception("Failed to SeedData. Empty Admin Email/Phone.");
		}
		if(String.IsNullOrEmpty(adminName))
		{
			throw new Exception("Failed to SeedData. Empty Admin Name.");
		}

		Console.WriteLine("Seeding database...");

		var context = serviceProvider.GetRequiredService<DefaultContext>();
	   context.Database.EnsureCreated();

      var manager = serviceProvider.GetRequiredService<IOpenIddictApplicationManager>();

      using (var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>())
		{
			await SeedRoles(roleManager);
		}

		if(!String.IsNullOrEmpty(adminEmail)) {
			using (var userManager = serviceProvider.GetRequiredService<UserManager<User>>())
			{
				var user = new User
				{
					Email = adminEmail,			
					UserName = adminEmail,
					Name = adminName,
					PhoneNumber = adminPhone,
					EmailConfirmed = true,
					SecurityStamp = Guid.NewGuid().ToString(),
					Active = true
				};
				await CreateUserIfNotExist(userManager, user, new List<string>() { DevRoleName });
			}
		}
      Console.WriteLine("Done seeding database.");
	}
   static async Task SeedRoles(RoleManager<Role> roleManager)
	{
		var roles = new List<Role> 
		{ 
			new Role { Name = DevRoleName, Title = "開發者" },
         new Role { Name = BossRoleName, Title = "老闆" },
         new Role { Name = ITRoleName, Title = "資訊人員" },
         new Role { Name = RecorderRoleName, Title = "錄事" },
         new Role { Name = ClerkRoleName, Title = "書記官" },
         new Role { Name = FilesRoleName, Title = "檔案管理員" },
         new Role { Name = DriverRoleName, Title = "司機" },
         new Role { Name = CarManagerRoleName, Title = "車輛管理" }
      };
		foreach (var item in roles) await AddRoleIfNotExist(roleManager, item);
	}
	static async Task AddRoleIfNotExist(RoleManager<Role> roleManager, Role role)
	{
		var existingRole = await roleManager.FindByNameAsync(role.Name!);
		if (existingRole == null) await roleManager.CreateAsync(role);
		else
		{
         existingRole.Title = role.Title;
			await roleManager.UpdateAsync(existingRole);
      } 

   }
	static async Task CreateUserIfNotExist(UserManager<User> userManager, User newUser, IList<string>? roles = null)
	{
		var user = await userManager.FindByEmailAsync(newUser.Email!);
		if (user == null)
		{
			var result = await userManager.CreateAsync(newUser);

			if (roles!.HasItems())
			{
				await userManager.AddToRolesAsync(newUser, roles!);
			}
		}
		else
		{
			user.PhoneNumber = newUser.PhoneNumber;
			user.Name = newUser.Name;
			await userManager.UpdateAsync(user);
			if (roles!.HasItems())
			{
				foreach (var role in roles!)
				{
					bool hasRole = await userManager.IsInRoleAsync(user, role);
					if (!hasRole) await userManager.AddToRoleAsync(user, role);
				}
			}
		}
	}
}