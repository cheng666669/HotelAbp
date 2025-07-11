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
        public Guid RoomTypeId { get; set; }
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 早餐数量
        /// </summary>
        public int BreakfastCount { get; set; }
        /// <summary>
        /// 销售策略（如“提前7天预订”、“无”）
        /// </summary>
        public string SaleStrategy { get; set; }
        /// <summary>
        /// 付款方式（如“预订付费”、“到店付费”）
        /// </summary>
        public string    PaymentType { get; set; }
        /// <summary>
        /// 住宿优惠
        /// </summary>
        public string Preferential { get; set; }
        /// <summary>
        /// 会员差价
        /// </summary>
        public string MemberPriceSpread { get; set;}

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
