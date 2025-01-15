using Ardalis.Specification;
using ApplicationCore.Models;

namespace ApplicationCore.Specifications;
public class ProfilesSpecification : Specification<Profiles>
{
   public ProfilesSpecification(User user)
   {
      Query.Where(p => p.UserId == user.Id);
   }
}

