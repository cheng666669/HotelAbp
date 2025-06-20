using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace HotelABP
{
    public class Usertinfo:FullAuditedAggregateRoot<Guid>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime LastLoginTime { get; set; } = DateTime.Now;
        public string LastLoginIp { get; set; }
    }
}
