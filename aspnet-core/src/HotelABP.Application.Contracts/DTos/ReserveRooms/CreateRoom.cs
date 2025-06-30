using HotelABP.RoomTypes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.DTos.ReserveRooms
{
    public class CreateRoom
    {
        /// <summary>
        /// 客源信息
        /// </summary>
        public string Infomation { get; set; }
        /// <summary>
        /// 订单来源     
        /// </summary>
        public string Ordersource { get; set; }
        /// <summary>
        /// 预定姓名
        /// </summary>
        public string ReserveName { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 预定单号
        /// </summary>
        public string BookingNumber { get; set; }
        /// <summary>
        /// 入住日期
        /// </summary>
        public DateTime Sdate { get; set; }
        /// <summary>
        /// 离店日期
        /// </summary>
        public DateTime Edate { get; set; }
        /// <summary>
        /// 入住天数
        /// </summary>
        public int Day { get; set; }
        public string? Message { get; set; } // 预定备注信息
        public IList<RoomTypeDto> aaa { get; set; } // 房型信息列表

    }
}
