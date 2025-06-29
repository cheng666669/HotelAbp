using HotelABP.DTos.ReserveRooms;
using HotelABP.RoomNummbers;
using HotelABP.RoomReserves;
using HotelABP.RoomTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace HotelABP.ReserveRooms
{
    [IgnoreAntiforgeryToken]
    public class ReserveRoomServer : ApplicationService, IReserveRoomServer
    {
        private readonly IRepository<ReserveRoom, Guid> reserveRoomRepository; // Fix type to ReserveRoom
        private readonly IDistributedCache<List<ReserveRoomShowDto>> reserveRoomCache;
        private readonly IRepository<RoomType, Guid> _roomTypeRepository;
        private readonly IRepository<RoomNummber, Guid> roomnumberreposi;
        private readonly IRepository<MoneyDetail, Guid>  monrydetails;

        public ReserveRoomServer(IRepository<ReserveRoom, Guid> reserveRoomRepository, IDistributedCache<List<ReserveRoomShowDto>> reserveRoomCache, IRepository<RoomType, Guid> roomTypeRepository, IRepository<RoomNummber, Guid> roomnumberreposi, IRepository<MoneyDetail, Guid> monrydetails)
        {
            this.reserveRoomRepository = reserveRoomRepository;
            this.reserveRoomCache = reserveRoomCache;
            _roomTypeRepository = roomTypeRepository;
            this.roomnumberreposi = roomnumberreposi;
            this.monrydetails = monrydetails;
        }


        /// <summary>
        /// 房间类型列表
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult<List<RoomType>>> GetRoomTypesList()
        {
            try
            {
                var list = await _roomTypeRepository.GetQueryableAsync();
                return ApiResult<List<RoomType>>.Success(list.ToList(), ResultCode.Success);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 房间预定添加
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public async Task<ApiResult> AddReserveRoom(CreateRoom room)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var item in room.aaa)
                    {
                        // 检查Id有效性
                        if (item.Id == Guid.Empty)
                        {
                            return ApiResult.Fail("房型ID无效", ResultCode.Error);
                        }

                        var reserveRoom = ObjectMapper.Map<CreateRoom, ReserveRoom>(room);
                        reserveRoom.RoomTypeid = item.Id.ToString();
                        reserveRoom.Price = item.Price;
                        reserveRoom.RoomNum = "未排房";
                        // 建议补充 BreakfastNum 字段赋值
                        reserveRoom.BreakfastNum = 0;
                       var res= await reserveRoomRepository.InsertAsync(reserveRoom);
                        var moneyDetail = new MoneyDetail
                        {
                            BookingNumber = res.Id.ToString(),
                            BusinesName = "房间",
                            Money = res.Price,
                            Money1 = 0,
                            States = 2
                        };
                        await monrydetails.InsertAsync(moneyDetail);
                    }

                    // 添加成功后，清理缓存
                    await reserveRoomCache.RemoveAsync("GetReserRoom");
                    tran.Complete(); // 提交事务
                    return ApiResult.Success(ResultCode.Success);
                }
            }
            catch (Exception ex)
            {
                // 返回详细错误信息
                return ApiResult.Fail($"添加预定失败: {ex.Message}", ResultCode.Error);
            }
        }

        /// <summary>
        ///预定房间信息
        /// </summary>
        /// <param name="search1"></param>
        /// <param name="seach"></param>
        /// <returns></returns>
        public async Task<ApiResult<PageResult<ReserveRoomShowDto>>> ShowReserveRoom([FromQuery] SearchTiao search1, [FromQuery] Seach seach)
        {
            var cacheKey = "GetReserRoom";

            // 读取缓存
            var cachedResult = await reserveRoomCache.GetOrAddAsync(cacheKey, async () =>
            {
                // 查询数据库
                //预订信息
                var list = await reserveRoomRepository.GetQueryableAsync();
                //房间类型
                var roomTypes = await _roomTypeRepository.GetQueryableAsync();

                var listdto = from reserveRoom in list
                              join roomType in roomTypes on reserveRoom.RoomTypeid equals roomType.Id.ToString()
                              select new ReserveRoomShowDto
                              {
                                  Id = reserveRoom.Id,
                                  Infomation = reserveRoom.Infomation,
                                  Ordersource = reserveRoom.Ordersource,
                                  ReserveName = reserveRoom.ReserveName,
                                  Phone = reserveRoom.Phone,
                                  BookingNumber = reserveRoom.BookingNumber,
                                  Sdate = reserveRoom.Sdate,
                                  Edate = reserveRoom.Edate,
                                  Day = reserveRoom.Day,
                                  RoomTypeid = roomType.Id.ToString(),
                                  RoomTypeName = roomType.Name, // 房型名称
                                  BreakfastNum = reserveRoom.BreakfastNum,
                                  Price = reserveRoom.Price,
                                  Status = reserveRoom.Status,
                                  IdCard = reserveRoom.IdCard,
                                  RoomNum = reserveRoom.RoomNum,
                                  Message = reserveRoom.Message
                              };
                //var listdto = from reserveRoom in list
                //              join roomType in roomTypes on reserveRoom.RoomTypeid equals roomType.Id.ToString()
                //              select new ReserveRoomShowDto
                //              {
                //                  Id = reserveRoom.Id,
                //                  Infomation = reserveRoom.Infomation,
                //                  Ordersource = reserveRoom.Ordersource,
                //                  ReserveName = reserveRoom.ReserveName,
                //                  Phone = reserveRoom.Phone,
                //                  BookingNumber = reserveRoom.BookingNumber,
                //                  Sdate = reserveRoom.Sdate,
                //                  Edate = reserveRoom.Edate,
                //                  Day = reserveRoom.Day,
                //                  RoomTypeid = roomType.Id.ToString(),
                //                  RoomTypeName = roomType.Name, // 房型名称
                //                  BreakfastNum = reserveRoom.BreakfastNum,
                //                  Price = reserveRoom.Price,
                //                  Status = reserveRoom.Status,
                //                  IdCard = reserveRoom.IdCard,
                //                  RoomNum = reserveRoom.RoomNum,
                //                  Message = reserveRoom.Message
                //              };
                return listdto.ToList();

            }, () => new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(30) // 设置缓存过期时间为30分钟
            });
            cachedResult = cachedResult.WhereIf(search1.Status >= 0, x => x.Status == search1.Status)
                    .WhereIf(!string.IsNullOrEmpty(search1.Sdate), x => x.Sdate >= Convert.ToDateTime(search1.Sdate))
                    .WhereIf(!string.IsNullOrEmpty(search1.Edate), x => x.Sdate <= Convert.ToDateTime(search1.Edate))
                    .WhereIf(!string.IsNullOrEmpty(search1.Comman), x => x.ReserveName.Contains(search1.Comman) || x.Phone.Contains(search1.Comman) || x.RoomNum == search1.Comman || x.IdCard == search1.Comman).ToList();

            var pagedResult = cachedResult.AsQueryable().PageResult(seach.PageIndex, seach.PageSize);
            var totalPages = (int)Math.Ceiling((double)pagedResult.RowCount / seach.PageSize);

            var result = new PageResult<ReserveRoomShowDto>
            {
                Data = pagedResult.Queryable.ToList(),
                TotleCount = pagedResult.RowCount,
                TotlePage = totalPages
            };

            return ApiResult<PageResult<ReserveRoomShowDto>>.Success(result, ResultCode.Success);
        }
        /// <summary>
        /// 排房
        /// </summary>
        /// <param name="id"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        public async Task<ApiResult> UpdateTerraces(UpdateDto dto)
        {
            var list = await reserveRoomRepository.GetAsync(dto.id);
            if (list == null)
            {
                return ApiResult.Fail("未找到该预定信息", ResultCode.Error);
            }

            list.RoomNum = dto.roomnum; // 更新房间号码
            await reserveRoomRepository.UpdateAsync(list);
            // 成功后，清理缓存
            await reserveRoomCache.RemoveAsync("GetReserRoom");
            return ApiResult.Success(ResultCode.Success);
        }
        /// <summary>
        /// 房间号
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult<List<RoomNumDto>>> GetRoomNmlist(string guid)
        {
            var list = await roomnumberreposi.GetListAsync(x => x.RoomTypeId == guid);
            var roomNumList = list.Select(x => new RoomNumDto
            {
                ID = x.Id.ToString(),
                Name = x.RoomNum
            }).ToList();
            return ApiResult<List<RoomNumDto>>.Success(roomNumList, ResultCode.Success);
        }


        /// <summary>
        /// 取消排房
        /// </summary>
        /// <param name="id"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        public async Task<ApiResult> UpdateNoTerraces(Guid id)
        {
            var list = await reserveRoomRepository.GetAsync(id);
            if (list == null)
            {
                return ApiResult.Fail("未找到该预定信息", ResultCode.Error);
            }

            list.RoomNum = "未排房";
            await reserveRoomRepository.UpdateAsync(list);
            // 成功后，清理缓存
            await reserveRoomCache.RemoveAsync("GetReserRoom");
            return ApiResult.Success(ResultCode.Success);
        }

        /// <summary>
        /// 取消预定
        /// </summary>
        /// <param name="id"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        public async Task<ApiResult> UpdateNotReserc(UpdateNoReserDto dto)
        {
            var list = await reserveRoomRepository.GetAsync(dto.id);
            if (list == null)
            {
                return ApiResult.Fail("未找到该预定信息", ResultCode.Error);
            }

            list.Status = 5; // 设置为已取消状态
            list.NoReservRoom = dto.NoReservRoom;
            await reserveRoomRepository.UpdateAsync(list);
            // 成功后，清理缓存
            await reserveRoomCache.RemoveAsync("GetReserRoom");
            return ApiResult.Success(ResultCode.Success);
        }

        /// <summary>
        /// 入住
        /// </summary>
        /// <param name="id"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        public async Task<ApiResult> UpdateCheckin(Update1Dto dto)
        {
            var list = await reserveRoomRepository.GetAsync(dto.Id);
            if (list == null)
            {
                return ApiResult.Fail("未找到该预定信息", ResultCode.Error);
            }
            list.Status = 2; // 设置为已入住状态
            list.IdCard = dto.IdCard; // 更新身份证号码
            list.RoomNum = dto.RoomNum;
            list.ReserveName = dto.ReserveName;
            list.Phone = dto.Phone; // 更新手机号码
            await reserveRoomRepository.UpdateAsync(list);
            // 成功后，清理缓存
            await reserveRoomCache.RemoveAsync("GetReserRoom");
            return ApiResult.Success(ResultCode.Success);
        }
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        /// <summary>
        /// 退房
        /// </summary>
        /// <param name="id"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        public async Task<ApiResult> UpdateNoRoom(Guid id)
        {
            var list = await reserveRoomRepository.GetAsync(id);
            if (list == null)
            {
                return ApiResult.Fail("未找到该预定信息", ResultCode.Error);
            }

            list.Status = 3; // 设置为已退房状态
            await reserveRoomRepository.UpdateAsync(list);
            // 成功后，清理缓存
            await reserveRoomCache.RemoveAsync("GetReserRoom");
            return ApiResult.Success(ResultCode.Success);
        }
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        //结算
        public async Task<ApiResult> UpdateSettlement(MoneyDetailDto dto)
        {
            using (var tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Fix for CS1503: Use a lambda expression to match the BookingNumber field
                var list = await reserveRoomRepository.GetAsync(x => x.Id.ToString() == dto.BookingNumber);
                if (list == null)
                {
                    return ApiResult.Fail("未找到该预定信息", ResultCode.Error);
                }
                list.Status = 3; // 设置为已结算状态
                await reserveRoomRepository.UpdateAsync(list);
                // 成功后，清理缓存
                await reserveRoomCache.RemoveAsync("GetReserRoom");

                var list123 = await monrydetails.FirstOrDefaultAsync(x => x.BookingNumber == dto.BookingNumber);
                if (list123 != null)
                {
                    list.Status = 2; // 设置为已结算状态
                    await monrydetails.UpdateAsync(list123);
                }
               

                var moneyDetail = ObjectMapper.Map<MoneyDetailDto, MoneyDetail>(dto);
                moneyDetail.States=2; // 设置为已结算状态
                await monrydetails.InsertAsync(moneyDetail);

                tran.Complete(); // 提交事务
                return ApiResult.Success(ResultCode.Success);
            }
        }

        [HttpGet]
        public async Task<ApiResult<ReserveRoomShowDto>> ShowFanReserveRoom(Guid id)
        {
            // 查询数据库
            // 预订信息
            var list = await reserveRoomRepository.GetListAsync(x => x.Id == id);
            // 房间类型
            var roomTypes = await _roomTypeRepository.GetListAsync();

            var listdto = (from reserveRoom in list
                           join roomType in roomTypes on reserveRoom.RoomTypeid equals roomType.Id.ToString()
                           select new ReserveRoomShowDto
                           {
                               Id = reserveRoom.Id,
                               Infomation = reserveRoom.Infomation,
                               Ordersource = reserveRoom.Ordersource,
                               ReserveName = reserveRoom.ReserveName,
                               Phone = reserveRoom.Phone,
                               BookingNumber = reserveRoom.BookingNumber,
                               Sdate = reserveRoom.Sdate,
                               Edate = reserveRoom.Edate,
                               Day = reserveRoom.Day,
                               RoomTypeid = roomType.Id.ToString(),
                               RoomTypeName = roomType.Name, // 房型名称
                               BreakfastNum = reserveRoom.BreakfastNum,
                               Price = reserveRoom.Price,
                               Status = reserveRoom.Status,
                               IdCard = reserveRoom.IdCard,
                               RoomNum = reserveRoom.RoomNum,
                               Message = reserveRoom.Message
                           }).FirstOrDefault(); // 使用 FirstOrDefault 获取单个对象

            return ApiResult<ReserveRoomShowDto>.Success(listdto, ResultCode.Success);
        }
    }
}
