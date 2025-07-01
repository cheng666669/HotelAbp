using Alipay.AopSdk.Core.Util;
using Azure.Core;
using HotelABP.ReserveRooms;
using HotelABP.RoomReserves;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace HotelABP.Controllers
{
    [Route("api/alipay")]
    public class AlipayController : AbpController
    {
        private readonly IAlipayService _alipayService;
        private readonly AlipayOptions _options;

        public AlipayController(IAlipayService alipayService)
        {
            _alipayService = alipayService;
        }

        [HttpPost("pay")]
        public async Task<IActionResult> Pay(string orderId, string subject, decimal amount)
        {
            var html = await _alipayService.CreatePaymentUrlAsync(orderId, subject, amount);
            return Content(html, "text/html");
        }

        [HttpPost("notify")]
        public async Task<IActionResult> Notify()
        {
            var form = await Request.ReadFormAsync();
            // 验签略（可使用 AlipaySignature.RSACheckV1）
            var outTradeNo = form["out_trade_no"];
            var tradeStatus = form["trade_status"];

            // TODO: 根据 out_trade_no 更新数据库状态
            if (tradeStatus == "TRADE_SUCCESS")
            {
                // 修改订单状态为已支付
            }

            return Content("success");
        }
    }


}
