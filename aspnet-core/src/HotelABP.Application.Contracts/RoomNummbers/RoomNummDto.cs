using System;

namespace HotelABP.RoomNummbers
{
    public class RoomNummDto
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 房型Id
        /// </summary>
        public string RoomTypeId { get; set; }
        public string TypeName { get; set; }
        public int TypeState { get; set; }

        /// <summary>
        /// 房号
        /// </summary>
        public string RoomNum { get; set; }
        public bool State { get; set; }
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
