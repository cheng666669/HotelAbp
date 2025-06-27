using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace HotelABP.Grades
{
    public class HotelAbpGrades:FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 会员等级名称
        /// </summary>
        public string GradeName { get; set; } = string.Empty;
    }
}
