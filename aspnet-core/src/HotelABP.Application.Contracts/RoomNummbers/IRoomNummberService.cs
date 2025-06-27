using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HotelABP.RoomNummbers
{
    public interface IRoomNummberService:IApplicationService
    {
        Task<ApiResult<RoomNummDto>> CreateRoomNumAdd(CreateUpdataRoomNummDto input);

        Task<ApiResult<PageResult<RoomNummDto>>> GetListToRoomTypeId(Seach seach, RoomNummRoomTypeRequestDto input);
        Task<ApiResult<bool>> DeleteRoomNumBatch(List<Guid> ids);
        Task<ApiResult<bool>> UpdateStateToRoomNum(Guid id);
        Task<ApiResult<int>> UpdateRoomNumm(Guid Id, CreateUpdataRoomNummDto input);

    }
} 