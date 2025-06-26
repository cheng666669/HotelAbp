using HotelABP.DTos.ReserveRooms;
using HotelABP.RoomReserves;
using HotelABP.RoomTypes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HotelABP.ReserveRooms
{
    public interface IReserveRoomServer:IApplicationService
    {
        Task<ApiResult> AddReserveRoom(CreateRoom room);
        Task<ApiResult<PageResult<ReserveRoomShowDto>>> ShowReserveRoom([FromQuery] SearchTiao search1, [FromQuery] Seach seach);
    }
}
