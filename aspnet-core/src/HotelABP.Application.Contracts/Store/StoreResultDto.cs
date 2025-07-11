using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace HotelABP.Store
{
    public class StoreResultDto:Entity<Guid>
    {
        public string StoreName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string StoreImg { get; set; } = string.Empty;
        public bool Status { get; set; }
        public string Mobile { get; set; } = string.Empty;
        public string Introduction { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
    }
}
