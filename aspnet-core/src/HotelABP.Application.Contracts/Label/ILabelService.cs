using HotelABP.Customer;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HotelABP.Label
{
    public interface ILabelService :IApplicationService
    {
        Task<ApiResult<LabelDto>> AddLabelAsync(LabelDto ldto);
        Task<ApiResult<PageResult<GetLabelDto>>> GetCustomerListAsync(Seach seach, GetLabeDtoList dtoList);
        Task<ApiResult> DelLabelAsync(Guid guid);
        Task<ApiResult> UpdateLabelAsync(Guid guid,LabelDto ldto);
        Task<ApiResult<FanLabelDto>>  GetLabelByIdAsync(Guid id);
    }
}
