using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        /// <summary>
        /// 添加客户
        /// </summary>
        Task<ApiResult<CustomerDto>> AddCustomerAsync (CustomerDto cudto);
        /// <summary>
        /// 获取客户列表
        /// </summary>
        /// <param name="seach">分页</param>
        /// <param name="cudto">>包含客户列表查询相关信息的DTO，用于指定客户列表查询相或具体客户信息</param>
        /// <returns></returns>
        Task<ApiResult<PageResult<GetCustomerDto>>> GetCustomerListAsync(Seach seach, GetCustomerDtoList cudto);
        /// <summary>
        /// 获取客户类型列表
        /// </summary>
        Task<ApiResult<List<GetCustoimerTypeNameDto>>> GetCustoimerTypeNameAsync();
        /// <summary>
        /// 更新客户信息
        /// </summary>
        /// <param name="customerDto">包含更新客户信息相关信息的DTO</param>
        /// <returns></returns>
        Task<ApiResult<bool>> UpdateCustomerAsync (UpCustomerDto customerDto);
        /// <summary>
        /// 充值
        /// </summary>
        /// <param name="balanceDto">包含更新客户信息相关信息的DTO</param>
        /// <returns></returns>
        Task<ApiResult<bool>> UpAvailableBalance(UpAvailableBalanceDto balanceDto);
        /// <summary>
        /// 消费
        /// </summary>
        /// <param name="balanceDto">包含可用余额相关信息的DTO，用于指定消费的账户或具体余额信息</param>
        /// <returns>操作成功返回 true，否则返回 false。</returns>
        Task<ApiResult<bool>> UpSumofconsumption(UpSumofconsumptionDto sumofconsumptionDto);
        /// <summary>
        /// 修改会员状态
        /// </summary>
        /// <param name="upStautsdto"></param>
        /// <returns></returns>
        Task<ApiResult<bool>> UpdateCustomerStatusAsync(UpStautsDto upStautsdto);
    }
}
