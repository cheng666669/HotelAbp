using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.DTos.ReserveRooms
{
    public class UpdateDto
    {
        public Guid id { get; set; }
        public string roomnum { get; set; } // 房间号

    }
    public class Update1Dto
    {
        public Guid Id { get; set; }
        public string RoomNum { get; set; }
        public string Phone { get; set; } // 手机号
        public string ReserveName { get; set; } // 姓名

        public string IdCard { get; set; } // 身份证号
    }

    public class UpdateNoReserDto
    {
        public Guid id { get; set; }

        public string NoReservRoom { get; set; } // 取消预订备注
    }
}
