using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.RoomPriceCalendarService
{
    public class RoomTypeOrRoomPriceDto
    {
        public Guid Id { get; set;}
        /// <summary>
        /// 房间类型ID
        /// </summary>
        public Guid? RoomTypeId { get; set; }
        /// <summary>
        /// 房型名称，例如：标准大床房、豪华双床房
        /// </summary>
        public string? TypeName { get; set; }

        /// <summary>
        /// 门市价（默认销售价格）
        /// </summary>
        public int? TypePrice { get; set; }
        /// <summary>
        /// 可入住人数（最多人数）
        /// </summary>
        public int? MaxOccupancy { get; set; }
        /// 产品名称
        /// </summary>
        public string? ProductName { get; set; }

        /// <summary>
        /// 早餐数量
        /// </summary>
        public int? BreakfastCount { get; set; }

        /// <summary>
        /// 销售策略（如“提前7天预订”、“无”）
        /// </summary>
        public string? SaleStrategy { get; set; }
        /// <summary>
        /// 付款方式（如“预订付费”、“到店付费”）
        /// </summary>
        public string? PaymentType { get; set; }
        /// <summary>
        /// 住宿优惠
        /// </summary>
        public string? Preferential { get; set; }
        /// <summary>
        /// 会员差价
        /// </summary>
        public string? MemberPriceSpread { get; set; }

        /// <summary>最低价</summary>
        public decimal? MinPrice { get; set; }

        /// <summary>最高价</summary>
        public decimal? MaxPrice { get; set; }
        /// <summary>状态（如“启用”、“停用”）</summary>
        public bool? CalendarStatus { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? Sort { get; set; }
    }
}
