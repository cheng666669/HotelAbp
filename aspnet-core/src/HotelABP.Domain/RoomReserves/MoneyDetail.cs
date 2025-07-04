using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace HotelABP.RoomReserves
{
    public class MoneyDetail:FullAuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// 预定单号
        /// </summary>
        public string BookingNumber { get; set; }
        public DateTime GetDate { get; set; } = DateTime.Now; // 获取时间
        public string BusinesName { get; set; } //营业项目
        public decimal? Money { get; set; } // 消费金额
        public decimal? Money1 { get; set; } // 支付金额
        public DateTime GetTime { get; set; } = DateTime.Now; // 获取时间
        public int States { get; set; } // 状态 0未支付 1已支付

        /// <summary>
        /// 摘要
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string? LoginName { get; set; }

    }
}
