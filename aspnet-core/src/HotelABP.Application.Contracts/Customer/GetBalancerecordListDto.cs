using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace HotelABP.Customer
{
    public class GetBalancerecordListDto: CreationAuditedEntity<Guid>
    {
        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string? Phone { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string? Ordernumber { get; set; }
    }
}
