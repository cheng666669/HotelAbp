using Alipay.AopSdk.Core;
using Alipay.AopSdk.Core.Domain;
using Alipay.AopSdk.Core.Request;
using HotelABP.RoomReserves;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace HotelABP.ReserveRooms
{
    public interface IAlipayService
    {
        Task<string> CreatePaymentUrlAsync(string orderId, string subject, decimal amount);
    }
    /// <summary>
    /// 支付宝服务
    /// </summary>
    
    public class AlipayService : IAlipayService, ITransientDependency
    {
        private readonly IOptions<AlipayOptions> _options;

        public AlipayService(IOptions<AlipayOptions> options)
        {
            _options = options;
        }

        /// <summary>
        /// 创建支付宝支付链接（生成支付页面HTML表单）
        /// </summary>
        /// <param name="orderId">订单号</param>
        /// <param name="subject">订单标题</param>
        /// <param name="amount">支付金额</param>
        /// <returns>支付宝支付页面HTML表单字符串</returns>
        /// <remarks>
        /// 1. 读取支付宝配置参数（网关、AppId、密钥等）。
        /// 2. 构建支付宝SDK客户端。
        /// 3. 构造支付请求参数模型（订单号、金额、标题、产品码等）。
        /// 4. 构建支付请求对象，设置回调地址。
        /// 5. 调用SDK生成支付页面HTML表单。
        /// 6. 返回HTML字符串，前端可直接渲染跳转。
        /// </remarks>
        public async Task<string> CreatePaymentUrlAsync(string orderId, string subject, decimal amount)
        {
            var opt = _options.Value;

            // 1. 构建支付宝SDK客户端，传入网关、AppId、私钥、公钥等参数
            var client = new DefaultAopClient(
                opt.GatewayUrl,
                opt.AppId,
                opt.PrivateKey,
                "json",
                "1.0",
                "RSA2",
                opt.AlipayPublicKey,
                "utf-8",
                false
            );

            // 2. 构造支付参数模型
            var model = new AlipayTradePagePayModel
            {
                OutTradeNo = orderId.ToString(), // 订单号
                TotalAmount = amount.ToString("F2"), // 金额，保留两位小数
                Subject = subject, // 订单标题
                ProductCode = "FAST_INSTANT_TRADE_PAY" // 支付产品码（网页支付）
            };

            // 3. 构建支付请求对象
            var request = new AlipayTradePagePayRequest();
            request.SetBizModel(model); // 设置业务参数
            request.SetNotifyUrl(opt.NotifyUrl); // 设置异步通知回调地址
            request.SetReturnUrl(opt.ReturnUrl); // 设置同步跳转回调地址

            // 4. 调用SDK生成支付页面HTML表单（同步调用，包装为异步）
            var response = await Task.Run(() => client.PageExecute(request));
            return response.Body; // 这是 HTML 表单，会自动跳转
        }
    }


}
