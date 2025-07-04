using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.RoomReserves
{
    public class AlipayOptions
    {
        public string AppId { get; set; }
        public string PrivateKey { get; set; }
        public string AlipayPublicKey { get; set; }
        public string GatewayUrl { get; set; }
        public string NotifyUrl { get; set; }
        public string ReturnUrl { get; set; }
    }
}
