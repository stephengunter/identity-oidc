using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Helpers;
using ApplicationCore.Consts;
using System.Data;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services;

public interface IRolesService
{
   Task<IEnumerable<Role>> FetchAsync();
   Task<IEnumerable<Role>> FetchAllAsync();
   
   Task<IEnumerable<Role>> FetchByIdsAsync(ICollection<string> ids);

   Task<Role?> FindAsync(string name);
   Task<Role?> FindByIdAsync(string id);
   IEnumerable<Role> GetRolesByUser(User user);
}

public class RolesService : IRolesService
{
   private readonly DefaultContext _context;
   private readonly RoleManager<Role> _roleManager;
   private readonly IDefaultRepository<Role> _rolesRepository;
   public RolesService(DefaultContext context, RoleManager<Role> roleManager, IDefaultRepository<Role> rolesRepository)
   {
		_roleManager = roleManager;
      _rolesRepository = rolesRepository;
      _context = context;
   }
	string DevRoleName = AdminRoles.Dev;
	string BossRoleName = AdminRoles.Boss;

   public async Task<IEnumerable<Role>> FetchAsync()
      => await _roleManager.Roles
                .Where(r => r.Name != DevRoleName && r.Name != BossRoleName)
                .ToListAsync();

   public async Task<IEnumerable<Role>> FetchAllAsync()
      => await _roleManager.Roles.ToListAsync();
   public async Task<IEnumerable<Role>> FetchByIdsAsync(ICollection<string> ids)
      => await _rolesRepository.ListAsync(new RolesIdSpecification(ids));
   public async Task<Role?> FindByIdAsync(string id)
      => await _roleManager.FindByIdAsync(id);

   public async Task<Role?> FindAsync(string name)
      => await _roleManager.FindByNameAsync(name);

   public IEnumerable<Role> GetRolesByUser(User user)
   {
      var userRoles = _context.UserRoles.Where(x => x.UserId == user.Id);
      var roleIds = userRoles.Select(ur => ur.RoleId);

      return _roleManager.Roles.Where(r => roleIds.Contains(r.Id));
   }

}
