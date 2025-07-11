using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.Customer
{
    /// <summary>
    /// 标签列表 DTO
    /// </summary>
    public class LabelListDto
    {
        /// <summary>
        /// 标签 ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 标签名称
        /// </summary>
        public string LabelName { get; set; }

        /// <summary>
        /// 标签类型
        /// </summary>
        public int? TagType { get; set; }

        /// <summary>
        /// 人数
        /// </summary>
        public int? PeopleNumber { get; set; }
    }
}
