using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace HotelABP.Customer
{
    public class UpAvailableBalanceDto
    {
        public Guid Id { get; set; } // 客户ID
        public decimal AvailableBalance { get; set; }

        /// <summary>
        /// 充值金额（默认为0元）
        /// </summary>
        public decimal Rechargeamount { get; set; } = 0; // 充值金额，默认为0元
        /// <summary>
        /// 描述
        /// </summary>
        public string? CustomerDesc { get; set; } = string.Empty;
    }
}
