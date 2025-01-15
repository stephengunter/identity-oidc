using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicationCore.DataAccess.Config;
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
	public void Configure(EntityTypeBuilder<UserRole> builder)
	{
      builder.HasKey(item => new { item.UserId, item.RoleId });

      builder.HasOne<User>(item => item.User)
         .WithMany(item => item.UserRoles)
         .HasForeignKey(item => item.UserId);


      builder.HasOne<Role>(item => item.Role)
         .WithMany(item => item.UserRoles)
         .HasForeignKey(item => item.RoleId);
   }
}
