using HotelABP.RoomNummbers;
using HotelABP.RoomReserves;
using HotelABP.RoomTypes.States;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HotelABP.RoomTypes
{
    /// <summary>
    /// 房态管理
    /// </summary>
    [IgnoreAntiforgeryToken]
    [ApiExplorerSettings(GroupName = "roomstate")]
    public class RoomStateServices: ApplicationService, IRoomStateServices
    {

        private readonly IRepository<RoomType, Guid> _roomTypeRepository;
        private readonly IRepository<ReserveRoom, Guid> _reserveRoomRepository;
        private readonly IRepository<RoomNummber, Guid> _roomNummberRepository;

        public RoomStateServices(IRepository<RoomType, Guid> roomTypeRepository, IRepository<ReserveRoom, Guid> reserveRoomRepository, IRepository<RoomNummber, Guid> roomNummberRepository)
        {
            _roomTypeRepository = roomTypeRepository;
            _reserveRoomRepository = reserveRoomRepository;
            _roomNummberRepository = roomNummberRepository;
        }
        /// <summary>
        /// 获取房型状态（1设为净房，2设为脏房，3设为维修，4设为预定，5设为在住，6设为保留，7设为空房）
        /// </summary>
        /// <param name="id">房号ID</param>
        /// <param name="state">房态编号（1-7）</param>
        /// <returns>房型与预定信息DTO</returns>
        /// <remarks>
        /// 1. 根据房号ID查找房号实体，并更新其房态字段。
        /// 2. 更新房号实体到数据库。
        /// 3. 查询与该房号ID相同的房型信息。
        /// 4. 查询所有预定信息。
        /// 5. 通过LINQ将房型与预定信息进行关联，组装为DTO。
        /// 6. 返回组装好的DTO。
        /// </remarks>
        public async Task<ApiResult<RoomTypeOrReserveRoomDto>> UpdateRoomTypeState(Guid id, int state)
        {
            // 1. 查找房号实体并更新房态
            var roomState = await _roomNummberRepository.FindAsync(id);
            roomState.RoomState = state;
            await _roomNummberRepository.UpdateAsync(roomState);

            // 2. 查询与该房号ID相同的房型信息
            var roomType = await _roomTypeRepository.GetListAsync(x=>x.Id==id);
            // 3. 查询所有预定信息
            var reserveRoom = await _reserveRoomRepository.GetListAsync();

            // 4. 组装DTO，房型与预定信息关联
            var dto = (from type in roomType
                      join reserve in reserveRoom on type.Id.ToString() equals reserve.RoomTypeid
                      select new RoomTypeOrReserveRoomDto
                      {
                          Id = type.Id,
                          Name = type.Name,
                          TypeState = type.State,
                          Infomation = reserve.Infomation,
                          Ordersource = reserve.Ordersource,
                          ReserveName = reserve.ReserveName,
                          Phone = reserve.Phone,
                          BookingNumber = reserve.BookingNumber,
                          Sdate = reserve.Sdate,
                          Edate = reserve.Edate,
                          Day = reserve.Day,
                          RoomTypeid = reserve.RoomTypeid
                      }).FirstOrDefault();

            // 5. 返回结果
            return ApiResult<RoomTypeOrReserveRoomDto>.Success(dto, ResultCode.Success);
        }
        /// <summary>
        /// 根据房型名称对房号进行分组并对状态进行查询
        /// </summary>
        /// <param name="SearchDto">查询条件DTO</param>
        /// <returns>分组后的房型及房号状态列表</returns>
        /// <remarks>
        /// 1. 查询所有房型。
        /// 2. 查询所有房号。
        /// 3. 按TypeName筛选房型（如有条件）。
        /// 4. 遍历房型，将其下所有房号分组组装为DTO。
        /// 5. 如有状态筛选条件，则对房号按TypeState进一步筛选。
        /// 6. 返回分组后的结果。
        /// </remarks>
        public async Task<ApiResult<List<RoomTypeOrRoomNumGroupDto>>> GetRoomTypeList(RoomTypeOrRoomStateSearchDto SearchDto)
        {
            // 1. 获取所有房型
            var roomTypes = await _roomTypeRepository.GetListAsync();
            // 获取所有房号（假设有房号仓储，需注入 IRepository<RoomNummber, Guid> _roomNummberRepository）
            var roomNummberRepository = LazyServiceProvider.LazyGetRequiredService<IRepository<RoomNummber, Guid>>();
            var roomNumms = await roomNummberRepository.GetListAsync();
          
            // 3. 先根据TypeName筛选房型
            if (!string.IsNullOrEmpty(SearchDto.TypeName))
            {
                roomTypes = roomTypes.Where(x => x.Name.Contains(SearchDto.TypeName)).ToList();
            }

            // 4. 遍历房型，组装分组DTO
            var groupList = roomTypes.Select(type => new RoomTypeOrRoomNumGroupDto
            {
                TypeName = type.Name,
                //TypeState = type.State,
                Rooms = roomNumms.Where(r => r.RoomTypeId == type.Id.ToString())
                    .Select(r => new RoomNummDto
                    {
                        Id = r.Id,
                        RoomTypeId = r.RoomTypeId,
                        TypeName = type.Name,
                        RoomNum = r.RoomNum,
                        //TypeState = type.State,
                        TypeState = r.RoomState,
                        Order = r.Order,
                        Description = r.Description
                    }).ToList()
            }).ToList();

            // 5. 再按房号TypeState筛选
            if (SearchDto.State >0)
            {
                groupList = groupList
                    .Select(g => {
                        g.Rooms = g.Rooms.Where(r => r.TypeState == SearchDto.State).ToList();
                        return g;
                    })
                    .Where(g => g.Rooms.Count > 0)
                    .ToList();
            }

            // 6. 返回分组结果
            return ApiResult<List<RoomTypeOrRoomNumGroupDto>>.Success(groupList, ResultCode.Success);
        }
    }
}
