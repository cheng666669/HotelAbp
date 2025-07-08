using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace HotelABP.Label
{
    /// <summary>
    ///  标签数据
    /// </summary>
    public class GetLabelDto:FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 标签名称
        /// </summary>
        public string LabelName { get; set; } = string.Empty;
        /// <summary>
        /// 标签类型（整型存储）：
        /// 0 = 手动标签，1 = 条件标签
        /// </summary>
        public int? TagType { get; set; }

        /// <summary>
        /// 是否必须满足所有条件，适用于条件标签
        /// </summary>
        public bool MustMeetAllConditions { get; set; } = false;

        // ---------------- 时间条件 ----------------

        /// <summary>
        /// 开卡时间（可选）
        /// </summary>
        public string StartTime { get; set; } = string.Empty;

        /// <summary>
        /// 交易时间（可选）
        /// </summary>
        public string TradeTime { get; set; } = string.Empty;

        // ---------------- 会员条件 ----------------

        /// <summary>
        /// 会员等级（如普通会员）
        /// </summary>
        public string MemberLevel { get; set; } = string.Empty;

        /// <summary>
        /// 会员性别（如 男、女）
        /// </summary>
        public int? MemberGender { get; set; }

        // ---------------- 交易条件 ----------------

        /// <summary>
        /// 交易笔数最小值
        /// </summary>
        [Range(0, 100000000, ErrorMessage = "交易笔数请填写 0～1亿的整数")]
        public int? TradeCountMin { get; set; }

        /// <summary>
        /// 交易笔数最大值
        /// </summary>
        [Range(0, 100000000, ErrorMessage = "交易笔数请填写 0～1亿的整数")]
        public int? TradeCountMax { get; set; }

        /// <summary>
        /// 客单价最小值（单位：元）
        /// </summary>
        [Range(0, 990000, ErrorMessage = "客单价请填写 0～99 万的小数")]
        public decimal? AvgOrderValueMin { get; set; }

        /// <summary>
        /// 客单价最大值（单位：元）
        /// </summary>
        [Range(0, 990000, ErrorMessage = "客单价请填写 0～99 万的小数")]
        public decimal? AvgOrderValueMax { get; set; }

        /// <summary>
        /// 累计消费最小值（单位：元）
        /// </summary>
        [Range(0, 990000, ErrorMessage = "累计消费请填写 0～99 万的小数")]
        public decimal? TotalSpentMin { get; set; }

        /// <summary>
        /// 累计消费最大值（单位：元）
        /// </summary>
        [Range(0, 990000, ErrorMessage = "累计消费请填写 0～99 万的小数")]
        public decimal? TotalSpentMax { get; set; }
        /// <summary>
        /// 人数
        /// </summary>
        public int? PeopleNumber { get; set; }
    }
}
