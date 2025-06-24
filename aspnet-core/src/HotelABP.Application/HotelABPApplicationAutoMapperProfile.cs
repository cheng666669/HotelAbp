using AutoMapper;
using HotelABP.Account;
using HotelABP.Users;
using HotelABP.RoomNummbers;
using HotelABP.RoomTypes;

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
        CreateMap<RoomType, RoomTypeDto>().ReverseMap();
        CreateMap<CreateUpdateRoomTypeDto, RoomType>().ReverseMap();

        CreateMap<RoomNummber, RoomNummDto>().ReverseMap();
        CreateMap<CreateUpdataRoomNummDto, RoomNummber>().ReverseMap();
    }
}
