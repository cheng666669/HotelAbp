using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HotelABP.RoomTypes
{
    public interface IRoomTypeService : IApplicationService
    {
       
        Task<ApiResult<RoomTypeDto>> CreateAdd(CreateUpdateRoomTypeDto input);
        Task<ApiResult<PageResult<RoomTypeDto>>> GetListShow(Seach seach, GetRoomTypeDto dto);
        Task<ApiResult<bool>> DeleteRoomTypeDel(Guid id);
        Task<ApiResult<bool>> DeleteBatchRoomType(List<Guid> ids);
        Task<ApiResult<RoomTypeDto>> UpdateRoomType(Guid id, CreateUpdateRoomTypeDto input);
        Task<ApiResult> UpdateRoomTypeOrder(UpdataRoomTypeOrderDto dto);

    }
}
