using HotelABP.RoomNummbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.RoomTypes.States
{
    public class RoomTypeOrRoomNumGroupDto
    {
        public string TypeName { get; set; }           // 房型名称
        public List<RoomNummDto> Rooms { get; set; }   // 该房型下的房号列表
    }
}
