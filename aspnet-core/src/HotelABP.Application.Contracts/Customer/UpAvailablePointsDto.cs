using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.Customer
{
    /// <summary>
    /// 修改会员可用积分
    /// </summary>
    public class UpAvailablePointsDto
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 可用积分（必须大于0，最多10亿，仅会员有效）
        /// </summary>
        //  [Range(1, 1000000000, ErrorMessage = "积分必须大于0且不超过10亿")]
        public decimal? AvailablePoints { get; set; }
        /// <summary>
        /// 累计积分
        /// </summary>
        public decimal? Accumulativeintegral { get; set; } = 1;
        /// <summary>
        /// 积分备注
        /// </summary>
        public string? Pointsmodifydesc { get; set; } = string.Empty;
    }
}
