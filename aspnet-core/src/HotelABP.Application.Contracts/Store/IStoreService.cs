using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HotelABP.Store
{
    public interface IStoreService:IApplicationService
    {
        Task<ApiResult> CreateStore(CreateUpdateStoreDto dto);
        Task<ApiResult> UpdateStoreinfo(Guid id,CreateUpdateStoreDto dto);
        Task<ApiResult> UpdateStatus(Guid id);
        Task<ApiResult<PageResult<StoreResultDto>>> GetStorelist(Seach seach,SearchStoreDto storeDto);
    }
}
