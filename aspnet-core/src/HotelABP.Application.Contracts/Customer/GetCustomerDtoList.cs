using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.Customer
{
    /// <summary>
    /// 客户列表查询参数
    /// </summary>
    public class GetCustomerDtoList
    {
        /// <summary>
        ///   客户昵称
        /// </summary>
        public string CustomerNickName { get; set; }=string.Empty;
        /// <summary>
        /// 客户类型（0 = 会员，1 = 普通客户）
        /// </summary>
        public Guid? CustomerType { get; set; }

        /// <summary>
        /// 客户姓名（必填，最多16个字符）
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// 手机号（必填，格式验证）
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// 性别（0 = 未知，1 = 男，2 = 女；可为空）
        /// </summary>
        public int? Gender { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        ///  结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

       
    }
}
