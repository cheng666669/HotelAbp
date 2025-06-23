using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.DTos.ReserveRooms
{
    public class SearchTiao
    {
        /// <summary>
        /// 入住状态
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 入住日期
        /// </summary>
        public DateTime? Sdate { get; set; }
        /// <summary>
        /// 离店日期
        /// </summary>
        public DateTime? Edate { get; set; }

        public string? Comman { get; set; }
    }
}
