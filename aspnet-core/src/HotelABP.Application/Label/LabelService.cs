using HotelABP.Labels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HotelABP.Label
{
    /// <summary>
    /// 标签管理
    /// </summary>
    [ApiExplorerSettings(GroupName ="label")]
    public class LabelService : ApplicationService, ILabelService
    {
        private readonly IRepository<HotelABPLabelss, Guid> _labelRepository;

        public LabelService(IRepository<HotelABPLabelss, Guid> labelRepository)
        {
            _labelRepository = labelRepository;
        }
        /// <summary>
        /// 添加标签（支持DTO映射和异常处理）
        /// </summary>
        /// <param name="ldto">标签DTO</param>
        /// <returns>插入后的标签DTO</returns>
        /// <remarks>
        /// 1. 使用ObjectMapper将输入DTO映射为实体。
        /// 2. 插入数据库。
        /// 3. 将插入后的实体再次映射为DTO。
        /// 4. 返回带有插入结果的ApiResult。
        /// 5. 捕获异常并返回失败信息。
        /// </remarks>
        public async Task<ApiResult<LabelDto>> AddLabelAsync(LabelDto ldto)
        {
            try
            {
                // 将 LabelDto 映射为 HotelABPLabels 实体对象
                var entity = ObjectMapper.Map<LabelDto, HotelABPLabelss>(ldto);
                // 插入实体对象到数据库，并返回插入后的实体
                var entitydto = await _labelRepository.InsertAsync(entity);
                // 将插入后的实体对象再次映射为 LabelDto
                var s = ObjectMapper.Map<HotelABPLabelss, LabelDto>(entitydto);
                // 返回带有插入结果的 ApiResult
                return ApiResult<LabelDto>.Success(s, ResultCode.Success);
            }
            catch (Exception ex)
            {
                // 捕获异常并返回失败的 ApiResult
                return ApiResult<LabelDto>.Fail(ex.Message, ResultCode.Error);
            }
        }

        /// <summary>
        /// 分页条件查询标签列表（支持DTO映射、条件筛选、分页、异常处理）
        /// </summary>
        /// <param name="seach">分页参数</param>
        /// <param name="dtoList">标签筛选条件DTO</param>
        /// <returns>分页后的标签DTO列表</returns>
        /// <remarks>
        /// 1. 查询所有标签。
        /// 2. 按标签名模糊筛选（如有条件）。
        /// 3. 分页处理。
        /// 4. DTO映射。
        /// 5. 返回分页结果。
        /// 6. 捕获异常并返回失败信息。
        /// </remarks>
        public async Task<ApiResult<PageResult<GetLabelDto>>> GetCustomerListAsync(Seach seach, GetLabeDtoList dtoList)
        {
            try
            {
                // 查询所有标签
                var list = await _labelRepository.GetQueryableAsync();
                // 按标签名模糊筛选
                list = list.WhereIf(!string.IsNullOrEmpty(dtoList.LabelName), x => x.LabelName.Contains(dtoList.LabelName));

                // 分页处理
                var res = list.PageResult(seach.PageIndex, seach.PageSize);
                // DTO映射
                var dto = ObjectMapper.Map<List<HotelABPLabelss>, List<GetLabelDto>>(list.ToList());
                // 返回分页结果
                return ApiResult<PageResult<GetLabelDto>>.Success(
             new PageResult<GetLabelDto>
             {
                 Data = dto,
                 TotleCount = list.Count(),
                 TotlePage = (int)Math.Ceiling(list.Count() / (double)seach.PageSize)

             },
                         ResultCode.Success
          );
            }
            catch (Exception ex)
            { 
                // 捕获异常并返回失败的 ApiResult
                return ApiResult<PageResult<GetLabelDto>>.Fail(ex.Message, ResultCode.Error);
            }
        }
    }
}
