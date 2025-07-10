using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.Store
{
    public class SearchStoreDto
    {
        public string Address { get; set; } = string.Empty;
        public bool? Status { get; set; }
        public string StoreName { get; set; } = string.Empty;
    }
}
