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
      //  [Range(0.01, 950000, ErrorMessage = "可用充值余额必须大于0且不超过95万")]
        public decimal? AvailableBalance { get; set; }

        /// <summary>
        /// 可用赠送余额余额（必须大于0，最多95万，仅会员有效）
        /// </summary>
      //  [Range(0.01, 950000, ErrorMessage = "可用赠送余额必须大于0且不超过95万")]
        public decimal? AvailableGiftBalance { get; set; }
        /// <summary>
        ///  消费金额（默认为0元）
        /// </summary>
        public decimal? Sumofconsumption { get; set; }  

        /// <summary>
        /// 消费次数
        /// </summary>
        public int? ComsumerNumber { get; set; } = 0; // 消费次数
        /// <summary>
        /// 消费描述
        /// </summary>
        public string? ConsumerDesc { get; set; } = string.Empty;
        /// <summary>
        ///  累计消费金额
        /// </summary>
        public decimal? Accumulativeconsumption { get; set; }

    }
}
