﻿using AutoMapper;
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
using HotelABP.RoomTypes.Types;

using HotelABP.RoomPriceCalendarService;
using HotelABP.RoomPriceCalendar;

using HotelABP.Store;
using HotelABP.Users;


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
        CreateMap<StoreInfo, StoreResultDto>().ReverseMap();
        CreateMap<MoneyDetail, MoneyDetailDto>().ReverseMap();
        CreateMap<HotelABPCustoimerss, GetCustomerDto>().ReverseMap();

        // 房间价格日历
        CreateMap<RoomPrice, RoomTypeOrRoomPriceDto>().ReverseMap();
        CreateMap<RoomPrice,CreateRoomPriceDto>().ReverseMap();


        CreateMap<CreateUpdateStoreDto, StoreInfo>().ReverseMap();
        CreateMap<HotelABPCustoimerss, FanCustomerDto>().ReverseMap();
        CreateMap<HotelABPLabelss, FanLabelDto>().ReverseMap();

    }
}
