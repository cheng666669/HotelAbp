using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HotelABP.RoomNummbers
{
    public interface IRoomNummberService:IApplicationService
    {
        Task<ApiResult<RoomNummDto>> CreateAsync(CreateUpdataRoomNummDto input);

        Task<ApiResult<PageResult<List<RoomNummDto>>>> GetListToRoomTypeIdAsync(Seach seach, RoomNummRoomTypeRequestDto input);
        Task<ApiResult<bool>> DeleteBatchAsync(List<Guid> ids);
        Task<ApiResult<bool>> UpdateStateToRoomNumAsync(Guid id);

    }
} 