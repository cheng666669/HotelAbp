using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HotelABP.RoomTypes
{
    public interface IRoomTypeService : IApplicationService
    {
        Task<ApiResult<RoomTypeDto>> CreateAsync(CreateUpdateRoomTypeDto input);
        Task<ApiResult<PageResult<List<RoomTypeDto>>>> GetListAsync(Seach seach, GetRoomTypeDto dto);
        Task<ApiResult<bool>> DeleteAsync(Guid id);
        Task<ApiResult<bool>> DeleteBatchAsync(List<Guid> ids);
        Task<ApiResult<RoomTypeDto>> UpdateAsync(Guid id, CreateUpdateRoomTypeDto input);
    }
}
