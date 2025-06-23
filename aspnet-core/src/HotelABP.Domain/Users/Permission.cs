using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace HotelABP.Users
{
    public class Permission:FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 父级权限Id
        /// </summary>
        public Guid ParentId { get; set; } = Guid.Empty;
        /// <summary>
        /// 权限名称
        /// </summary>
        public string PermissionName { get; set; } = string.Empty;
        /// <summary>
        /// 权限描述
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
