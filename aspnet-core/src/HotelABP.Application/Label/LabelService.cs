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
                var dto = ObjectMapper.Map<List<HotelABPLabelss>, List<GetLabelDto>>(res.Queryable.ToList());
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
        /// <summary>
        /// 删除标签（根据ID）
        /// </summary>
        /// <param name="guid">标签ID</param>
        /// <returns>操作结果，成功返回Success状态码</returns>
        /// <remarks>
        /// 1. 根据ID查找标签实体。
        /// 2. 从数据库中删除该标签。
        /// 3. 返回操作结果。
        /// 4. 捕获异常并向上抛出。
        /// </remarks>
        [HttpDelete]
        public async Task<ApiResult> DelLabelAsync(Guid guid)
        {
            try
            {
                // 根据ID查找标签实体，确保要删除的标签存在
                var res = await _labelRepository.FindAsync(x => x.Id == guid);
                
                // 如果找到标签，则从数据库中删除它
                // 如果标签不存在，FindAsync会抛出异常，会被catch块捕获
                await _labelRepository.DeleteAsync(res);
                
                // 删除成功，返回成功的ApiResult，状态码为Success
                return ApiResult.Success(ResultCode.Success);
            }
            catch (Exception ex)
            {
                // 捕获异常（如标签不存在、数据库操作失败等）
                // 向上层抛出异常，由全局异常处理器或调用方处理
                throw;
            }
        }
        
        /// <summary>
        /// 修改标签信息（根据ID）
        /// </summary>
        /// <param name="guid">标签ID</param>
        /// <param name="ldto">包含更新信息的标签DTO</param>
        /// <returns>操作结果，成功返回Success状态码</returns>
        /// <remarks>
        /// 1. 根据ID查找标签实体。
        /// 2. 使用ObjectMapper将DTO中的属性映射到实体。
        /// 3. 更新数据库中的标签信息。
        /// 4. 返回操作结果。
        /// 5. 捕获异常并向上抛出。
        /// </remarks>
        public async Task<ApiResult> UpdateLabelAsync(Guid guid, LabelDto ldto)
        {
            try
            {
                // 根据ID查找标签实体，确保要更新的标签存在
                var res = await _labelRepository.FindAsync(x => x.Id == guid);
                
                // 使用ObjectMapper将输入DTO的属性映射到找到的实体对象
                // 这样可以只更新DTO中包含的字段，保留其他字段的原值
                var dto = ObjectMapper.Map(ldto, res);
                
                // 将更新后的实体保存到数据库
                await _labelRepository.UpdateAsync(dto);
                
                // 更新成功，返回成功的ApiResult，状态码为Success
                return ApiResult.Success(ResultCode.Success);
            }
            catch (Exception ex)
            {
                // 捕获异常（如标签不存在、数据库操作失败等）
                // 向上层抛出异常，由全局异常处理器或调用方处理
                throw;
            }
        }

        public async Task<ApiResult<FanLabelDto>> GetLabelByIdAsync(Guid id)
        {
            // 参数校验
            if (id == Guid.Empty)
            {
                return ApiResult<FanLabelDto>.Fail("ID无效", ResultCode.Error);
            }

            // 查询数据库中的标签实体
            var entity = await _labelRepository.GetAsync(id);
            if (entity == null)
            {
                return ApiResult<FanLabelDto>.Fail("未找到对应标签", ResultCode.Error);
            }

            // 使用 ObjectMapper 将实体反填到 FanLabelDto
            var dto = ObjectMapper.Map<HotelABPLabelss, FanLabelDto>(entity);

            // 返回成功结果
            return ApiResult<FanLabelDto>.Success(dto, ResultCode.Success);
        }
    }
}
