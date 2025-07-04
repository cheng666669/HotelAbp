using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.Customer
{
    public class UpSumofconsumptionDto
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 可用余额（必须大于0，最多95万，仅会员有效）
        /// </summary>
        public decimal AvailableBalance { get; set; }
        /// <summary>
        /// 可用赠送余额余额（必须大于0，最多95万，仅会员有效）
        /// </summary>
        public decimal AvailableGiftBalance { get; set; }
        /// <summary>
        ///  消费金额（默认为0元）
        /// </summary>
        public decimal Sumofconsumption { get; set; } = 0; // 累计消费金额，默认为0元
        /// <summary>
        /// 消费次数
        /// </summary>
        public int? ComsumerNumber { get; set; } // 消费次数
        /// <summary>
        /// 消费描述
        /// </summary>
        public string? ConsumerDesc { get; set; } = string.Empty;
    }
}
