using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.RoomPriceCalendarService
{
    public class UpdateRoomPriceCalendarsDto
    {
        
       // public  Guid PriceId { get;  set; }
        /// <summary>
        /// 最低价
        /// </summary>
        public decimal? MinPrice { get; set; }

        /// <summary>最高价</summary>
        public decimal? MaxPrice { get; set; }
        /// <summary>
        /// 房间类型ID
        /// </summary>
        public Guid RoomTypeId { get; set; }
        /// <summary>
        /// 价格日历ID
        /// </summary>
        public  Guid CalendarsId { get; set; }
        /// <summary>
        /// 日历日期
        /// </summary>
        public DateTime CalendarDate { get; set; }
        /// <summary>
        /// 日历价格
        /// </summary>
        public decimal CalendarPrice { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
    }
}
