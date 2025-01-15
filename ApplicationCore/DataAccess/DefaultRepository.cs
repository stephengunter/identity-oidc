using Ardalis.Specification.EntityFrameworkCore;
using Infrastructure.Interfaces;

namespace ApplicationCore.DataAccess;
public interface IDefaultRepository<T> : IRepository<T>, IReadRepository<T> where T : class, IAggregateRoot
{
	
}
public class DefaultRepository<T> : RepositoryBase<T>, IDefaultRepository<T> where T : class, IAggregateRoot
{
	public DefaultRepository(DefaultContext dbContext) : base(dbContext)
	{
		
	}

}
