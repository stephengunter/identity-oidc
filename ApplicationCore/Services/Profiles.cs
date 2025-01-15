using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services;

public interface IProfilesService
{
   Task<IEnumerable<Profiles>> FetchAsync();
   Task<Profiles?> FindAsync(User user);
   Task<Profiles> CreateAsync(Profiles profiles);
	Task UpdateAsync(Profiles profiles);
   Task DeleteAsync(Profiles profiles);
}

public class ProfilesService : IProfilesService
{
	private readonly IDefaultRepository<Profiles> _profilesRepository;

	public ProfilesService(IDefaultRepository<Profiles> profilesRepository)
	{
      _profilesRepository = profilesRepository;
	}
   public async Task<IEnumerable<Profiles>> FetchAsync()
      => await _profilesRepository.ListAsync();
   public async Task<Profiles?> FindAsync(User user)
      => await _profilesRepository.FirstOrDefaultAsync(new ProfilesSpecification(user));

   public async Task<Profiles> CreateAsync(Profiles profiles)
		=> await _profilesRepository.AddAsync(profiles);

	public async Task UpdateAsync(Profiles profiles)
		=> await _profilesRepository.UpdateAsync(profiles);

   public async Task DeleteAsync(Profiles profiles)
      => await _profilesRepository.DeleteAsync(profiles);

}
