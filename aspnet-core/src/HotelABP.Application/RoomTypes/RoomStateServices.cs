using HotelABP.RoomNummbers;
using HotelABP.RoomReserves;
using HotelABP.RoomTypes.States;
using HotelABP.RoomTypes.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HotelABP.RoomTypes
{
    [IgnoreAntiforgeryToken]
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
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<ApiResult<RoomTypeOrReserveRoomDto>> UpdateRoomTypeState(Guid id, int state)
        {
            var roomState = await _roomNummberRepository.FindAsync(id);
            roomState.RoomState = state;
            await _roomNummberRepository.UpdateAsync(roomState);

            var roomType = await _roomTypeRepository.GetListAsync(x=>x.Id==id);
            var reserveRoom = await _reserveRoomRepository.GetListAsync();

     

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

            return ApiResult<RoomTypeOrReserveRoomDto>.Success(dto, ResultCode.Success);
        }
        /// <summary>
        /// 根据房型名称对房号进行分组并对状态进行查询
        /// </summary>
        /// <param name="SearchDto"></param>
        /// <returns></returns>

        public async Task<ApiResult<List<RoomTypeOrRoomNumGroupDto>>> GetRoomTypeList(RoomTypeOrRoomStateSearchDto SearchDto)
        {
            // 获取所有房型
            var roomTypes = await _roomTypeRepository.GetListAsync();
            // 获取所有房号（假设有房号仓储，需注入 IRepository<RoomNummber, Guid> _roomNummberRepository）
            var roomNummberRepository = LazyServiceProvider.LazyGetRequiredService<IRepository<RoomNummber, Guid>>();
            var roomNumms = await roomNummberRepository.GetListAsync();
          
            // 先根据TypeName筛选房型
            if (!string.IsNullOrEmpty(SearchDto.TypeName))
            {
                roomTypes = roomTypes.Where(x => x.Name.Contains(SearchDto.TypeName)).ToList();
            }

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

            // 再按房号TypeState筛选
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

            return ApiResult<List<RoomTypeOrRoomNumGroupDto>>.Success(groupList, ResultCode.Success);
        }
    }
}
