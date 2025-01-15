using Ardalis.Specification;
using ApplicationCore.Models;

namespace ApplicationCore.Specifications;
public class RolesIdSpecification : Specification<Role>
{
   public RolesIdSpecification(ICollection<string> ids)
   {
      Query.Where(x => ids.Contains(x.Id));
   }
}