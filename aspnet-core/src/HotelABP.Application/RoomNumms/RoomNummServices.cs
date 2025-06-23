using HotelABP.RoomNummbers;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;

namespace HotelABP.RoomNumms
{
    public class RoomNummServices:ApplicationService,IRoomNummberService
    {
        IRepository<RoomNummber, Guid> _roomNummberRepository;
        IDistributedCache<RoomNummber> _cache;
        public RoomNummServices(IRepository<RoomNummber, Guid> roomNummberRepository, IDistributedCache<RoomNummber> cache)
        {
            _roomNummberRepository = roomNummberRepository;
            _cache = cache;
        }
        /// <summary>
        /// 新增房号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ApiResult<RoomNummDto>> CreateAsync(CreateUpdataRoomNummDto input)
        {
            var entity=ObjectMapper.Map<CreateUpdataRoomNummDto, RoomNummber>(input);
            var entityDto=await _roomNummberRepository.InsertAsync(entity);
            var dto=ObjectMapper.Map<RoomNummber, RoomNummDto>(entityDto);
            return ApiResult<RoomNummDto>.Success(dto, ResultCode.Success);
        }


        /// <summary>
        /// 根据房型Id查询房号列表
        /// </summary>
        /// <param name="seach"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ApiResult <PageResult<List<RoomNummDto>>>> GetListToRoomTypeIdAsync(Seach seach, RoomNummRoomTypeRequestDto input)
        {
            var queryable = await _roomNummberRepository.GetQueryableAsync();

            queryable = queryable
                .WhereIf(!string.IsNullOrWhiteSpace(input.RoomTypeId), x => x.RoomTypeId == input.RoomTypeId);

            var res = queryable.PageResult(seach.PageIndex, seach.PageSize);
            var dto=ObjectMapper.Map<List<RoomNummber>, List<RoomNummDto>>(queryable.ToList());

           return ApiResult<PageResult<List<RoomNummDto>>>.Success(
               new PageResult<List<RoomNummDto>>
               {
                    Data = dto,
                    TotleCount = queryable.Count(),
                    TotlePage = (int)Math.Ceiling(queryable.Count() / (double)seach.PageSize)
                     
               },
               ResultCode.Success);
        }
        /// <summary>
        /// 批量删除房号
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<ApiResult<bool>> DeleteBatchAsync(List<Guid> ids)
        {
            foreach (var id in ids)
            {
                await _roomNummberRepository.DeleteAsync(id);
            }
            return ApiResult<bool>.Success(true, ResultCode.Success);
        }
        /// <summary>
        /// 设置房号状态为0
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ApiResult<bool>> UpdateStateToRoomNumAsync(Guid id)
        {
            try
            {
                var entity = await _roomNummberRepository.GetAsync(id);
                entity.State = false;
                await _roomNummberRepository.UpdateAsync(entity);
                return ApiResult<bool>.Success(true, ResultCode.Success);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message, ResultCode.Error);
            }
        }
        /// <summary>
        /// 批量设置房号状态为0
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<ApiResult<bool>> UpdateStateToZeroBatchAsync(List<Guid> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var entity = await _roomNummberRepository.GetAsync(id);
                    entity.State = false;
                    await _roomNummberRepository.UpdateAsync(entity);
                }
                return ApiResult<bool>.Success(true, ResultCode.Success);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message, ResultCode.Error);
            }
        }
        public async Task<ApiResult<PageResult<List<RoomNummDto>>>> GetListAsync(Seach seach, GetRoomNummberQuery input)
        {
            var queryable = await _roomNummberRepository.GetQueryableAsync();
            queryable = queryable
                .WhereIf(!string.IsNullOrEmpty(input.RoomTypeId), x => x.RoomTypeId == input.RoomTypeId)
                .WhereIf(input.State!=null, x => x.State==input.State)
                .WhereIf(!string.IsNullOrEmpty(input.RoomNum),x=>x.RoomNum==input.RoomNum);
            var res = queryable.PageResult(seach.PageIndex, seach.PageSize);
            var dto = ObjectMapper.Map<List<RoomNummber>, List<RoomNummDto>>(queryable.ToList());
            return ApiResult<PageResult<List<RoomNummDto>>>.Success(
                new PageResult<List<RoomNummDto>>
                {
                    Data = dto,
                    TotleCount = queryable.Count(),
                    TotlePage = (int)Math.Ceiling(queryable.Count() / (double)seach.PageSize)

                },
                ResultCode.Success);
        }
            
    }
}
