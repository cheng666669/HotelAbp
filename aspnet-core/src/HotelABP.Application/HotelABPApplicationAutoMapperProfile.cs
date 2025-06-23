using AutoMapper;
using HotelABP.DTos.ReserveRooms;
using HotelABP.RoomReserves;

namespace HotelABP;

public class HotelABPApplicationAutoMapperProfile : Profile
{
    public HotelABPApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        CreateMap<ReserveRoom,ReserveRoomDto>().ReverseMap();
        CreateMap<ReserveRoomShowDto, ReserveRoom>().ReverseMap();
    }
}
