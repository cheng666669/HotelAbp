using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.Customer
{
    public class GetCustomerDtoList
    {
        /// <summary>
        /// 客户姓名（必填，最多16个字符）
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 手机号（必填，格式验证）
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 性别（0 = 未知，1 = 男，2 = 女；可为空）
        /// </summary>
        public int? Gender { get; set; }
    }
}
