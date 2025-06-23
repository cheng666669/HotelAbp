using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HotelABP.Customer
{
    public interface ICustomerServices : IApplicationService
    {
        Task<ApiResult<CustomerDto>> AddCustomerAsync (CustomerDto cudto);
        Task<ApiResult<PageResult<List<CustomerDto>>>> GetCustomerListAsync(Seach seach, GetCustomerDtoList cudto);
    }
}
