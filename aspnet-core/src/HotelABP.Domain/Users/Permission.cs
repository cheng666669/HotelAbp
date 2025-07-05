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
        /// <summary>
        /// 菜单路由（新增）
        /// </summary>
        public string Path { get; set; } = string.Empty;
        /// <summary>
        /// 菜单图标（新增）
        /// </summary>
        public string Icon { get; set; } = string.Empty;
        /// <summary>
        /// 菜单排序（新增）
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 是否为菜单（true=菜单，false=按钮/操作）
        /// </summary>
        public bool IsMenu { get; set; }
        /// <summary>
        /// 是否显示（可选）
        /// </summary>
        public bool IsVisible { get; set; } = true;           
        
    }
}
