using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace HotelABP.Users
{
    public class RolePermission:FullAuditedEntity<Guid>
    {
        public Guid RoleId { get; set; } = Guid.Empty;
        public Guid PermissionId { get; set; } = Guid.Empty;
    }
}
