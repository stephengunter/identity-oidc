using ApplicationCore.Models;
using ApplicationCore.Models.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Identity;

namespace ApplicationCore.DataAccess;
public class DefaultContext : IdentityDbContext<User, Role, string,
        IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
{
  
   public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
	{
      
   }
   protected override void OnModelCreating(ModelBuilder builder)
   {
      base.OnModelCreating(builder);
      builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
      builder.UseOpenIddict();
   }
   public DbSet<Profiles> Profiles => Set<Profiles>();


   #region Auth
   public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
	public DbSet<OAuth> OAuth => Set<OAuth>();
   public DbSet<AuthToken> AuthTokens => Set<AuthToken>();
   #endregion

   public override int SaveChanges() => SaveChangesAsync().GetAwaiter().GetResult();

}
