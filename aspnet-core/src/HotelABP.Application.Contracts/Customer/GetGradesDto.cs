using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace HotelABP.Customer
{
    /// <summary>
    /// 获取等级列表
    /// </summary>
    public class GetGradesDto :EntityDto<Guid>
    {
        public string GradeName { get; set; }=string.Empty;
    }
}
