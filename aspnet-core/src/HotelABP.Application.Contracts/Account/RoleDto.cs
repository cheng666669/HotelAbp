using System;
using Volo.Abp.Application.Dtos;

namespace HotelABP.Account
{
    public class RoleDto:FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; } = string.Empty;
    }
}
