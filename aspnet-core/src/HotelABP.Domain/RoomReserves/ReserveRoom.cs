using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace HotelABP.RoomReserves
{

    public enum ReserveRoomStatus
    {
        NotCheckedIn = 0, // 未入住
        CheckedIn = 1, // 已入住
        CheckedOut = 2, // 已离店
        Cancelled = 3 // 已取消
    }
    public class ReserveRoom:FullAuditedAggregateRoot<Guid>
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
       
        public int Status { get; set; } = 0; // 0:待入住,1:入住中,2:已退房,3:已结算,4:超时未入住5:已取消
        /// <summary>
        /// 房间号码
        /// </summary>
        public string? RoomNum { get; set; } = "未排房";

        public string? Message { get; set; } // 预定备注信息
        //身份证号
        public string? IdCard { get; set; }
        //取消预订备注
        public string? NoReservRoom { get; set; }

        public int? PayStatus {  get; set; }=0; // 0:未支付,1:已支付,2:支付失败,3:退款中,4:已退款       

    }
}
