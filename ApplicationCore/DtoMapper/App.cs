using Infrastructure.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class AppMappingProfile : Profile
{
	public AppMappingProfile()
	{
		CreateMap<App, AppViewModel>();

		CreateMap<AppViewModel, App>();
	}
}

