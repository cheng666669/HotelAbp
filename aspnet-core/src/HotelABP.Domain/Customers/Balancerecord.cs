using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace HotelABP.Customers
{
    public class Balancerecord: CreationAuditedEntity<Guid>
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string? Phone { get; set; } = string.Empty;
        /// <summary>
        /// 客户昵称
        /// </summary>
        public string? CustomerNockName { get; set; } = string.Empty;
        /// <summary>
        /// 客户名称
        /// </summary>
        public string? CustomerName { get; set; } = string.Empty;
        /// <summary>
        /// 变动余额
        /// </summary>
        public decimal? SlidingPrice { get; set; } 
        /// <summary>
        /// 变动金额
        /// </summary>
        public decimal? ChangePrice { get; set; } 
        /// <summary>
        /// 会员卡号
        /// </summary>
        public Guid? Id { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string? Ordernumber { get; set; }= string.Empty;
        /// <summary>
        /// 操作人
        /// </summary>

        public string? Operator { get; set; }= string.Empty;
        /// <summary>
        /// 操作人id
        /// </summary>
        public Guid? OperatorId { get; set; }

    }
}
