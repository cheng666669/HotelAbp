using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HotelABP.Users
{
    /// <summary>
    /// 用户信息表
    /// </summary>
    public class SysUser : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; } = string.Empty;
        /// <summary>
        /// 性别
        /// </summary>
        public Gender Gender { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = string.Empty;
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; } = string.Empty;
        /// <summary>
        /// 状态（1-正常，0-禁用）
        /// </summary>
        public Status Status { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}
