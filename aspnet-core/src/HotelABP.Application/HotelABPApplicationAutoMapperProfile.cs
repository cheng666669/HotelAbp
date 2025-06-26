using AutoMapper;
using HotelABP.DTos.ReserveRooms;
using HotelABP.RoomReserves;
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
        CreateMap<ReserveRoom,ReserveRoomDto>().ReverseMap();
        CreateMap<ReserveRoom,CreateRoom>().ReverseMap();
        CreateMap<ReserveRoomShowDto, ReserveRoom>().ReverseMap();
        CreateMap<RoomType, RoomTypeDto>().ReverseMap();
        CreateMap<CreateUpdateRoomTypeDto, RoomType>().ReverseMap();

        CreateMap<RoomNummber, RoomNummDto>().ReverseMap();
        CreateMap<CreateUpdataRoomNummDto, RoomNummber>().ReverseMap();
    }
}
