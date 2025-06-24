using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.RoomTypes
{
    public class GetRoomTypeDto
    {
        /// <summary>
        /// 房型名称，例如：标准大床房、豪华双床房
        /// </summary>
        public string? Name { get; set; }
    }
}
