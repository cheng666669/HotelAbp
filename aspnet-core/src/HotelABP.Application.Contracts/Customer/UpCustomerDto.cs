using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace HotelABP.Customer
{
    /// <summary>
    /// <summary>
    /// 修改客户信息
    /// </summary>
    /// </summary>
    public class UpCustomerDto
    {
        public List<Guid> ids { get; set; }
        public Guid? CustomerType { get; set; }
    }
}
