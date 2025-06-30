using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace HotelABP.Customer
{
    /// <summary>
    /// 客户服务
    /// </summary>
    public interface ICustomerServices : IApplicationService
    {
        Task<ApiResult<CustomerDto>> AddCustomerAsync (CustomerDto cudto);
        Task<ApiResult<PageResult<GetCustomerDto>>> GetCustomerListAsync(Seach seach, GetCustomerDtoList cudto);
        Task<ApiResult<List<GetCustoimerTypeNameDto>>> GetCustoimerTypeNameAsync();
        Task<ApiResult<bool>> UpdateCustomerAsync (UpCustomerDto customerDto);
      
    }
}
