using AutoMapper;
using HotelABP.Account;
using HotelABP.Users;

namespace HotelABP;

public class HotelABPApplicationAutoMapperProfile : Profile
{
    public HotelABPApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        CreateMap<AccountRoleDto,SysUser>().ReverseMap();
        CreateMap<RoleDto,Roles>().ReverseMap();
    }
}
