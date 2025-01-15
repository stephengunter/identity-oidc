using Ardalis.Specification;
using ApplicationCore.Models;

namespace ApplicationCore.Specifications;
public class UsersSpecification : Specification<User>
{
   public UsersSpecification(bool includeRoles = false)
	{
      if (includeRoles) Query.Include(u => u.Profiles).Include(user => user.UserRoles);
      else Query.Include(user => user.Profiles);
   }
   public UsersSpecification(string id, bool includeRoles = false)
   {
      if (includeRoles) Query.Include(user => user.Profiles).Include(user => user.UserRoles).Where(user => user.Id == id);
      else Query.Include(u => u.Profiles).Where(user => user.Id == id);
   }
   public UsersSpecification(IEnumerable<string> ids, bool includeRoles = false)
   {
      if (includeRoles) Query.Include(user => user.Profiles).Include(user => user.UserRoles).Where(user => ids.Contains(user.Id));
      else Query.Include(u => u.Profiles).Where(user => ids.Contains(user.Id));
   }
}

public class UsersFetchByPhoneSpecification : Specification<User>
{
   public UsersFetchByPhoneSpecification(string phone, bool includeRoles = false)
   {
      if (includeRoles) Query.Include(user => user.Profiles).Include(user => user.UserRoles).Where(user => user.PhoneNumber == phone);
      else Query.Include(u => u.Profiles).Where(user => user.PhoneNumber == phone);
   }
}