using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.RoomNummbers
{
    public class GetRoomNummberQuery
    {
        /// <summary>
        /// 房型Id
        /// </summary>
        public string? RoomTypeId { get; set; }

        public bool? State { get; set; }
        public string? RoomNum { get; set; }
    }
}
