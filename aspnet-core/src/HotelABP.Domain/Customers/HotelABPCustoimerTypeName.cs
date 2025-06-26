using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace HotelABP.Customers
{
    public class HotelABPCustoimerTypeName:FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 客户类型名称
        /// </summary>
        public string? CustomerTypeName { get; set; }
    }
}
