using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace HotelABP.Customer
{
    /// <summary>
    /// 获取客户类型名称
    /// </summary>
    public class GetCustoimerTypeNameDto: EntityDto<Guid>
    {
        public string CustomerTypeName { get; set; }
    }
}
