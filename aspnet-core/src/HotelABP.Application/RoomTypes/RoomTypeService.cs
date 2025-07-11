using HotelABP.RoomTypes.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Transactions;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;

namespace HotelABP.RoomTypes
{
    /// <summary>
    /// 房型管理
    /// </summary>
    [IgnoreAntiforgeryToken]
    [ApiExplorerSettings(GroupName = "roomtype")]
    public class RoomTypeService : ApplicationService, IRoomTypeService
    {
        private readonly IRepository<RoomType, Guid> _roomTypeRepository;
        IDistributedCache<List<RoomTypeDto>> distributedCache;

        public RoomTypeService(IRepository<RoomType, Guid> roomTypeRepository, IDistributedCache<List<RoomTypeDto>> distributedCache)
        {
            _roomTypeRepository = roomTypeRepository;
            this.distributedCache = distributedCache;
        }
        /// <summary>
        /// 新增房型（支持DTO映射和缓存清理）
        /// </summary>
        /// <param name="input">房型新增DTO</param>
        /// <returns>新增后的房型DTO</returns>
        /// <remarks>
        /// 1. 使用ObjectMapper将输入DTO映射为实体。
        /// 2. 插入数据库。
        /// 3. 映射为返回DTO。
        /// 4. 新增成功后清理房型列表缓存，保证数据一致性。
        /// 5. 捕获异常并返回失败信息。
        /// </remarks>
        public async Task<ApiResult<RoomTypeDto>> CreateAdd(CreateUpdateRoomTypeDto input)
        {
            try
            {
                // 用 ObjectMapper 映射
                var entity = ObjectMapper.Map<CreateUpdateRoomTypeDto, RoomType>(input);

                var entitydto = await _roomTypeRepository.InsertAsync(entity);
                var s  = ObjectMapper.Map<RoomType, RoomTypeDto>(entitydto);
                // 添加成功后，清理缓存
                await distributedCache.RemoveAsync("RoomType_GetListAsync");
                return ApiResult<RoomTypeDto>.Success(s, ResultCode.Success);
            }
            catch (Exception ex)
            {
                return ApiResult<RoomTypeDto>.Fail(ex.Message, ResultCode.Error);
            }
        }
        /// <summary>
        /// 分页条件查询房型（支持缓存、DTO映射和多条件筛选）
        /// </summary>
        /// <param name="seach">分页参数</param>
        /// <param name="dto">筛选条件DTO</param>
        /// <returns>分页后的房型DTO列表</returns>
        /// <remarks>
        /// 1. 优先从缓存获取房型列表。
        /// 2. 若缓存命中则直接分页返回。
        /// 3. 若缓存未命中则查询数据库，映射为DTO并写入缓存。
        /// 4. 支持按名称模糊查询。
        /// 5. 返回分页结果。
        /// </remarks>
        public async Task<ApiResult<PageResult<RoomTypeDto>>> GetListShow(Seach seach, GetRoomTypeDto dto)
        {
            string cacheKey = $"RoomType_GetListAsync";
            var list = await distributedCache.GetAsync(cacheKey);
            if (list != null)
            {
                // 缓存命中，直接筛选和分页
                list = list.WhereIf(!string.IsNullOrWhiteSpace(dto.Name), x => x.Name.Contains(dto.Name)).ToList();
                var a = list.AsQueryable().PageResult(seach.PageIndex, seach.PageSize);
                return ApiResult<PageResult<RoomTypeDto>>.Success(
                    new PageResult<RoomTypeDto>
                    {
                        Data = a.Queryable.OrderByDescending(x=>x.Order).ToList(),
                        TotleCount = a.RowCount,
                        TotlePage = (int)Math.Ceiling(a.RowCount / (double)seach.PageSize)
                    }, ResultCode.Success);
            }

            // 缓存未命中，查询数据库
            var query = await _roomTypeRepository.GetQueryableAsync();

            // 在缓存之前将RoomType转换成RoomTypeDto
            var roomTypeDtos = ObjectMapper.Map<List<RoomType>, List<RoomTypeDto>>(query.ToList());

            await distributedCache.SetAsync(cacheKey, roomTypeDtos, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10000)
            });

            // 支持按名称模糊查询
            query = query.WhereIf(!string.IsNullOrWhiteSpace(dto.Name), x => x.Name.Contains(dto.Name));
            
            var dtos =ObjectMapper.Map<List<RoomType>, List<RoomTypeDto>>(query.ToList());
            var res = dtos.AsQueryable().PageResult(seach.PageIndex, seach.PageSize);
            return ApiResult<PageResult<RoomTypeDto>>.Success(
                new PageResult<RoomTypeDto>
                {
                    Data = res.Queryable.OrderByDescending(x=>x.Order).ToList(),
                    TotleCount = res.RowCount,
                    TotlePage =(int)Math.Ceiling(res.RowCount / (double)seach.PageSize)
                }, ResultCode.Success);
        }
        /// <summary>
        /// 修改房型（支持DTO映射和缓存清理）
        /// </summary>
        /// <param name="id">房型ID</param>
        /// <param name="input">房型修改DTO</param>
        /// <returns>修改后的房型DTO</returns>
        /// <remarks>
        /// 1. 根据ID查询现有实体。
        /// 2. 用ObjectMapper将DTO属性映射到实体。
        /// 3. 更新数据库。
        /// 4. 映射为返回DTO。
        /// 5. 修改成功后清理缓存。
        /// 6. 捕获异常并返回失败信息。
        /// </remarks>
        public async Task<ApiResult<RoomTypeDto>> UpdateRoomType(Guid id, CreateUpdateRoomTypeDto input)
        {
            try
            {
                // 先根据 ID 查询现有实体
                var existingEntity = await _roomTypeRepository.GetAsync(id);
                
                // 用 ObjectMapper 将 DTO 的属性映射到现有实体
                ObjectMapper.Map(input, existingEntity);
                
                // 更新实体
                var updatedEntity = await _roomTypeRepository.UpdateAsync(existingEntity);
                
                // 映射为 DTO 返回
                var dto = ObjectMapper.Map<RoomType, RoomTypeDto>(updatedEntity);
                // 修改成功后，清理缓存
                await distributedCache.RemoveAsync("RoomType_GetListAsync");
                return ApiResult<RoomTypeDto>.Success(dto, ResultCode.Success);
            }
            catch (Exception ex)
            {
                return ApiResult<RoomTypeDto>.Fail(ex.Message, ResultCode.Error);
            }
        }
        /// <summary>
        /// 删除房型（支持缓存清理）
        /// </summary>
        /// <param name="id">房型ID</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 1. 根据ID查找房型。
        /// 2. 删除数据库记录。
        /// 3. 删除成功后清理缓存。
        /// 4. 捕获异常并返回失败信息。
        /// </remarks>
        public async Task<ApiResult<bool>> DeleteRoomTypeDel(Guid id)
        {
            try
            {
                var res=await _roomTypeRepository.FindAsync(id);
                await _roomTypeRepository.DeleteAsync(id);
                // 删除成功后，清理缓存
                await distributedCache.RemoveAsync("RoomType_GetListAsync");
                return ApiResult<bool>.Success(true, ResultCode.Success);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message, ResultCode.Error);
            }
        }
        /// <summary>
        /// 批量删除房型（支持事务和缓存清理）
        /// </summary>
        /// <param name="ids">房型ID集合</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 1. 开启事务，保证批量删除的原子性。
        /// 2. 遍历ID集合，依次删除房型。
        /// 3. 删除成功后清理缓存。
        /// 4. 捕获异常并返回失败信息。
        /// </remarks>
        public async Task<ApiResult<bool>> DeleteBatchRoomType(List<Guid> ids)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var id in ids)
                    {
                        await _roomTypeRepository.DeleteAsync(id);
                    }
                    // 删除成功后，清理缓存
                    await distributedCache.RemoveAsync("RoomType_GetListAsync");
                    tran.Complete();
                    return ApiResult<bool>.Success(true, ResultCode.Success);
                }
                
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message, ResultCode.Error);
            }
        }
        /// <summary>
        /// 修改房型排序（支持缓存清理）
        /// </summary>
        /// <param name="dto">排序DTO，包含房型ID和新排序值</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 1. 根据ID查找房型。
        /// 2. 修改排序字段。
        /// 3. 更新数据库。
        /// 4. 修改成功后清理缓存。
        /// 5. 捕获异常并抛出。
        /// </remarks>
        public async Task<ApiResult> UpdateRoomTypeOrder(UpdataRoomTypeOrderDto dto)
        {
            try
            {
                var res=await _roomTypeRepository.GetAsync(dto.Id);
                res.Order = dto.Order;
               await _roomTypeRepository.UpdateAsync(res);
                // 修改成功后，清理缓存
                await distributedCache.RemoveAsync("RoomType_GetListAsync");
                return ApiResult.Success( ResultCode.Success);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
    }
}
