using HotelABP.DTos.ReserveRooms;
using HotelABP.RoomReserves;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace HotelABP.ReserveRooms
{
    public class ReserveRoomServer : ApplicationService, IReserveRoomServer
    {
        private readonly IRepository<ReserveRoom, Guid> reserveRoomRepository; // Fix type to ReserveRoom
        private readonly IDistributedCache<PageResult<ReserveRoomShowDto>> reserveRoomCache;

        public ReserveRoomServer(IRepository<ReserveRoom, Guid> reserveRoomRepository, IDistributedCache<PageResult<ReserveRoomShowDto>> reserveRoomCache) // Fix constructor parameter type
        {
            this.reserveRoomRepository = reserveRoomRepository;
            this.reserveRoomCache = reserveRoomCache;
        }

        /// <summary>
        /// 房间预定添加
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public async Task<ApiResult> ResuAdd(ReserveRoomDto room)
        {
            var list = ObjectMapper.Map<ReserveRoomDto, ReserveRoom>(room);
            list.RoomNum = "未排房"; // 默认未排房
            var result = await reserveRoomRepository.InsertAsync(list);
            return ApiResult.Success(ResultCode.Success);
        }
        /// <summary>
        ///预定房间信息
        /// </summary>
        /// <param name="search1"></param>
        /// <param name="seach"></param>
        /// <returns></returns>
        public async Task<ApiResult<PageResult<ReserveRoomShowDto>>> ReserveRoomShow([FromQuery] SearchTiao search1, [FromQuery] Seach seach)
        {
            var cacheKey = "GetReserRoom";

            // 读取缓存
            var cachedResult = await reserveRoomCache.GetOrAddAsync(cacheKey, async () =>
            {
                // 查询数据库
                var list = await reserveRoomRepository.GetQueryableAsync();
                list = list.WhereIf(search1.Status >= 0, x => x.Status == search1.Status)
                    .WhereIf(search1.Sdate != null, x => x.Sdate >= search1.Sdate)
                    .WhereIf(search1.Edate != null, x => x.Edate <= search1.Edate)
                    .WhereIf(!string.IsNullOrEmpty(search1.Comman), x => x.ReserveName.Contains(search1.Comman) || x.Phone.Contains(search1.Comman) || x.RoomNum == search1.Comman || x.IdCard == search1.Comman);

                // 修复错误：将 ReserveRoom 映射为 ReserveRoomShowDto
                var mappedList = ObjectMapper.Map<List<ReserveRoom>, List<ReserveRoomShowDto>>(list.ToList());
                var pagedResult = mappedList.AsQueryable().PageResult(seach.PageIndex, seach.PageSize);

                var total = list.Count();
                var totalPages = (int)Math.Ceiling((double)total / seach.PageSize);

                var result = new PageResult<ReserveRoomShowDto>
                {
                    Data = pagedResult.Queryable.ToList(),
                    TotleCount = total,
                    TotlePage = totalPages
                };
                return result;
            }, () => new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(30) // 设置缓存过期时间为30分钟
            });

            return ApiResult<PageResult<ReserveRoomShowDto>>.Success(cachedResult, ResultCode.Success);
        }
        /// <summary>
        /// 排房
        /// </summary>
        /// <param name="id"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        public async Task<ApiResult> TerracesUpdate(Guid id, string roomnum)
        {
            var list = await reserveRoomRepository.GetAsync(id);
            if (list == null)
            {
                return ApiResult.Fail("未找到该预定信息", ResultCode.Error);
            }

            list.RoomNum = roomnum; // 更新房间号码
            await reserveRoomRepository.UpdateAsync(list);
            return ApiResult.Success(ResultCode.Success);
        }

        /// <summary>
        /// 取消排房
        /// </summary>
        /// <param name="id"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        public async Task<ApiResult> Terraces1Update(Guid id)
        {
            var list = await reserveRoomRepository.GetAsync(id);
            if (list == null)
            {
                return ApiResult.Fail("未找到该预定信息", ResultCode.Error);
            }

            list.RoomNum = "未排房";
            await reserveRoomRepository.UpdateAsync(list);
            return ApiResult.Success(ResultCode.Success);
        }

        /// <summary>
        /// 取消预定
        /// </summary>
        /// <param name="id"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        public async Task<ApiResult> Reserc1Update(Guid id)
        {
            var list = await reserveRoomRepository.GetAsync(id);
            if (list == null)
            {
                return ApiResult.Fail("未找到该预定信息", ResultCode.Error);
            }

            list.Status= 3; // 设置为已取消状态
            await reserveRoomRepository.UpdateAsync(list);
            return ApiResult.Success(ResultCode.Success);
        }

        /// <summary>
        /// 入住
        /// </summary>
        /// <param name="id"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        public async Task<ApiResult> CheckinUpdate(Guid id,string idcrad)
        {
            var list = await reserveRoomRepository.GetAsync(id);
            if (list == null)
            {
                return ApiResult.Fail("未找到该预定信息", ResultCode.Error);
            }
            list.Status = 2; // 设置为已入住状态
            list.IdCard = idcrad; // 更新身份证号码
            await reserveRoomRepository.UpdateAsync(list);
            return ApiResult.Success(ResultCode.Success);
        }
    }
}
