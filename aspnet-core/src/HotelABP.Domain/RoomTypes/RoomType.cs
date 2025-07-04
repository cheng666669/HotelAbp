using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace HotelABP.RoomTypes
{
    /// <summary>
    /// 房型信息
    /// </summary>
    public class RoomType : FullAuditedAggregateRoot<Guid>
    {


        /// <summary>
        /// 房型名称，例如：标准大床房、豪华双床房
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 0设为净房，1设为脏房，2设为维修，3设为预定，4设为在住，5设为保留，6设为空房
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 门市价（默认销售价格）
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// 是否需要押金（true=需要，false=不需要）
        /// </summary>
        public bool? DepositRequired { get; set; }

        /// <summary>
        /// 押金金额（仅当 DepositRequired = true 时有效）
        /// </summary>
        public decimal? DepositAmount { get; set; }

        /// <summary>
        /// 房型面积（单位：平方米）
        /// </summary>
        public int? Area { get; set; }

        /// <summary>
        /// 可入住人数（最多人数）
        /// </summary>
        public int? MaxOccupancy { get; set; }

        /// <summary>
        /// 是否预留房间（true=支持，false=不支持）
        /// </summary>
        public bool Obligate { get; set; }

        /// <summary>
        /// 加床类型（例如："不可加床"、"免费加床"、"收费加床"、"自定义"）
        /// </summary>
        public string? ExtraBedPolicy { get; set; }

        /// <summary>
        /// 房型图片路径（主图）
        /// </summary>
        public string? ImageUrls { get; set; }

        /// <summary>
        /// 房型视频路径
        /// </summary>
        public string? VideoUrl { get; set; }

        /// <summary>
        /// 房型数量
        /// </summary>
        public int RoomTypeCount { get; set; }

        /// <summary>
        /// 房型排序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 房型介绍（文字说明）
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 展示渠道，例如："官网直销"，"当日特惠"等（可以是逗号分隔）
        /// </summary>
        public string? DisplayChannels { get; set; }

        /// <summary>
        /// 配套服务列表（可为空，多个服务名称用逗号分隔或通过关系表管理）
        /// </summary>
        public string? Facilities { get; set; }
    }
}
