using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.RoomTypes.States
{
    public class RoomTypeOrRoomStateSearchDto
    {

        /// <summary>
        /// 房型名称，例如：标准大床房、豪华双床房
        /// </summary>
        public string? TypeName { get; set; }
        /// <summary>
        /// 0设为净房，1设为脏房，2设为维修，，3设为预定，4设为在住，5设为保留，6设为空房
        /// </summary>
        public int State { get; set; }
    }
}
