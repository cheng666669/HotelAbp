using System;
using System.ComponentModel.DataAnnotations;
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
        [Required(ErrorMessage = "用户名不能为空")]
        [MaxLength(20, ErrorMessage = "用户名长度不能超过20")]
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// 昵称
        /// </summary>
        [Required(ErrorMessage = "昵称不能为空")]
        [MaxLength(20, ErrorMessage = "昵称长度不能超过20")]
        public string NickName { get; set; } = string.Empty;
        /// <summary>
        /// 性别
        /// </summary>
        [Required(ErrorMessage = "性别不能为空")]
        public Gender Gender { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; } = string.Empty;
        /// <summary>
        /// 手机号
        /// </summary>
        [Required(ErrorMessage = "手机号不能为空")]
        public string Mobile { get; set; } = string.Empty;
        /// <summary>
        /// 状态（1-正常，0-禁用）
        /// </summary>
        [Required(ErrorMessage = "状态不能为空")]
        public Status Status { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [Required(ErrorMessage = "邮箱不能为空")]
        public string Email { get; set; } = string.Empty;
    }
}
