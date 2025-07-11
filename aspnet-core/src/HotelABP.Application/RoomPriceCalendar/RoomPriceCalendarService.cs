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
            var roomPrices = await db.Queryable<RoomPrice>().ToListAsync();

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
                var startDate = DateTime.Now.Date;
                var days = DateTime.DaysInMonth(startDate.Year, startDate.Month); // 本月天数

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

        //public async Task<ApiResult<int>> UpdateRoomPriceAsync(UpdateRoomPriceDto dto)
        //{
        //    1.根据 CalendarsId 查询对应的 RoomPriceCalendars 记录
        //    var entity = await db.Queryable<RoomPriceCalendars>()
        //        .FirstAsync(x => x.Id == dto.CalendarsId);
        //    if (entity == null)
        //    {
        //        return ApiResult<int>.Fail(ResultCode.NotFound, "未找到对应的日历价格记录");
        //    }

        //    2.修改价格
        //    entity.CalendarPrice = dto.NewPrice;

        //    3.更新到数据库
        //    var result = await db.Updateable(entity).ExecuteCommandAsync();
        //    return ApiResult<int>.Success(result, ResultCode.Success);
        //}
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

     

    }
}
