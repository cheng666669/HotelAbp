using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.Store
{
    public class CreateUpdateStoreDto
    {
        public string StoreName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string StoreImg { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public bool Status { get; set; }
        public string Introduction { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public Guid? UserId { get; set; }//创建人
    }
}
