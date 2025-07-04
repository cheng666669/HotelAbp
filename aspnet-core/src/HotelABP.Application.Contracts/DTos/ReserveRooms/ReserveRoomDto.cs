using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.DTos.ReserveRooms
{
    public class ReserveRoomDto
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
        /// <summary>
        ///房间类型ID
        /// </summary>
        public string? RoomTypeid { get; set; }
        /// <summary>
        /// 早餐数量
        /// </summary>
        public int? BreakfastNum { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
        public int Status { get; set; } = 0; // 0:未入住,1:已入住,2:已离店,3:已取消
        /// <summary>
        /// 房间号码
        /// </summary>
        public string? RoomNum { get; set; } = "未排房";

        public string? Message { get; set; } // 预定备注信息
        //身份证号
        public string? IdCard { get; set; }
        public int? PayStatus { get; set; }
    }
}
