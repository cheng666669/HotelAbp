using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace HotelABP.RoomNummbers
{
    public class CreateUpdataRoomNummDto
    {
       

        /// <summary>
        /// 房型Id
        /// </summary>
        public string RoomTypeId { get; set; }
        /// <summary>
        /// 房号
        /// </summary>
        public string RoomNum { get; set; }
        public bool State { get; set; } = true;
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
