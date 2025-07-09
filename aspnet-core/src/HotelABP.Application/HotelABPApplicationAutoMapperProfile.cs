using AutoMapper;
using HotelABP.Account;
using HotelABP.Customer;
using HotelABP.Customers;
using HotelABP.DTos.ReserveRooms;
using HotelABP.Label;
using HotelABP.Labels;
using HotelABP.Role;
using HotelABP.RoomNummbers;
using HotelABP.RoomReserves;
using HotelABP.RoomTypes;
using HotelABP.Users;
using HotelABP.RoomTypes.Types;

namespace HotelABP;

public class HotelABPApplicationAutoMapperProfile : Profile
{
    public HotelABPApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        CreateMap<CustomerDto, HotelABPCustoimerss>().ReverseMap();
        CreateMap<HotelABPCustoimerss, GetCustomerDtoList>().ReverseMap();
        CreateMap<AccountRoleDto,SysUser>().ReverseMap();
        CreateMap<AccountDto, SysUser>().ReverseMap();
        CreateMap<RoleDto,Roles>().ReverseMap();
        CreateMap<CreateUpdateRoleDto, Roles>().ReverseMap();
        CreateMap<GetAccountResultDTO, SysUser>().ReverseMap();
        CreateMap<GetRoleResultDTO, Roles>().ReverseMap();
        CreateMap<ReserveRoom,ReserveRoomDto>().ReverseMap();
        CreateMap<ReserveRoom,CreateRoom>().ReverseMap();
        CreateMap<ReserveRoomShowDto, ReserveRoom>().ReverseMap();
        CreateMap<RoomType, RoomTypeDto>().ReverseMap();
        CreateMap<CreateUpdateRoomTypeDto, RoomType>().ReverseMap();
        CreateMap<RoomNummber, RoomNummDto>().ReverseMap();
        CreateMap<CreateUpdataRoomNummDto, RoomNummber>().ReverseMap();

        CreateMap<UpCustomerDto,HotelABPCustoimerss>().ReverseMap();
        CreateMap<LabelDto,HotelABPLabelss>().ReverseMap();
        CreateMap<GetLabeDtoList, HotelABPLabelss>().ReverseMap();
        CreateMap<GetLabelDto, HotelABPLabelss>().ReverseMap();

        CreateMap<MoneyDetail, MoneyDetailDto>().ReverseMap();
        CreateMap<HotelABPCustoimerss, FanCustomerDto>().ReverseMap();
    }
}
