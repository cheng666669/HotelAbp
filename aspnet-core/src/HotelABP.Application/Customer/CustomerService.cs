using HotelABP.Customers;
using HotelABP.Export;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Validation;
using static Volo.Abp.Http.MimeTypes;
using System.Transactions;
using HotelABP.Labels;




namespace HotelABP.Customer
{
    /// <summary>
    /// 客户管理
    /// </summary>
    [ApiExplorerSettings(GroupName = "customer")]
    public class CustomerService : ApplicationService, ICustomerServices
    {
        private readonly IRepository<HotelABPCustoimerss, Guid> _customerRepository;
        private readonly IRepository<HotelABPCustoimerTypeName, Guid> _customerTypeRepository;
        private readonly IRepository<HotelABPLabelss, Guid> _labelRepository;
        // 声明一个只读字段来保存我们封装的导出服务实例
        private readonly IExportAppService _exportAppService;
        // 声明一个只读字段来保存导入服务实例


        public CustomerService(
            IRepository<HotelABPCustoimerss, Guid> customerRepository,
            IRepository<HotelABPCustoimerTypeName, Guid> customerTypeRepository,
            IRepository<HotelABPLabelss, Guid> labelRepository,
            IExportAppService exportAppService)
        {
            _customerRepository = customerRepository;
            _customerTypeRepository = customerTypeRepository;
            _labelRepository = labelRepository;
            _exportAppService = exportAppService;
        }
        /// <summary>
        /// 添加客户信息（支持DTO映射和异常处理）
        /// </summary>
        /// <param name="cudto">客户DTO</param>
        /// <returns>插入后的客户DTO</returns>
        /// <remarks>
        /// 1. 使用ObjectMapper将输入DTO映射为实体。
        /// 2. 插入数据库。
        /// 3. 将插入后的实体再次映射为DTO。
        /// 4. 返回带有插入结果的ApiResult。
        /// 5. 捕获异常并返回失败信息。
        /// </remarks>
        public async Task<ApiResult<CustomerDto>> AddCustomerAsync(CustomerDto cudto)
        {
            try
            {
                // 将 CustomerDto 映射为 HotelABPCustoimers 实体对象
                var entity = ObjectMapper.Map<CustomerDto, HotelABPCustoimerss>(cudto);
                // 插入实体对象到数据库，并返回插入后的实体
                var entitydto = await _customerRepository.InsertAsync(entity);
                // 将插入后的实体对象再次映射为 CustomerDto
                var s = ObjectMapper.Map<HotelABPCustoimerss, CustomerDto>(entitydto);
                // 返回带有插入结果的 ApiResult
                return ApiResult<CustomerDto>.Success(s, ResultCode.Success);
            }
            catch (Exception ex)
            {
                return ApiResult<CustomerDto>.Fail(ex.Message, ResultCode.Error);
            }
        }
        /// <summary>
        /// 获取客户类型列表（支持DTO组装）
        /// </summary>
        /// <returns>客户类型DTO列表</returns>
        /// <remarks>
        /// 1. 查询所有客户类型。
        /// 2. 组装为DTO列表。
        /// </remarks>
        public async Task<ApiResult<List<GetCustoimerTypeNameDto>>> GetCustomerTypeNameAsync()
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
        /// 获取客户列表（支持多条件筛选、分页、DTO组装）
        /// </summary>
        /// <param name="seach">分页参数</param>
        /// <param name="cudto">筛选条件DTO</param>
        /// <returns>分页后的客户DTO列表</returns>
        /// <remarks>
        /// 1. 查询所有客户。
        /// 2. 多条件筛选（昵称、类型、姓名、手机号、性别、生日区间）。
        /// 3. 联表查询客户类型。
        /// 4. DTO组装。
        /// 5. 分页返回。
        /// </remarks>
        public async Task<ApiResult<PageResult<GetCustomerDto>>> GetCustomerListAsync(Seach seach, GetCustomerDtoList cudto)
        {
            var list = await _customerRepository.GetQueryableAsync();
            var types = await _customerTypeRepository.GetQueryableAsync();
            var labels = await _labelRepository.GetQueryableAsync();
            // 多条件筛选
            list = list.WhereIf(!string.IsNullOrEmpty(cudto.CustomerNickName), x => x.CustomerNickName.Contains(cudto.CustomerNickName));
            list = list.WhereIf(cudto.CustomerType != null, x => x.CustomerType == cudto.CustomerType);
            // Guid模糊查询
            list = list.WhereIf(cudto.Id != null, x => x.Id.ToString().Contains(cudto.Id.ToString()));
     
            list = list.WhereIf(!string.IsNullOrEmpty(cudto.CustomerName), x => x.CustomerName.Contains(cudto.CustomerName));
            list = list.WhereIf(!string.IsNullOrEmpty(cudto.PhoneNumber), x => x.PhoneNumber.Contains(cudto.PhoneNumber));
            // 性别判断逻辑
            list = list.WhereIf(cudto.Gender.HasValue && cudto.Gender >= 0, x => x.Gender == cudto.Gender);
           
            var startTime = cudto.StartTime?.Date;
            var endTime = cudto.EndTime?.Date.AddDays(1);
            list = list.WhereIf(cudto.StartTime != null, x => x.Birthday >= cudto.StartTime);
            list = list.WhereIf(cudto.EndTime != null, x => x.Birthday < cudto.EndTime.Value.AddDays(1));
            // 联表查询客户类型，组装DTO
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
                           AvailablePoints = a.AvailablePoints,
                           Status = a.Status,
                           Sumofconsumption = a.Sumofconsumption,
                           ComsumerNumber = a.ComsumerNumber,
                           Accumulativeconsumption = a.Accumulativeconsumption,
                           ConsumerDesc = a.ConsumerDesc,
                           CreationTime = a.CreationTime,
                           CustomerDesc = a.CustomerDesc,
                           Rechargeamount = a.Rechargeamount,
                         
                       };



            var res = type.PageResult(seach.PageIndex, seach.PageSize);

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
        /// 修改客户信息（批量，支持DTO映射和异常处理）
        /// </summary>
        /// <param name="customerDto">批量修改DTO，包含ID集合和要修改的字段</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 1. 遍历ID集合，依次查找客户。
        /// 2. DTO映射到实体。
        /// 3. 更新数据库。
        /// 4. 捕获异常并返回失败信息。
        /// </remarks>
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
        /// 导出所有客户数据（支持DTO组装、字段映射、调用导出服务）
        /// </summary>
        /// <returns>Excel流内容</returns>
        /// <remarks>
        /// 1. 查询所有客户和客户类型。
        /// 2. 联表组装DTO。
        /// 3. 构造导出数据结构，设置字段映射。
        /// 4. 调用导出服务生成Excel流。
        /// </remarks>
        public async Task<IRemoteStreamContent> ExportAllCustomersAsync()
        {
            // 获取数据
            var list = await _customerRepository.GetListAsync();
            var types = await _customerTypeRepository.GetListAsync();
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
                           AvailablePoints = a.AvailablePoints,
                           Rechargeamount = a.Rechargeamount,
                           Sumofconsumption = a.Sumofconsumption,
                           ComsumerNumber = a.ComsumerNumber,
                           CustomerDesc = a.CustomerDesc,
                           Status = a.Status,
                           ConsumerDesc = a.ConsumerDesc,
                           Accumulativeconsumption = a.Accumulativeconsumption

                       };
            // 构造导出数据结构
            var exportData = new ExportDataDto<GetCustomerDto>
            {
                FileName = "客户管理",
                Items = type.ToList(), // 将查询结果转换为列表并赋值给 Items
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
            // 返回导出的内容
            return await _exportAppService.ExportToExcelAsync(exportData);
        }
        /// <summary>
        /// 更新客户可用余额（支持参数校验、异常处理）
        /// </summary>
        /// <param name="balanceDto">余额变更DTO</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 1. 校验参数。
        /// 2. 查找客户。
        /// 3. 增加可用余额，记录备注。
        /// 4. 更新数据库。
        /// 5. 捕获异常并返回失败信息。
        /// </remarks>
        public async Task<ApiResult<bool>> UpAvailableBalance(UpAvailableBalanceDto balanceDto)
        {
            try
            {
                // 1. 校验参数
                if (balanceDto == null || balanceDto.Id == Guid.Empty || balanceDto.Rechargeamount < 0)
                {
                    return ApiResult<bool>.Fail("参数无效", ResultCode.Error);
                }

                // 2. 获取客户实体
                var customer = await _customerRepository.GetAsync(balanceDto.Id);
                if (customer == null)
                {
                    return ApiResult<bool>.Fail("客户不存在", ResultCode.Error);
                }
                // 4. 更新客户的可用余额
                customer.AvailableBalance += balanceDto.Rechargeamount;
                customer.CustomerDesc = balanceDto.CustomerDesc;

                // 5. 更新数据库
                await _customerRepository.UpdateAsync(customer);

                return ApiResult<bool>.Success(true, ResultCode.Success);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message, ResultCode.Error);
            }
        }
        /// <summary>
        /// 更新客户累计消费（支持余额扣减、赠送余额优先级、消费次数累计、异常处理）
        /// </summary>
        /// <param name="sumofconsumptionDto">消费变更DTO</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 1. 校验参数。
        /// 2. 查找客户。
        /// 3. 判断余额是否足够，优先扣减可用余额，不足时扣赠送余额。
        /// 4. 增加累计消费金额和消费次数，记录备注。
        /// 5. 更新数据库。
        /// 6. 捕获异常并返回失败信息。
        /// </remarks>
        public async Task<ApiResult<bool>> UpSumofconsumption(UpSumofconsumptionDto sumofconsumptionDto)
        {
            try
            {
                // 1. 校验参数
                if (sumofconsumptionDto == null || sumofconsumptionDto.Id == Guid.Empty || sumofconsumptionDto.Sumofconsumption == null || sumofconsumptionDto.Sumofconsumption <= 0)
                {
                    return ApiResult<bool>.Fail("参数无效", ResultCode.Error);
                }

                // 2. 获取客户实体
                var customer = await _customerRepository.GetAsync(sumofconsumptionDto.Id);
                if (customer == null)
                {
                    return ApiResult<bool>.Fail("客户不存在", ResultCode.Error);
                }

                // 3. 判断余额是否足够
                decimal availableBalance = customer.AvailableBalance ?? 0;
                decimal availableGiftBalance = customer.AvailableGiftBalance ?? 0;
                decimal totalAvailable = availableBalance + availableGiftBalance;
                decimal consume = sumofconsumptionDto.Sumofconsumption.Value;

                if (totalAvailable < consume)
                {
                    return ApiResult<bool>.Fail("余额不足", ResultCode.Error);
                }

                // 优先扣减可用余额
                if (availableBalance >= consume)
                {
                    customer.AvailableBalance = availableBalance - consume;
                }
                else
                {
                    // 可用余额不足，先扣完可用余额，再扣赠送余额
                    decimal left = consume - availableBalance;
                    customer.AvailableBalance = 0;
                    customer.AvailableGiftBalance = availableGiftBalance - left;
                }

                // 4. 增加累计消费金额和消费次数
                customer.Accumulativeconsumption = (customer.Accumulativeconsumption ?? 0) + consume;
                customer.ComsumerNumber = (customer.ComsumerNumber ?? 0) + 1;
                customer.ConsumerDesc = sumofconsumptionDto.ConsumerDesc;

                // 5. 更新数据库
                await _customerRepository.UpdateAsync(customer);

                return ApiResult<bool>.Success(true, ResultCode.Success);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message, ResultCode.Error);
            }
        }

        /// <summary>
        /// 批量修改客户状态（带事务）
        /// </summary>
        /// <param name="upStautsdto">包含客户ID列表和目标状态的DTO</param>
        /// <returns>操作结果，全部成功返回true，否则返回错误信息</returns>
        public async Task<ApiResult<bool>> UpdateCustomerStatusAsync(UpStautsDto upStautsdto)
        {
            try
            {
                // 使用事务，确保批量操作的原子性（全部成功或全部失败）
                using (var tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // 遍历所有待更新的客户ID
                    foreach (var id in upStautsdto.ids)
                    {
                        // 获取客户实体
                        var entity = await _customerRepository.GetAsync(id);
                        if (entity != null)
                        {
                            // 设置客户状态为目标状态
                            entity.Status = upStautsdto.Status;
                            // 更新数据库
                            await _customerRepository.UpdateAsync(entity);
                        }
                    }
                    // 提交事务，所有操作成功才会真正写入数据库
                    tran.Complete();
                }
                // 返回成功结果
                return ApiResult<bool>.Success(true, ResultCode.Success);
            }
            catch (Exception ex)
            {
                // 捕获异常并返回失败结果，包含错误信息
                return ApiResult<bool>.Fail(ex.Message, ResultCode.Error);
            }
        }
        /// <summary>
        /// 更新客户积分（支持积分增减、累计积分记录、备注说明）
        /// </summary>
        /// <param name="upAvailable">积分变更DTO，包含客户ID、积分变动值和备注</param>
        /// <returns>操作结果，成功返回true，失败返回错误信息</returns>
        /// <remarks>
        /// 1. 校验参数有效性（ID不为空，积分变动值不为0）。
        /// 2. 查找客户实体，不存在则返回错误。
        /// 3. 更新客户可用积分（增加或减少）。
        /// 4. 校验积分变更后是否有效（不能为负数）。
        /// 5. 更新客户累计积分记录。
        /// 6. 记录积分变更备注。
        /// 7. 更新数据库。
        /// 8. 捕获异常并返回失败信息。
        /// </remarks>

        public async Task<ApiResult<bool>> UpdateAvailablePoints(UpAvailablePointsDto upAvailable)
        {
            try
            {
                // 1. 校验参数有效性：检查DTO对象是否为空、ID是否为空值、积分变动是否为0
                if (upAvailable == null || upAvailable.Id == Guid.Empty || upAvailable.Accumulativeintegral == 0)
                {
                    // 参数无效，返回失败结果
                    return ApiResult<bool>.Fail("参数无效", ResultCode.Error);
                }

                // 2. 获取客户实体：根据ID从数据库查找客户
                var customer = await _customerRepository.GetAsync(upAvailable.Id);
                // 如果客户不存在，返回失败结果
                if (customer == null)
                {
                    return ApiResult<bool>.Fail("客户不存在", ResultCode.Error);
                }

                // 3. 更新客户的可用积分：将传入的积分变动值加到客户当前积分上（可以是正数或负数）
                customer.AvailablePoints += upAvailable.Accumulativeintegral;

                // 4. 校验积分是否有效：确保更新后的积分不为负数
                if (customer.AvailablePoints < 0)
                {
                    // 积分不足，返回失败结果
                    return ApiResult<bool>.Fail("积分不足", ResultCode.Error);
                }


                // 5. 更新累计积分
                customer.Accumulativeintegral = (customer.Accumulativeintegral ?? 0) + upAvailable.Accumulativeintegral;

                // 6. 更新积分变更备注：记录本次积分变动的原因或说明
                customer.Pointsmodifydesc = upAvailable.Pointsmodifydesc;

                // 7. 更新数据库：将修改后的客户实体保存到数据库
                await _customerRepository.UpdateAsync(customer);

                // 操作成功，返回成功结果
                return ApiResult<bool>.Success(true, ResultCode.Success);
            }
            catch (Exception ex)
            {
                // 8. 捕获异常并返回失败信息：记录异常信息并返回给调用方
                return ApiResult<bool>.Fail(ex.Message, ResultCode.Error);
            }
        }

        /// <summary>
        /// 获取标签列表
        /// </summary>
        public async Task<ApiResult<List<LabelListDto>>> GetLabelListAsync()
        {
            try
            {
                var labels = await _labelRepository.GetListAsync();
                var result = labels.Select(l => new LabelListDto
                {
                    Id = l.Id,
                    LabelName = l.LabelName,
                    TagType = l.TagType,
                    PeopleNumber = l.PeopleNumber
                }).ToList();

                return ApiResult<List<LabelListDto>>.Success(result, ResultCode.Success);
            }
            catch (Exception ex)
            {
                return ApiResult<List<LabelListDto>>.Fail(ex.Message, ResultCode.Error);
            }
        }

        /// <summary>
        /// 为客户添加标签
        /// </summary>
        public async Task<ApiResult<bool>> AddCustomerLabelsAsync(CustomerLabelDto dto)
        {
            try
            {
                // 参数校验
                if (dto.CustomerIds == null || !dto.CustomerIds.Any() ||
                    dto.LabelIds == null || !dto.LabelIds.Any())
                {
                    return ApiResult<bool>.Fail("参数无效", ResultCode.Error);
                }

                // 验证标签是否存在
                var labels = await _labelRepository.GetListAsync(l => dto.LabelIds.Contains(l.Id));
                if (labels.Count != dto.LabelIds.Count)
                {
                    return ApiResult<bool>.Fail("部分标签不存在", ResultCode.Error);
                }

                // 获取客户列表
                var customers = await _customerRepository.GetListAsync(c => dto.CustomerIds.Contains(c.Id));

                // 更新每个客户的标签
                foreach (var customer in customers)
                {
                    // 如果客户当前没有标签，初始化为空字符串
                    if (string.IsNullOrEmpty(customer.CustomerLabel))
                    {
                        customer.CustomerLabel = "";
                    }

                    // 将现有标签转换为列表
                    var currentLabels = customer.CustomerLabel.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                     .Select(l => Guid.Parse(l))
                                                     .ToList();

                    // 添加新标签（避免重复）
                    foreach (var labelId in dto.LabelIds)
                    {
                        if (!currentLabels.Contains(labelId))
                        {
                            currentLabels.Add(labelId);
                        }
                    }

                    // 更新客户的标签字段
                    customer.CustomerLabel = string.Join(",", currentLabels);
                    await _customerRepository.UpdateAsync(customer);

                    // 更新标签的人数统计
                    foreach (var label in labels)
                    {
                        label.PeopleNumber = (label.PeopleNumber ?? 0) + 1;
                        await _labelRepository.UpdateAsync(label);
                    }
                }

                return ApiResult<bool>.Success(true, ResultCode.Success);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message, ResultCode.Error);
            }
        }

        /// <summary>
        /// 移除客户标签
        /// </summary>
        public async Task<ApiResult<bool>> RemoveCustomerLabelsAsync(CustomerLabelDto dto)
        {
            try
            {
                // 参数校验
                if (dto.CustomerIds == null || !dto.CustomerIds.Any() ||
                    dto.LabelIds == null || !dto.LabelIds.Any())
                {
                    return ApiResult<bool>.Fail("参数无效", ResultCode.Error);
                }

                // 获取标签列表（用于更新人数）
                var labels = await _labelRepository.GetListAsync(l => dto.LabelIds.Contains(l.Id));

                // 获取客户列表
                var customers = await _customerRepository.GetListAsync(c => dto.CustomerIds.Contains(c.Id));

                // 更新每个客户的标签
                foreach (var customer in customers)
                {
                    // 检查是否存在 CustomerLabel 属性
                    if (string.IsNullOrEmpty(customer.CustomerLabel))
                    {
                        continue;
                    }

                    // 将现有标签转换为列表
                    var currentLabels = customer.CustomerLabel.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                             .Select(l => Guid.Parse(l))
                                                             .Where(l => !dto.LabelIds.Contains(l))
                                                             .ToList();

                    // 更新客户的标签字段
                    customer.CustomerLabel = string.Join(",", currentLabels);
                    await _customerRepository.UpdateAsync(customer);

                    // 更新标签的人数统计
                    foreach (var label in labels)
                    {
                        label.PeopleNumber = Math.Max(0, (label.PeopleNumber ?? 1) - 1);
                        await _labelRepository.UpdateAsync(label);
                    }
                }

                return ApiResult<bool>.Success(true, ResultCode.Success);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message, ResultCode.Error);
            }
        }

        /// <summary>
        /// 获取客户的所有标签
        /// </summary>
        public async Task<ApiResult<List<CustomerLabelResultDto>>> GetCustomerLabelsAsync(Guid customerId)
        {
            try
            {
                // 获取客户
                var customer = await _customerRepository.GetAsync(customerId);
                if (string.IsNullOrEmpty(customer.CustomerLabel))
                {
                    return ApiResult<List<CustomerLabelResultDto>>.Success(
                        new List<CustomerLabelResultDto>(), 
                        ResultCode.Success
                    );
                }

                // 获取标签ID列表
                var labelIds = customer.CustomerLabel.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                            .Select(l => Guid.Parse(l))
                                            .ToList();

                // 获取标签详细信息
                var labels = await _labelRepository.GetListAsync(l => labelIds.Contains(l.Id));

                // 转换为DTO
                var result = labels.Select(l => new CustomerLabelResultDto
                {
                    LabelId = l.Id,
                    LabelName = l.LabelName,
                    TagType = l.TagType
                }).ToList();

                return ApiResult<List<CustomerLabelResultDto>>.Success(result, ResultCode.Success);
            }
            catch (Exception ex)
            {
                return ApiResult<List<CustomerLabelResultDto>>.Fail(ex.Message, ResultCode.Error);
            }
        }
        /// <summary>
        /// 根据ID获取客户详细信息
        /// </summary>
        /// <param name="id">客户ID（Guid类型）</param>
        /// <returns>客户详细信息DTO，包含客户的所有属性</returns>
        /// <remarks>
        /// 1. 根据ID查询客户实体。
        /// 2. 检查客户是否存在，不存在则返回NotFound错误。
        /// 3. 使用ObjectMapper将实体映射为DTO。
        /// 4. 返回包含客户详细信息的ApiResult。
        /// </remarks>
        
        public async Task<ApiResult<FanCustomerDto>> GetCustomerByIdAsync(Guid id)
        {
            // 获取客户信息
            var customer = await _customerRepository.FirstOrDefaultAsync(c => c.Id == id);
            
            // 2. 检查客户是否存在：如果查询结果为null，则返回NotFound错误
            if (customer == null)
            {
                return ApiResult<FanCustomerDto>.Fail("客户不存在", ResultCode.NotFound);
            }
            // 获取客户类型信息
            var customerType = await _customerTypeRepository.FirstOrDefaultAsync(t => t.Id == customer.CustomerType);

            // 映射客户信息到 DTO
            var customerDto = ObjectMapper.Map<HotelABPCustoimerss, FanCustomerDto>(customer);

            // 设置客户类型名称
            customerDto.CustomerTypeName = customerType?.CustomerTypeName;

            return ApiResult<FanCustomerDto>.Success(customerDto, ResultCode.Success);
        }
    }
   
}

