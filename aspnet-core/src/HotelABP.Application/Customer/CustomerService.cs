using HotelABP.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using static Volo.Abp.Http.MimeTypes;

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

        public async Task<ApiResult<PageResult<List<CustomerDto>> >>GetCustomerListAsync(Seach seach, GetCustomerDtoList cudto)
        {
            var list =await _customerRepository.GetQueryableAsync();
            list = list.WhereIf(!string.IsNullOrEmpty(cudto.CustomerNickName), x => x.CustomerNickName == cudto.CustomerNickName);
            list = list.WhereIf(cudto.CustomerType >= 0, x => x.CustomerType == cudto.CustomerType);
            list = list.WhereIf(!string.IsNullOrEmpty(cudto.CustomerName), x => x.CustomerName == cudto.CustomerName);
            list = list.WhereIf(!string.IsNullOrEmpty(cudto.PhoneNumber), x => x.PhoneNumber == cudto.PhoneNumber);
            list = list.WhereIf(cudto.Gender >= 0, x => x.Gender == cudto.Gender);
            var startTime = cudto.StartTime?.Date;
            var endTime = cudto.EndTime?.Date.AddDays(1);
            list = list.WhereIf(cudto.StartTime != null, x => x.Birthday >= cudto.StartTime);
            list = list.WhereIf(cudto.EndTime != null, x => x.Birthday < cudto.EndTime.Value.AddDays(1));
            var res = list.PageResult(seach.PageIndex, seach.PageSize);
            var dto = ObjectMapper.Map<List<HotelABPCustoimers>, List<CustomerDto>>(list.ToList());
            return ApiResult<PageResult<List<CustomerDto>>>.Success(
                new PageResult<List<CustomerDto>>
            {
                Data = dto,
                TotleCount = list.Count(),
                 TotlePage = (int)Math.Ceiling(list.Count() / (double)seach.PageSize)

            },
            ResultCode.Success
            );
        }
    }
}
