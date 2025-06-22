using HotelABP.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace HotelABP.Customer
{
    public class CustomerService : ApplicationService, ICustomerServices
    {
        private readonly IRepository<HotelABPCustoimers, Guid> _customerRepository;

        public CustomerService(IRepository<HotelABPCustoimers, Guid> customerRepository)
        {
            _customerRepository = customerRepository;
        }
        /// <summary>
        /// 添加客户信息
        /// </summary>
        /// <param name="cudto"></param>
        /// <returns></returns>
        public async Task<ApiResult<CustomerDto>> AddCustomerAsync(CustomerDto cudto)
        {
            try
            {
                var  entity = ObjectMapper.Map<CustomerDto, HotelABPCustoimers>(cudto);
                var  entitydto = await _customerRepository.InsertAsync(entity);
                var s = ObjectMapper.Map<HotelABPCustoimers, CustomerDto>(entitydto);
                return ApiResult<CustomerDto>.Success(s, ResultCode.Success);
            }
            catch (Exception ex)
            {

                return ApiResult<CustomerDto>.Fail(ex.Message, ResultCode.Error);
            }
        }
    }
}
