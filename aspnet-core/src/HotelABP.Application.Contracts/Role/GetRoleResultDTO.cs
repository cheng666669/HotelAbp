using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace HotelABP.Role
{
    public class GetRoleResultDTO:FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; } = string.Empty;
        ///// <summary>
        ///// 权限Id
        ///// </summary>
        //public List<Guid> PermissionIds { get; set; } = new List<Guid>();

    }
}
