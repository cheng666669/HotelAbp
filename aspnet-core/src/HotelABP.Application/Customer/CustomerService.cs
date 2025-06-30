using AutoMapper;
using HotelABP.Customers;
using HotelABP.Export;
using HotelABP.RoomTypes;
using Microsoft.AspNetCore.Http;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Validation;
using static Volo.Abp.Http.MimeTypes;

namespace HotelABP.Customer
{
    public class CustomerService : ApplicationService, ICustomerServices
    {
        private readonly IRepository<HotelABPCustoimers, Guid> _customerRepository;
        private readonly IRepository<HotelABPCustoimerTypeName, Guid> _customerTypeRepository;
        // 声明一个只读字段来保存我们封装的导出服务实例
        private readonly IExportAppService _exportAppService;
   
   
        public CustomerService(IRepository<HotelABPCustoimers, Guid> customerRepository, IRepository<HotelABPCustoimerTypeName, Guid> customerTypeRepository, IExportAppService exportAppService)
        {
            _customerRepository = customerRepository;
            _customerTypeRepository = customerTypeRepository;
            _exportAppService = exportAppService;
           
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
                var entity = ObjectMapper.Map<CustomerDto, HotelABPCustoimers>(cudto);
                var entitydto = await _customerRepository.InsertAsync(entity);
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
            list = list.WhereIf(!string.IsNullOrEmpty(cudto.CustomerNickName), x => x.CustomerNickName.Contains(cudto.CustomerNickName));
            list = list.WhereIf(cudto.CustomerType != null, x => x.CustomerType == cudto.CustomerType);
            list = list.WhereIf(!string.IsNullOrEmpty(cudto.CustomerName), x => x.CustomerName.Contains(cudto.CustomerName));
            list = list.WhereIf(!string.IsNullOrEmpty(cudto.PhoneNumber), x => x.PhoneNumber.Contains(cudto.PhoneNumber));
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
        /// <summary>
        ///  修改客户信息（批量）
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="customerDto"></param>
        /// <returns></returns>
        public async Task<ApiResult<bool>> UpdateCustomerAsync(UpCustomerDto customerDto)
        {
            try
            {
                foreach (var id in customerDto.ids)
                {
                    var entity = await _customerRepository.GetAsync(id);
                    // 将 customerDto 的属性映射到 entity
                    ObjectMapper.Map(customerDto, entity);
                    await _customerRepository.UpdateAsync(entity);
                }
                return ApiResult<bool>.Success(true, ResultCode.Success);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message, ResultCode.Error);
            }
        }
        /// <summary>
        /// 导出所有客户数据
        /// </summary>
        /// <returns></returns>
        public async Task<IRemoteStreamContent> ExportAllCustomersAsync()
        {
            // ... 获取数据的代码 ...
            var allCustomers = await _customerRepository.GetListAsync();

            var exportData = new ExportDataDto<HotelABPCustoimers>
            {
                FileName = "客户管理",
                Items = allCustomers,
                ColumnMappings = new Dictionary<string, string>
        {
            { "Id", "客户ID" },
            { "CustomerNickName", "客户昵称" },
            { "CustomerType", "客户类型" },
            { "CustomerName", "客户姓名" },
            { "PhoneNumber", "手机号" },
            { "Gender", "性别" },
            { "Birthday", "出生日期" },
            { "City", "所在城市" },
            { "Address", "详细地址" },
            { "GrowthValue", "成长值" },
            { "AvailableBalance", "可用充值余额" },
            { "AvailableGiftBalance", "可用赠送余额" },
            { "AvailablePoints", "可用积分" }
        }
            };

            // 直接调用并返回 IRemoteStreamContent
            return await _exportAppService.ExportToExcelAsync(exportData);
        }

       

    }
  
}

