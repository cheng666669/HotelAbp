using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace HotelABP.Customer
{
    public class UpCustomerDto
    {
        public List<Guid> ids { get; set; }
        public Guid? CustomerType { get; set; }
    }
}
