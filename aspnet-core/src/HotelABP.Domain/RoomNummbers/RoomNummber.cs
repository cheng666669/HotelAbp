using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace HotelABP.RoomNummbers
{
    /// <summary>
    /// 房号信息
    /// </summary>
    public class RoomNummber:FullAuditedAggregateRoot<Guid>
    {

        /// <summary>
        /// 房型Id
        /// </summary>
        public string RoomTypeId { get; set; }
        /// <summary>
        /// 房号
        /// </summary>
        public string RoomNum { get; set;}
        public bool State { get; set; } = true;
        /// <summary>
        /// 房间状态
        /// </summary>
        public int RoomState { get; set; }//1设为净房，2设为脏房，3设为维修，4设为预定，5设为在住，6设为保留，7设为空房
        /// <summary>
        /// 排序
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 房间描述
        /// </summary>
        public string Description { get; set; }
    }
}
