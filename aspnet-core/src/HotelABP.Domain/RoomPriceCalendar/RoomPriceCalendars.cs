using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace HotelABP.RoomPriceCalendar
{
    public class RoomPriceCalendars: FullAuditedAggregateRoot<Guid>
    {
        [SugarColumn(IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }
        /// <summary>
        /// 房间类型ID
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public Guid RoomTypeId { get; set; }
        /// <summary>
        /// 日历日期
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime CalendarDate { get; set; }
        /// <summary>
        /// 日历价格
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public decimal CalendarPrice { get; set; }



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
