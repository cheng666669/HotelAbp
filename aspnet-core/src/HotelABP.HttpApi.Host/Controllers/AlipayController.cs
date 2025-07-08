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

        /// <summary>
        /// 创建支付宝支付请求，生成支付页面HTML。
        /// </summary>
        /// <param name="orderId">订单编号，唯一标识本次支付的订单。</param>
        /// <param name="subject">订单标题或商品名称，显示在支付宝支付页面。</param>
        /// <param name="amount">支付金额，单位为元，精确到小数点后两位。</param>
        /// <returns>
        /// 返回支付宝支付页面的HTML内容，前端可直接渲染跳转到支付宝支付。
        /// </returns>
        [HttpPost("pay")]
        public async Task<IActionResult> Pay(string orderId, string subject, decimal amount)
        {
            // 调用支付宝服务，生成支付页面HTML（包含自动跳转到支付宝的表单）
            var html = await _alipayService.CreatePaymentUrlAsync(orderId, subject, amount);
            // 以text/html格式返回HTML内容，前端可直接渲染
            return Content(html, "text/html");
        }

        /// <summary>
        /// 支付宝支付异步通知回调接口，处理支付结果。
        /// </summary>
        /// <returns>
        /// 返回字符串"success"，表示回调已处理，支付宝不会再次重试。
        /// </returns>
        [HttpPost("notify")]
        public async Task<IActionResult> Notify()
        {
            // 读取支付宝回调的表单参数（包含订单号、支付状态等）
            var form = await Request.ReadFormAsync();

            // 打印日志，记录支付宝回调的所有参数，便于排查问题
            Logger.LogInformation("支付宝回调参数：" + JsonSerializer.Serialize(form));

            // 获取订单号（out_trade_no）和支付状态（trade_status）
            var outTradeNo = form["out_trade_no"];
            var tradeStatus = form["trade_status"];

            // 判断支付状态是否为成功
            if (tradeStatus == "TRADE_SUCCESS")
            {
                // 根据支付宝回调的订单号查找本地订单（注意：outTradeNo应与本地订单唯一标识一致）
                var reserveRoom = await _roomReserveRepository.FirstOrDefaultAsync(
                    r => r.Id.ToString() == outTradeNo.ToString() // 这里假设outTradeNo就是主键Id
                );

                // 如果未找到订单，记录警告日志，返回success（支付宝不会重试）
                if (reserveRoom == null)
                {
                    Logger.LogWarning("未找到订单：" + outTradeNo);
                    return Content("success"); // 回调正常结束
                }

                // 幂等性处理：如果订单未支付，则更新支付状态
                if (reserveRoom.PayStatus != 1) // 1表示已支付
                {
                    reserveRoom.PayStatus = 1; // 标记为已支付
                    await _roomReserveRepository.UpdateAsync(reserveRoom,true); // 更新数据库
                    // 删除Redis缓存，保证数据库和缓存数据一致
                    await _reserveRoomCache.RemoveAsync("GetReserRoom");
                    Logger.LogInformation("订单状态更新成功并清理缓存：" + outTradeNo);
                }
            }

            // 返回success，告知支付宝回调已处理完毕
            return Content("success");
        }

    }


}
