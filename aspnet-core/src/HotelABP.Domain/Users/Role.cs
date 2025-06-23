using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HotelABP.Users
{
    /// <summary>
    /// 角色表
    /// </summary>
    public class Role : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; } = string.Empty;
        /// <summary>
        /// 状态（1-正常，0-禁用）
        /// </summary>
        public Status Status { get; set; }
    }
}
