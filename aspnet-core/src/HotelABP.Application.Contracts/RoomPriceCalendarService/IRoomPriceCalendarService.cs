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
        Task<ApiResult<int>> AddRoomPriceaddAsync(CreateRoomPriceDto input);
        Task<ApiResult<int>> UpdateRoomPriceupdateAsync(UpdateRoomPriceDto input);
        Task<ApiResult<List<RoomPriceCalendars>>> GetRoomPriceCalendarsAsync(GetRoomPriceCalendarsDto dto);
        Task<ApiResult<bool>> UpdateRoomPriceState(Guid id, bool CalendarStatus);
        Task<ApiResult<bool>> DeleteRoomPrice(Guid id);
        Task<ApiResult<bool>> UpdateRoomPriceCalendarsAsync(UpdateRoomPriceCalendarsDto dto);
        Task<ApiResult<int>> UpdateRoomPriceSort(Guid id, int sort);
    }
}
