using HotelABP.RoomPriceCalendar;
using HotelABP.RoomTypes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HotelABP.RoomPriceCalendarService
{
    public interface IRoomPriceCalendarService: IApplicationService
    {
        Task<ApiResult<List<RoomTypeOrRoomPriceDto>>> GetListAsync(string? TypeName);
        Task<ApiResult<int>> AddRoomPriceAsync(CreateRoomPriceDto input);
        Task<ApiResult<List<RoomPriceCalendars>>> GetRoomPriceCalendarsAsync(GetRoomPriceCalendarsDto dto);
    }
}
