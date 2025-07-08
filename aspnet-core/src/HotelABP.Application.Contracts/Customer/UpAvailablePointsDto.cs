using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.Customer
{
    public class UpAvailablePointsDto
    {
        public Guid Id { get; set; }
        public decimal AvailablePoints { get; set; } = 1;
        /// <summary>
        /// 累计积分
        /// </summary>
        public decimal Accumulativeintegral { get; set; } = 0;
        /// <summary>
        /// 积分备注
        /// </summary>
        public string Pointsmodifydesc { get; set; } = string.Empty;
    }
}
