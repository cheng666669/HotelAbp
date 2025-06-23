using HotelABP.DTos.ReserveRooms;
using HotelABP.RoomReserves;
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
        Task<ApiResult> ResuAdd(ReserveRoomDto room);
        Task<ApiResult<PageResult<ReserveRoomShowDto>>> ReserveRoomShow([FromQuery] SearchTiao search1, [FromQuery] Seach seach);
    }
}
