using Alipay.AopSdk.Core;
using Alipay.AopSdk.Core.Domain;
using Alipay.AopSdk.Core.Request;
using HotelABP.RoomReserves;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace HotelABP.ReserveRooms
{
    public interface IAlipayService
    {
        Task<string> CreatePaymentUrlAsync(string orderId, string subject, decimal amount);
    }

    public class AlipayService : IAlipayService, ITransientDependency
    {
        private readonly IOptions<AlipayOptions> _options;

        public AlipayService(IOptions<AlipayOptions> options)
        {
            _options = options;
        }

        public async Task<string> CreatePaymentUrlAsync(string orderId, string subject, decimal amount)
        {
            var opt = _options.Value;

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

            var model = new AlipayTradePagePayModel
            {
                OutTradeNo = orderId.ToString(),
                TotalAmount = amount.ToString("F2"),
                Subject = subject,
                ProductCode = "FAST_INSTANT_TRADE_PAY"
            };

            var request = new AlipayTradePagePayRequest();
            request.SetBizModel(model);
            request.SetNotifyUrl(opt.NotifyUrl);
            request.SetReturnUrl(opt.ReturnUrl);

            var response = await Task.Run(() => client.PageExecute(request));
            return response.Body; // 这是 HTML 表单，会自动跳转
        }
    }


}
