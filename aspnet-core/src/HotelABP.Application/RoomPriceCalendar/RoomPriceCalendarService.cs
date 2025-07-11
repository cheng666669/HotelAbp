using Dm.util;
using HotelABP.RoomPriceCalendarService;
using HotelABP.RoomTypes;
using HotelABP.RoomTypes.Types;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HotelABP.RoomPriceCalendar
{
    /// <summary>
    /// 房型价格日历
    /// </summary>
    [ApiExplorerSettings(GroupName = "roomPriceCalendar")]
    
    public class RoomPriceCalendarService:ApplicationService, IRoomPriceCalendarService
    {
        private readonly ISqlSugarClient db;
        IRepository<RoomType> typeRep;

        public RoomPriceCalendarService(ISqlSugarClient db, IRepository<RoomType> typeRep)
        {
            this.db = db;
            this.typeRep = typeRep;
        }
        /// <summary>
        /// 获取房型价格列表
        /// </summary>
        /// <param name="TypeName"></param>
        /// <returns></returns>
        public async Task<ApiResult<List<RoomTypeOrRoomPriceDto>>> GetListAsync(string? TypeName)
        {
            // 1. 用 EF Core 查询 RoomType
            var roomTypes = await typeRep.GetListAsync();

            // 2. 用 SqlSugar 查询 RoomPrice
            var roomPrices = await db.Queryable<RoomPrice>()
                .Where(x => x.IsDeleted == false)
                .ToListAsync();

            // 3. 根据 TypeName 过滤
            if (!string.IsNullOrEmpty(TypeName))
            {
                roomTypes = roomTypes.Where(rt => rt.Name == TypeName).ToList();
            }

            // 4. 在内存中用 LINQ join
            var result = (from rt in roomTypes
                        join rp in roomPrices on rt.Id equals rp.RoomTypeId
                        select new RoomTypeOrRoomPriceDto
                        {
                            Id = rp.Id,
                            RoomTypeId = rt.Id,
                            TypeName = rt.Name,
                            TypePrice = rt.Price,
                            MaxOccupancy = rt.MaxOccupancy,
                            ProductName = rp.ProductName,
                            BreakfastCount = rp.BreakfastCount,
                            SaleStrategy = rp.SaleStrategy,
                            PaymentType = rp.PaymentType,
                            Preferential = rp.Preferential,
                            MemberPriceSpread = rp.MemberPriceSpread,
                            MinPrice = rp.MinPrice,
                            MaxPrice = rp.MaxPrice,
                            CalendarStatus = rp.CalendarStatus,
                            Sort = rp.Sort
                        }).ToList();


            return ApiResult<List<RoomTypeOrRoomPriceDto>>.Success(result, ResultCode.Success);
        }
        /// <summary>
        /// 添加房型价格
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ApiResult<int>> AddRoomPriceAsync(CreateRoomPriceDto input)
        {
            // 1. 查询该房型最早的日历日期
            var minCalendarDate = await db.Queryable<RoomPriceCalendars>()
                .Where(x => x.RoomTypeId == input.RoomTypeId && x.IsDeleted == false)
                .OrderBy(x => x.CalendarDate)
                .Select(x => x.CalendarDate)
                .FirstAsync();

            // 如果没有记录，默认用当前日期
            var startDate = minCalendarDate != default(DateTime) ? minCalendarDate.Date : DateTime.Now.Date;
            var days = DateTime.DaysInMonth(startDate.Year, startDate.Month);
            var endDate = startDate.AddDays(days - 1);

            // 2. 判断本月是否已存在房价
            bool exists = await db.Queryable<RoomPriceCalendars>()
                .AnyAsync(x => x.RoomTypeId == input.RoomTypeId && x.CalendarDate >= startDate && x.CalendarDate <= endDate && x.IsDeleted == false);

            if (exists)
            {
                return ApiResult<int>.Fail("该房型本月已经添加过房价", ResultCode.Error);
            }

            // UseTranAsync 是 SqlSugar 框架提供的事务方法，用于在一段代码块内自动开启数据库事务，确保代码块内的所有数据库操作要么全部成功、要么全部失败（回滚）
            var result = await db.Ado.UseTranAsync(async () =>
            {
                // 2. 查询 RoomType 的 Price
                var roomTypes = await typeRep.FirstAsync(rt => rt.Id == input.RoomTypeId);
                

                // 1. 插入 RoomPrice
                var entity = new RoomPrice
                {
                    RoomTypeId = input.RoomTypeId,
                    ProductName = input.ProductName,
                    BreakfastCount = input.BreakfastCount,
                    SaleStrategy = input.SaleStrategy,
                    PaymentType = input.PaymentType,
                    Preferential = input.Preferential,
                    MemberPriceSpread = input.MemberPriceSpread,
                    MinPrice = roomTypes.Price,
                    MaxPrice = roomTypes.Price,
                    CalendarStatus = input.CalendarStatus,
                    Sort = input.Sort,
                    CreationTime= DateTime.Now,
                };
                await db.Insertable(entity).ExecuteCommandAsync();

              

                // 3. 批量插入 RoomPriceCalendars（一个月）
                var calendars = new List<RoomPriceCalendars>();
                
                for (int i = 0; i < days; i++)
                {
                    calendars.Add(new RoomPriceCalendars
                    {
                        RoomTypeId = input.RoomTypeId,
                        CalendarPrice = roomTypes.Price,
                        CalendarDate = startDate.AddDays(i),
                        CreationTime = DateTime.Now,
                    });

                }

                await db.Insertable(calendars).ExecuteCommandAsync();
            });

            if (result.IsSuccess)
                return ApiResult<int>.Success(1, ResultCode.Success);
            else
                return ApiResult<int>.Fail("新增房价失败",ResultCode.Error);
        }
        /// <summary>
        /// 修改房价
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ApiResult<int>> UpdataRoomPriceAsync(UpdateRoomPriceDto dto)
        {

        }
        /// <summary>
        /// 修改房价状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="CalendarStatus"></param>
        /// <returns></returns>
        public async Task<ApiResult<bool>> UpdateRoomPriceState(Guid id, bool CalendarStatus)
        {
            // 1. 查询 RoomPrice 是否存在
            var roomPrice = await db.Queryable<RoomPrice>().FirstAsync(x => x.Id == id);
            if (roomPrice == null)
            {
                return ApiResult<bool>.Fail("未找到对应房价", ResultCode.Error);
            }

            // 使用 SqlSugar 的 Updateable 进行房价状态的更新
            // SetColumns 用于设置需要更新的字段，这里将 CalendarStatus 字段更新为传入的 CalendarStatus 参数
            // Where 用于指定更新条件，这里根据房价的主键 Id 进行定位
            // ExecuteCommandAsync 执行更新操作，返回受影响的行数
            var updateCount = await db.Updateable<RoomPrice>()
                .SetColumns(x => x.CalendarStatus == CalendarStatus) // 设置房价状态
                .Where(x => x.Id == id) // 指定要更新的房价记录
                .ExecuteCommandAsync(); // 执行更新操作

            if (updateCount > 0)
            {
                return ApiResult<bool>.Success(true, ResultCode.Success);
            }
            else
            {
                return ApiResult<bool>.Fail("状态更新失败", ResultCode.Error);
            }
        }
        /// <summary>
        /// 删除房价
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ApiResult<bool>> DeleteRoomPrice(Guid id)
        {
           var roomPrice= await db.Updateable<RoomPrice>()
                .SetColumns(x => x.IsDeleted == true)
                .Where(x => x.Id == id)
                .ExecuteCommandAsync();
            if (roomPrice > 0)
            {
                return ApiResult<bool>.Success(true, ResultCode.Success);
            }
            else
            {
                return ApiResult<bool>.Fail("删除失败", ResultCode.Error);
            }
        }
        /// <summary>
        /// 获取房型价格日历
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ApiResult<List<RoomPriceCalendars>>> GetRoomPriceCalendarsAsync(GetRoomPriceCalendarsDto dto)
        {
            var query = db.Queryable<RoomPriceCalendars>();

            if (dto.roomTypeId!=null)
                query = query.Where(x => x.RoomTypeId == dto.roomTypeId);
            var list = await query.ToListAsync();
            return ApiResult<List<RoomPriceCalendars>>.Success(list, ResultCode.Success);
        }
        /// <summary>
        /// 修改日历价格
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ApiResult<bool>> UpdateRoomPriceCalendarsAsync(UpdateRoomPriceCalendarsDto dto)
        {
            var result = await db.Ado.UseTranAsync(async () =>
            {
                // 1. 修改 RoomPriceCalendars 某一天的价格
                var calendar = await db.Queryable<RoomPriceCalendars>().FirstAsync(x => x.Id == dto.CalendarsId);
                if (calendar == null)
                    throw new Exception("未找到对应的日历记录");
                calendar.CalendarPrice = dto.CalendarPrice;
                await db.Updateable(calendar).ExecuteCommandAsync();

                // 2. 查出该 RoomTypeId 下所有日历价格，升序排序
                var allPrices = await db.Queryable<RoomPriceCalendars>()
                    .Where(x => x.RoomTypeId == calendar.RoomTypeId && x.IsDeleted == false)
                    .Select(x => x.CalendarPrice)
                    .ToListAsync();
                if (allPrices == null || allPrices.Count == 0)
                    throw new Exception("未找到该房型的日历价格数据");
                var minPrice = allPrices.Min();
                var maxPrice = allPrices.Max();

                // 3. 更新 RoomPrice 表中的 MinPrice 和 MaxPrice
                await db.Updateable<RoomPrice>()
                    .SetColumns(x => x.MinPrice == minPrice)
                    .SetColumns(x => x.MaxPrice == maxPrice)
                    .Where(x => x.RoomTypeId == calendar.RoomTypeId && x.IsDeleted == false)
                    .ExecuteCommandAsync();
            });

            if (result.IsSuccess)
                return ApiResult<bool>.Success(true, ResultCode.Success);
            else
                return ApiResult<bool>.Fail(result.ErrorMessage, ResultCode.Error);
        }

    }
}
