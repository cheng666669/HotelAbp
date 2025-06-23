using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HotelABP.Users
{
    public class UserRole : FullAuditedEntity<Guid>
    {
        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }
    }
}
