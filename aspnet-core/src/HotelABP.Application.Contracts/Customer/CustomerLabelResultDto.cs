using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.Customer
{
    // 添加缺失的 CustomerLabelResultDto 定义
    public class CustomerLabelResultDto
    {
        /// <summary>
        /// 标签ID
        /// </summary>
        public Guid LabelId { get; set; }

        /// <summary>
        /// 标签名称
        /// </summary>
        public string LabelName { get; set; }

        /// <summary>
        /// 标签类型
        /// </summary>
        public int? TagType { get; set; }
    }
}
