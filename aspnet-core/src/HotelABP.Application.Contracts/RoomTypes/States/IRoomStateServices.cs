using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HotelABP.RoomTypes.States
{
    public interface IRoomStateServices:IApplicationService
    {
        Task<ApiResult<RoomTypeOrReserveRoomDto>> UpdateRoomTypeState(Guid id, int state);
        Task<ApiResult<List<RoomTypeOrRoomNumGroupDto>>> GetRoomTypeList(RoomTypeOrRoomStateSearchDto SearchDto);
    }
}
