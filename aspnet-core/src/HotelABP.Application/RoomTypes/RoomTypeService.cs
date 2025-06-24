using AutoMapper.Internal.Mappers;
using HotelABP.RoomNummbers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace HotelABP.RoomTypes
{
    public class RoomTypeService : ApplicationService, IRoomTypeService
    {
        private readonly IRepository<RoomType, Guid> _roomTypeRepository;
        IDistributedCache<PageResult<RoomTypeDto>> distributedCache;

        public RoomTypeService(IRepository<RoomType, Guid> roomTypeRepository, IDistributedCache<PageResult<RoomTypeDto>> distributedCache)
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
            string cacheKey = $"RoomType_GetListAsync_{dto.Name}_{seach.PageIndex}_{seach.PageSize}";
            var cache = await distributedCache.GetOrAddAsync(cacheKey, async () =>
            {
                // 构建查询
                var query = await _roomTypeRepository.GetQueryableAsync();
                query = query.WhereIf(!string.IsNullOrWhiteSpace(dto.Name), x => x.Name.Contains(dto.Name));
                var res = query.PageResult(seach.PageIndex, seach.PageSize);
                var dtos = ObjectMapper.Map<List<RoomType>, List<RoomTypeDto>>(query.ToList());
                return new PageResult<RoomTypeDto>
                {
                    Data = dtos.ToList(),
                    TotleCount = query.Count(),
                    TotlePage = (int)Math.Ceiling(query.Count() / (double)seach.PageSize)

                };
            },()=>new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
            
            return ApiResult<PageResult<RoomTypeDto>>.Success(cache, ResultCode.Success);
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
                await _roomTypeRepository.DeleteAsync(id);
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
                foreach (var id in ids)
                {
                    await _roomTypeRepository.DeleteAsync(id);
                }
                return ApiResult<bool>.Success(true, ResultCode.Success);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message, ResultCode.Error);
            }
        }
    }
}
