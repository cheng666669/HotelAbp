﻿using HotelABP.DTos.ReserveRooms;
using HotelABP.Export;
using HotelABP.RoomNummbers;
using HotelABP.RoomReserves;
using HotelABP.RoomTypes;
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
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;

namespace HotelABP.ReserveRooms
{
    /// <summary>
    /// 预定房间管理
    /// </summary>
    [IgnoreAntiforgeryToken]
    [ApiExplorerSettings(GroupName = "reserveroom")]
    public class ReserveRoomServer : ApplicationService, IReserveRoomServer
    {
        private readonly IRepository<ReserveRoom, Guid> reserveRoomRepository; // Fix type to ReserveRoom
        private readonly IDistributedCache<List<ReserveRoomShowDto>> reserveRoomCache;
        private readonly IRepository<RoomType, Guid> _roomTypeRepository;
        private readonly IRepository<RoomNummber, Guid> roomnumberreposi;
        private readonly IRepository<MoneyDetail, Guid>  monrydetails;
        private readonly IExportAppService _exportAppService;

        public ReserveRoomServer(IRepository<ReserveRoom, Guid> reserveRoomRepository, IDistributedCache<List<ReserveRoomShowDto>> reserveRoomCache, IRepository<RoomType, Guid> roomTypeRepository, IRepository<RoomNummber, Guid> roomnumberreposi, IRepository<MoneyDetail, Guid> monrydetails, IExportAppService exportAppService)
        {
            this.reserveRoomRepository = reserveRoomRepository;
            this.reserveRoomCache = reserveRoomCache;
            _roomTypeRepository = roomTypeRepository;
            this.roomnumberreposi = roomnumberreposi;
            this.monrydetails = monrydetails;
            _exportAppService = exportAppService;
        }


        /// <summary>
        /// 房间类型列表（查询所有房型）
        /// </summary>
        /// <returns>房型实体列表</returns>
        /// <remarks>
        /// 1. 查询所有房型。
        /// 2. 返回ApiResult包装的房型列表。
        /// 3. 捕获异常并抛出。
        /// </remarks>
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
        /// 房间预定添加（支持事务、DTO映射、房态变更、金额明细记录、缓存清理）
        /// </summary>
        /// <param name="room">预定房间DTO</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 1. 开启事务，保证预定、金额明细、房态变更原子性。
        /// 2. 遍历每个预定房型，校验ID有效性。
        /// 3. DTO映射为实体，插入预定表。
        /// 4. 插入金额明细表。
        /// 5. 修改房号状态为"预定"。
        /// 6. 清理缓存。
        /// 7. 提交事务。
        /// 8. 捕获异常并返回详细错误信息。
        /// </remarks>
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
                        reserveRoom.RoomTypeid = item.Id;
                        reserveRoom.Price = item.Price;
                        //reserveRoom.RoomNum = "未排房";
                        // 建议补充 BreakfastNum 字段赋值
                        reserveRoom.BreakfastNum = 0;
                        reserveRoom.CreatorId=room.UserId; // 设置创建者ID
                        var res= await reserveRoomRepository.InsertAsync(reserveRoom);
                        var moneyDetail = new MoneyDetail
                        {
                            BookingNumber = res.Id.ToString(),
                            BusinesName = "房间",
                            Money = res.Price,
                            Money1 = 0,
                            States = 2,
                             LoginName = room.UserId.ToString(), // 设置操作人
                              CreatorId = room.UserId, // 设置创建者ID
                        };
                        await monrydetails.InsertAsync(moneyDetail);
                        var num=await roomnumberreposi.FirstOrDefaultAsync(x => x.RoomNum==res.RoomNum);
                        num.RoomState = 4;
                        await roomnumberreposi.UpdateAsync(num);
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
        /// 预定房间信息（支持缓存、DTO组装、条件筛选、分页）
        /// </summary>
        /// <param name="search1">查询条件</param>
        /// <param name="seach">分页参数</param>
        /// <returns>分页后的预定房间DTO列表</returns>
        /// <remarks>
        /// 1. 优先从缓存获取预定信息。
        /// 2. 若缓存未命中则查询数据库并组装DTO。
        /// 3. 支持多条件筛选（状态、时间、关键字等）。
        /// 4. 支持分页。
        /// </remarks>
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
                              join roomType in roomTypes on reserveRoom.RoomTypeid equals roomType.Id
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
                                  RoomTypeid = roomType.Id,
                                  RoomTypeName = roomType.Name, // 房型名称
                                  BreakfastNum = reserveRoom.BreakfastNum,
                                  Price = reserveRoom.Price,
                                  Status = reserveRoom.Status,
                                  IdCard = reserveRoom.IdCard,
                                  RoomNum = reserveRoom.RoomNum,
                                  Message = reserveRoom.Message,
                                   PayStatus= reserveRoom.PayStatus
                              };
                return listdto.ToList();

            }, () => new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(30) // 设置缓存过期时间为30分钟
            });
            // 多条件筛选
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
        /// 排房（为预定分配房号）
        /// </summary>
        /// <param name="dto">排房DTO</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 1. 根据预定ID查找预定信息。
        /// 2. 更新房号和操作人。
        /// 3. 更新数据库。
        /// 4. 清理缓存。
        /// </remarks>
        public async Task<ApiResult> UpdateTerraces(UpdateDto dto)
        {
            var list = await reserveRoomRepository.GetAsync(dto.id);
            if (list == null)
            {
                return ApiResult.Fail("未找到该预定信息", ResultCode.Error);
            }

            list.RoomNum = dto.roomnum; // 更新房间号码
            list.CreatorId = dto.Userid; // 更新操作人ID
            await reserveRoomRepository.UpdateAsync(list);
            // 成功后，清理缓存
            await reserveRoomCache.RemoveAsync("GetReserRoom");
            return ApiResult.Success(ResultCode.Success);
        }
        /// <summary>
        /// 房间号列表（根据房型ID查询）
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult<List<RoomNumDto>>> GetRoomNmlist(Guid guid)
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
        /// 取消排房（将房号重置为"未排房"）
        /// </summary>
        /// <param name="id">预定ID</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 1. 根据预定ID查找预定信息。
        /// 2. 重置房号。
        /// 3. 更新数据库。
        /// 4. 清理缓存。
        /// </remarks>
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
        /// 取消预定（支持原因记录）
        /// </summary>
        /// <param name="dto">取消预定DTO</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 1. 根据预定ID查找预定信息。
        /// 2. 设置状态为已取消，记录原因。
        /// 3. 更新操作人。
        /// 4. 更新数据库。
        /// 5. 清理缓存。
        /// </remarks>
        public async Task<ApiResult> UpdateNotReserc(UpdateNoReserDto dto)
        {
            var list = await reserveRoomRepository.GetAsync(dto.id);
            if (list == null)
            {
                return ApiResult.Fail("未找到该预定信息", ResultCode.Error);
            }

            list.Status = 5; // 设置为已取消状态
            list.NoReservRoom = dto.NoReservRoom;
            list.CreatorId = dto.Userid; // 更新操作人ID
            await reserveRoomRepository.UpdateAsync(list);
            // 成功后，清理缓存
            await reserveRoomCache.RemoveAsync("GetReserRoom");
            return ApiResult.Success(ResultCode.Success);
        }

        /// <summary>
        /// 入住（支持信息补全）
        /// </summary>
        /// <param name="dto">入住DTO</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 1. 根据预定ID查找预定信息。
        /// 2. 设置状态为已入住，补全入住信息。
        /// 3. 更新数据库。
        /// 4. 清理缓存。
        /// </remarks>
        public async Task<ApiResult> UpdateCheckin(Update1Dto dto)
        {
            using (var tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var list = await reserveRoomRepository.GetAsync(dto.Id);
                if (list == null)
                {
                    return ApiResult.Fail("未找到该预定信息", ResultCode.Error);
                }
                list.Status = 1; // 设置为已入住状态
                list.IdCard = dto.IdCard; // 更新身份证号码
                list.RoomNum = dto.RoomNum;
                list.ReserveName = dto.ReserveName;
                list.Phone = dto.Phone; // 更新手机号码
                list.CreatorId = dto.Userid; // 更新操作人ID
                await reserveRoomRepository.UpdateAsync(list);
                // 成功后，清理缓存
                await reserveRoomCache.RemoveAsync("GetReserRoom");

                var num = await roomnumberreposi.FirstOrDefaultAsync(x => x.RoomNum == list.RoomNum);
                num.RoomState = 4;
                await roomnumberreposi.UpdateAsync(num);

                tran.Complete();
                return ApiResult.Success(ResultCode.Success);
            }

           
        }

        /// <summary>
        /// 退房（设置状态）
        /// </summary>
        /// <param name="id">预定ID</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 1. 根据预定ID查找预定信息。
        /// 2. 设置状态为已退房。
        /// 3. 更新数据库。
        /// 4. 清理缓存。
        /// </remarks>
        public async Task<ApiResult> UpdateNoRoom(Guid id)
        {
            var list = await reserveRoomRepository.GetAsync(id);
            if (list == null)
            {
                return ApiResult.Fail("未找到该预定信息", ResultCode.Error);
            }

            list.Status = 2; // 设置为已退房状态
            await reserveRoomRepository.UpdateAsync(list);
            // 成功后，清理缓存
            await reserveRoomCache.RemoveAsync("GetReserRoom");
            return ApiResult.Success(ResultCode.Success);
        }

        /// <summary>
        /// 结算（支持事务、金额明细记录、缓存清理）
        /// </summary>
        /// <param name="dto">结算DTO</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 1. 开启事务。
        /// 2. 根据预定编号查找预定信息。
        /// 3. 设置状态为已结算。
        /// 4. 更新数据库。
        /// 5. 清理缓存。
        /// 6. 查找并更新金额明细。
        /// 7. 新增结算金额明细。
        /// 8. 提交事务。
        /// </remarks>
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
                    list.Status = 3; // 设置为已结算状态
                    await monrydetails.UpdateAsync(list123);
                }
               

                var moneyDetail = ObjectMapper.Map<MoneyDetailDto, MoneyDetail>(dto);
                moneyDetail.States=2; // 设置为已结算状态
                await monrydetails.InsertAsync(moneyDetail);

                tran.Complete(); // 提交事务
                return ApiResult.Success(ResultCode.Success);
            }
        }

        /// <summary>
        /// 查询单个预定详情（根据ID）
        /// </summary>
        /// <param name="id">预定ID</param>
        /// <returns>预定详情DTO</returns>
        /// <remarks>
        /// 1. 查询所有预定信息。
        /// 2. 查询所有房型。
        /// 3. 联表组装DTO。
        /// 4. 返回单个对象。
        /// </remarks>
        [HttpGet]
        public async Task<ApiResult<ReserveRoomShowDto>> ShowFanReserveRoom(Guid id)
        {
            // 查询数据库
            // 预订信息
            var list = await reserveRoomRepository.GetListAsync(x => x.Id == id);
            // 房间类型
            var roomTypes = await _roomTypeRepository.GetListAsync();

            var listdto = (from reserveRoom in list
                           join roomType in roomTypes on reserveRoom.RoomTypeid equals roomType.Id
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
                               RoomTypeid = roomType.Id,
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

        /// <summary>
        /// 导出所有住房记录数据（支持DTO组装、字段映射、调用导出服务）
        /// </summary>
        /// <returns>Excel流内容</returns>
        /// <remarks>
        /// 1. 查询所有客户和客户类型。
        /// 2. 联表组装DTO。
        /// 3. 构造导出数据结构，设置字段映射。
        /// 4. 调用导出服务生成Excel流。
        /// </remarks>
        public async Task<IRemoteStreamContent> ExportAllReserverAsync()
        {
            // 获取数据
            var list = await reserveRoomRepository.GetListAsync();
            // 构造导出数据结构
            var exportData = new ExportDataDto<ReserveRoom>
            {
                FileName = "住宿管理",
                Items = list.ToList(), // 将查询结果转换为列表并赋值给 Items
                ColumnMappings = new Dictionary<string, string>
                {
                    { "Id", "预定ID" },
                    { "Infomation", "客源信息" },
                    { "Ordersource", "订单来源" },
                    { "ReserveName", "预定姓名" },
                    { "Phone", "手机号" },
                    { "Day", "入住天数" },
                    { "RoomTypeName", "房型名称" },
                    { "Price", "价格" },
                    { "Status", "状态" },
                    { "RoomNum", "房间号" },
                    { "Message", "备注" }
                }
            };
            // 返回导出的内容
            return await _exportAppService.ExportToExcelAsync(exportData);
        }
    }
}
