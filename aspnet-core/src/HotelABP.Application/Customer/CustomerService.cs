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
        private readonly IRepository<HotelABPCustoimerTypeName, Guid> _customerTypeRepository;

        public CustomerService(IRepository<HotelABPCustoimers, Guid> customerRepository, IRepository<HotelABPCustoimerTypeName, Guid> customerTypeRepository)
        {
            _customerRepository = customerRepository;
            _customerTypeRepository = customerTypeRepository;
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
        /// <summary>
        /// 获取客户类型列表
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResult<List<GetCustoimerTypeNameDto>>> GetCustoimerTypeNameAsync()
        {
            var list = await _customerTypeRepository.GetQueryableAsync();
            var result = list.Select(x => new GetCustoimerTypeNameDto
            {
                Id = x.Id,
                CustomerTypeName = x.CustomerTypeName
            }).ToList();

            return ApiResult<List<GetCustoimerTypeNameDto>>.Success(result, ResultCode.Success);
        }
        /// <summary>
        /// 获取客户列表
        /// </summary>
        /// <param name="seach"></param>
        /// <param name="cudto"></param>
        /// <returns></returns>
        public async Task<ApiResult<PageResult<GetCustomerDto>>> GetCustomerListAsync(Seach seach, GetCustomerDtoList cudto)
        {
            var list = await _customerRepository.GetQueryableAsync();
            var types = await _customerTypeRepository.GetQueryableAsync();
            list = list.WhereIf(!string.IsNullOrEmpty(cudto.CustomerNickName), x => x.CustomerNickName == cudto.CustomerNickName);
            list = list.WhereIf(cudto.CustomerType != null, x => x.CustomerType == cudto.CustomerType);
            list = list.WhereIf(!string.IsNullOrEmpty(cudto.CustomerName), x => x.CustomerName == cudto.CustomerName);
            list = list.WhereIf(!string.IsNullOrEmpty(cudto.PhoneNumber), x => x.PhoneNumber == cudto.PhoneNumber);
            list = list.WhereIf(cudto.Gender >= 0, x => x.Gender == cudto.Gender);
            var startTime = cudto.StartTime?.Date;
            var endTime = cudto.EndTime?.Date.AddDays(1);
            list = list.WhereIf(cudto.StartTime != null, x => x.Birthday >= cudto.StartTime);
            list = list.WhereIf(cudto.EndTime != null, x => x.Birthday < cudto.EndTime.Value.AddDays(1));

            var type = from a in list
                       join b in types
                       on a.CustomerType equals b.Id into temp
                       from b in temp.DefaultIfEmpty()
                       select new GetCustomerDto
                       {
                           Id = a.Id,
                           CustomerNickName = a.CustomerNickName,
                           CustomerType = a.CustomerType,
                           CustomerTypeName = b.CustomerTypeName,
                           Gender = a.Gender,
                           CustomerName = a.CustomerName,
                           PhoneNumber = a.PhoneNumber,
                           Birthday = a.Birthday,
                           Address = a.Address,
                           City = a.City,
                           GrowthValue = a.GrowthValue,
                           AvailableBalance = a.AvailableBalance,
                           AvailableGiftBalance = a.AvailableGiftBalance,
                           AvailablePoints = a.AvailablePoints

                       };


            var res = type.AsQueryable().PageResult(seach.PageIndex, seach.PageSize);
            
            return ApiResult<PageResult<GetCustomerDto>>.Success(
                new PageResult<GetCustomerDto>
            {
                Data = res.Queryable.ToList(),
                TotleCount = list.Count(),
                 TotlePage = (int)Math.Ceiling(list.Count() / (double)seach.PageSize)

            },
            ResultCode.Success
            );
        }
    }
}
