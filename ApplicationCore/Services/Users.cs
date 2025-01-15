using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Helpers;
using ApplicationCore.Exceptions;
using ApplicationCore.Consts;
using System.Data;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services;

public interface IUsersService
{
   #region Fetch
   Task<IEnumerable<User>> FetchAllAsync(bool includeRoles = false);
   Task<IEnumerable<User>> FetchByRolesAsync(IEnumerable<Role> roles, bool includeRoles = false);
   Task<IEnumerable<User>> FetchByIdsAsync(IEnumerable<string> ids, bool includeRoles = false);
	#endregion

	#region Find
	Task<User?> FindByIdAsync(string id);
	Task<User?> FindByEmailAsync(string email);
   Task<User?> FindByUsernameAsync(string username);
   Task<User?> FindByPhoneAsync(string phone);
   #endregion

   #region Get
   Task<User?> GetByIdAsync(string id, bool includeRoles = false);
   #endregion

   #region Store
   Task<User> CreateAsync(User user);
	Task UpdateAsync(User user);
   Task UpdateRangeAsync(ICollection<User>  users);
   Task AddToRoleAsync(User user, string role);

   Task SyncRolesAsync(User user, IEnumerable<string> roles);
   #endregion

   #region Get
   Task<IList<string>> GetRolesAsync(User user);

   #endregion

   #region Check
   Task<bool> HasRoleAsync(User user, string role);
   Task<bool> IsAdminAsync(User user);
   Task<bool> HasPasswordAsync(User user);
   Task<bool> CheckPasswordAsync(User user, string password);
   #endregion
}

public class UsersService : IUsersService
{
   private readonly DefaultContext _context;
	private readonly UserManager<User> _userManager;
   private readonly IDefaultRepository<User> _usersRepository;

   public UsersService(DefaultContext context, UserManager<User> userManager, 
      IDefaultRepository<User> usersRepository)
   {
		_context = context;
		_userManager = userManager;
      _usersRepository = usersRepository;
   }
	string DevRoleName = AppRoles.Dev.ToString();
	string BossRoleName = AppRoles.Boss.ToString();

   #region Fetch
   public async Task<IEnumerable<User>> FetchAllAsync(bool includeRoles = false)
    => await _usersRepository.ListAsync(new UsersSpecification(includeRoles));
   public async Task<IEnumerable<User>> FetchByIdsAsync(IEnumerable<string> ids, bool includeRoles = false)
      => await _usersRepository.ListAsync(new UsersSpecification(ids, includeRoles));
   public async Task<IEnumerable<User>> FetchByRolesAsync(IEnumerable<Role> roles, bool includeRoles = false)
   {
      var users = await _usersRepository.ListAsync(new UsersSpecification(includeRoles));
      if (roles.IsNullOrEmpty()) return users;

      return FetchByRoles(users, roles);
   }
   #endregion

   #region Find
   public async Task<User?> FindByIdAsync(string id) => await _userManager.FindByIdAsync(id);
	public async Task<User?> FindByEmailAsync(string email) => await _userManager.FindByEmailAsync(email);
   public async Task<User?> FindByUsernameAsync(string username) => await _userManager.FindByNameAsync(username);
   public async Task<User?> FindByPhoneAsync(string phone)
      => await _usersRepository.FirstOrDefaultAsync(new UsersFetchByPhoneSpecification(phone));
	#endregion


	#region Get
	public async Task<User?> GetByIdAsync(string id, bool includeRoles = false)
       => await _usersRepository.FirstOrDefaultAsync(new UsersSpecification(id, includeRoles));
   #endregion

   #region Store
   public async Task<User> CreateAsync(User user)
	{
      var result = await _userManager.CreateAsync(user);
		if (result.Succeeded) return user;

		var error = result.Errors.FirstOrDefault();
		string msg = $"{error!.Code} : {error!.Description}" ?? string.Empty;

		throw new CreateUserException(user, msg);
	}

	public async Task UpdateAsync(User user)
	{
		var result = await _userManager.UpdateAsync(user);
		if(!result.Succeeded)
		{
			var error = result.Errors.FirstOrDefault();
			string msg = $"{error!.Code} : {error!.Description}" ?? string.Empty;

			throw new UpdateUserException(user, msg);
		}
	}

	public async Task AddToRoleAsync(User user, string role)
	{
      var result = await _userManager.AddToRoleAsync(user, role);
      if (!result.Succeeded)
      {
         var error = result.Errors.FirstOrDefault();
         string msg = $"{error!.Code} : {error!.Description}" ?? string.Empty;

         throw new UpdateUserRoleException(user, role, msg);
      }
   }
   public async Task UpdateRangeAsync(ICollection<User> users)
   {
      await _usersRepository.UpdateRangeAsync(users);
   }

   public async Task SyncRolesAsync(User user, IEnumerable<string> roles)
   {
      var currentRoles = await GetRolesAsync(user);
      await _userManager.RemoveFromRolesAsync(user, currentRoles);

      if(roles.HasItems()) await _userManager.AddToRolesAsync(user, roles);
   }
   #endregion

   #region Get
   public async Task<IList<string>> GetRolesAsync(User user) => await _userManager.GetRolesAsync(user);

   #endregion

   #region Check
   public async Task<bool> HasRoleAsync(User user, string role) 
      => await _userManager.IsInRoleAsync(user, role);
   public async Task<bool> IsAdminAsync(User user)
	{
		var roles = await GetRolesAsync(user);
		if (roles.IsNullOrEmpty()) return false;

		var match = roles.Where(r => r.Equals(DevRoleName) || r.Equals(BossRoleName)).FirstOrDefault();

		return match != null;
	}
   public async Task<bool> HasPasswordAsync(User user)
      => await _userManager.HasPasswordAsync(user);

   public async Task<bool> CheckPasswordAsync(User user, string password)
      => await _userManager.CheckPasswordAsync(user, password);
	#endregion

	#region Helper
   IEnumerable<User> FetchByRoles(IEnumerable<User> users, IEnumerable<Role> roles)
   {
      var roleIds = roles.Select(x => x.Id);
      var userIdsInRoles = _context.UserRoles.Where(x => roleIds.Contains(x.RoleId)).Select(b => b.UserId).Distinct().ToList();
      return users.Where(user => userIdsInRoles.Contains(user.Id));
   }
   #endregion








}
