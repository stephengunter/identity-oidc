using Infrastructure.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;
public class RoleMappingProfile : Profile
{
   public RoleMappingProfile()
   {
      CreateMap<Role, RoleViewModel>();

      CreateMap<RoleViewModel, Role>();
   }
}

