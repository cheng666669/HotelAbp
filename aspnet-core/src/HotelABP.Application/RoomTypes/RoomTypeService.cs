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
    [IgnoreAntiforgeryToken]
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
        /// 房型新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

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
        /// 分页条件查询
        /// </summary>
        /// <param name="seach"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ApiResult<PageResult<RoomTypeDto>>> GetListShow(Seach seach, GetRoomTypeDto dto)
        {
            string cacheKey = $"RoomType_GetListAsync";
            var list = await distributedCache.GetAsync(cacheKey);
            if (list != null)
            {
                list = list.WhereIf(!string.IsNullOrWhiteSpace(dto.Name), x => x.Name.Contains(dto.Name)).ToList();
                var a = list.AsQueryable().PageResult(seach.PageIndex, seach.PageSize);
                return ApiResult<PageResult<RoomTypeDto>>.Success(
                    new PageResult<RoomTypeDto>
                    {
                        Data = a.Queryable.ToList(),
                        TotleCount = a.RowCount,
                        TotlePage = (int)Math.Ceiling(a.RowCount / (double)seach.PageSize)
                    }, ResultCode.Success);
            }

            // 查询
            var query = await _roomTypeRepository.GetQueryableAsync();

            //在缓存之前将RoomType转换成RoomTypeDto
            var roomTypeDtos = ObjectMapper.Map<List<RoomType>, List<RoomTypeDto>>(query.ToList());

            await distributedCache.SetAsync(cacheKey, roomTypeDtos, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10000)
            });

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
        /// 修改房型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
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
        /// 删除房型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// 批量删除房型
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
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
        ///  修改房型排序
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Order"></param>
        /// <returns></returns>
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
