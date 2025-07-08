using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.Customer
{
    public class UpStautsDto
    {
        public List<Guid> ids { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; } 
    }
}
