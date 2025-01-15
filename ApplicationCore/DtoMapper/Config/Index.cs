using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class MappingConfig
{
	public static MapperConfiguration CreateConfiguration()
	{
		return new MapperConfiguration(cfg => {
			cfg.AddMaps(typeof(UserMappingProfile).Assembly);
		});
	}

}


