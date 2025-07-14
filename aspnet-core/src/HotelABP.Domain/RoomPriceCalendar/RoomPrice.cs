using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace HotelABP.RoomPriceCalendar
{
    public class RoomPrice : FullAuditedAggregateRoot<Guid>
    {

        [SugarColumn(IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }
        /// <summary>
        /// 房间类型ID
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public Guid RoomTypeId { get; set; }
        /// 产品名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ProductName { get; set; }

        /// <summary>
        /// 早餐数量
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int BreakfastCount { get; set; }

        /// <summary>
        /// 销售策略（如“提前7天预订”、“无”）
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string SaleStrategy { get; set; }
        /// <summary>
        /// 付款方式（如“预订付费”、“到店付费”）
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string PaymentType { get; set; }
        /// <summary>
        /// 住宿优惠
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Preferential { get; set; }
        /// <summary>
        /// 会员差价
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string MemberPriceSpread { get; set; }

        /// <summary>最低价</summary>
        [SugarColumn(IsNullable = true)]
        public decimal MinPrice { get; set; }

        /// <summary>最高价</summary>
        [SugarColumn(IsNullable = true)]
        public decimal MaxPrice { get; set; }

        /// <summary>状态（如“启用”、“停用”）</summary>
        [SugarColumn(IsNullable = true)]
        public bool CalendarStatus { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int Sort { get; set; }
        [SugarColumn(IsIgnore = true)]
        public new Volo.Abp.Data.ExtraPropertyDictionary ExtraProperties { get; set; }

        [SugarColumn(IsNullable = true)]
        public override Guid? CreatorId { get; set; }

        [SugarColumn(IsNullable = true)]
        public override Guid? DeleterId { get; set; }

        [SugarColumn(IsNullable = true)]
        public override Guid? LastModifierId { get; set; }

        [SugarColumn(IsNullable = true)]
        public override DateTime? DeletionTime { get; set; }

        [SugarColumn(IsNullable = true)]
        public override DateTime? LastModificationTime { get; set; }
    }
}
