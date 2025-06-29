using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.DTos.ReserveRooms
{
    public class MoneyDetailDto
    {
          
      /// <summary>
      /// 预定单号
      /// </summary>
        public string BookingNumber { get; set; }

        public string BusinesName { get; set; } //营业项目
        public decimal? Money { get; set; } // 消费金额
        public decimal? Money1 { get; set; } // 支付金额
        /// <summary>
        /// 摘要
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string LoginName { get; set; }
    }
}
