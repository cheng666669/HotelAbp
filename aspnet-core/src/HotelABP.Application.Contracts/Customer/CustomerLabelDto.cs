using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.Customer
{
    /// <summary>
    /// 客户标签DTO
    /// </summary>
    public class CustomerLabelDto
    {
        /// <summary>
        /// 客户ID集合
        /// </summary>
        public List<Guid> CustomerIds { get; set; }

        /// <summary>
        /// 标签ID集合
        /// </summary>
        public List<Guid> LabelIds { get; set; }
    }
}
