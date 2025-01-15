using Infrastructure.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class ProfilesMappingProfile : Profile
{
	public ProfilesMappingProfile()
	{
		CreateMap<Profiles, ProfilesViewModel>();

		CreateMap<ProfilesViewModel, Profiles>();
	}
}

