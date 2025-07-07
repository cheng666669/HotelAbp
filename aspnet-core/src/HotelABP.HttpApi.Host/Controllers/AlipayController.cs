using Alipay.AopSdk.Core.Util;
using Azure.Core;
using HotelABP.ReserveRooms;
using HotelABP.RoomReserves;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using HotelABP.DTos.ReserveRooms;
using Volo.Abp.Caching;
using System.Collections.Generic;

namespace HotelABP.Controllers
{
    /// <summary>
    /// 支付宝
    /// </summary>
    [ApiExplorerSettings(GroupName = "apipay")]
    [Route("api/alipay")]
    public class AlipayController : AbpController
    {
        private readonly IAlipayService _alipayService;
        private readonly AlipayOptions _options;
        IRepository<ReserveRoom, Guid> _roomReserveRepository;
        private readonly IDistributedCache<List<ReserveRoomShowDto>> _reserveRoomCache;

        public AlipayController(IAlipayService alipayService, IRepository<ReserveRoom, Guid> roomReserveRepository, IDistributedCache<List<ReserveRoomShowDto>> reserveRoomCache)
        {
            _alipayService = alipayService;
            _roomReserveRepository = roomReserveRepository;
            _reserveRoomCache = reserveRoomCache;
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

            // 打印日志（建议）
            Logger.LogInformation("支付宝回调参数：" + JsonSerializer.Serialize(form));

            var outTradeNo = form["out_trade_no"];
            var tradeStatus = form["trade_status"];

            if (tradeStatus == "TRADE_SUCCESS")
            {
                // 用 outTradeNo 查找订单（确保是你订单表中的唯一编号）
                var reserveRoom = await _roomReserveRepository.FirstOrDefaultAsync(
                    r => r.Id.ToString() == outTradeNo.ToString() // ✅ 注意这里不要用 Id，除非你自己确实用 outTradeNo 生成的就是主键
                );

                if (reserveRoom == null)
                {
                    Logger.LogWarning("未找到订单：" + outTradeNo);
                    return Content("success"); // 回调正常结束，支付宝不再重试
                }

                if (reserveRoom.PayStatus != 1) // 幂等处理
                {
                    reserveRoom.PayStatus = 1;
                    await _roomReserveRepository.UpdateAsync(reserveRoom,true);
                    // 删除Redis缓存，保证数据库和缓存同步
                    await _reserveRoomCache.RemoveAsync("GetReserRoom");
                    Logger.LogInformation("订单状态更新成功并清理缓存：" + outTradeNo);
                }
            }

            return Content("success");
        }

    }


}
