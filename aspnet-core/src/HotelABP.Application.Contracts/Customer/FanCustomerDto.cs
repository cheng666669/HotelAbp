using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace HotelABP.Customer
{
    public class FanCustomerDto : FullAuditedEntityDto<Guid>
    {
        /// <summary>
        ///   客户昵称
        /// </summary>
        public string? CustomerNickName { get; set; } = string.Empty;
        /// <summary>
        /// 客户类型（0 = 会员，1 = 普通客户）
        /// </summary>
     //   [Required]
        public Guid? CustomerType { get; set; }

        /// <summary>
        /// 客户姓名（必填，最多16个字符）
        /// </summary>
      //  [StringLength(16, ErrorMessage = "客户姓名不能超过16个字符")]
        public string? CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// 手机号（必填，格式验证）
        /// </summary>
        //  [Phone(ErrorMessage = "请输入有效的手机号")]
        public string? PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// 性别（0 = 未知，1 = 男，2 = 女；可为空）
        /// </summary>
        public int? Gender { get; set; } = 0;

        /// <summary>
        /// 出生日期（可选）
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; } = DateTime.Now;

        /// <summary>
        /// 所在城市（可选，最多50个字符）
        /// </summary>
    //    [StringLength(50, ErrorMessage = "城市名称最多50个字符")]
        public string? City { get; set; } = string.Empty;

        /// <summary>
        /// 详细地址（可选，最多200个字符）
        /// </summary>
     //   [StringLength(200, ErrorMessage = "详细地址最多200个字符")]
        public string? Address { get; set; } = string.Empty;

        //  下面字段不能为 0，设定最小值为 0.01（或 1）

        /// <summary>
        /// 成长值（必须大于0，最多10亿，仅会员有效）
        /// </summary>
        // [Range(1, 1000000000, ErrorMessage = "成长值必须大于0且不超过10亿")]
        public decimal? GrowthValue { get; set; }

        /// <summary>
        /// 可用余额（必须大于0，最多95万，仅会员有效）
        /// </summary>
        //  [Range(0.01, 950000, ErrorMessage = "可用充值余额必须大于0且不超过95万")]
        public decimal? AvailableBalance { get; set; }

        /// <summary>
        /// 可用赠送余额余额（必须大于0，最多95万，仅会员有效）
        /// </summary>
        //  [Range(0.01, 950000, ErrorMessage = "可用赠送余额必须大于0且不超过95万")]
        public decimal? AvailableGiftBalance { get; set; }

        /// <summary>
        /// 可用积分（必须大于0，最多10亿，仅会员有效）
        /// </summary>
      //  [Range(1, 1000000000, ErrorMessage = "积分必须大于0且不超过10亿")]
        public decimal? AvailablePoints { get; set; }
        /// <summary>
        /// 客户类型名称
        /// </summary>
        public string? CustomerTypeName { get; set; }


        /// <summary>
        /// 充值金额（默认为0元）
        /// </summary>
        public decimal? Rechargeamount { get; set; }  // 充值金额，默认为0元
        /// <summary>
        ///  消费金额（默认为0元）
        /// </summary>
        public decimal? Sumofconsumption { get; set; }  // 累计消费金额，默认为0元
        /// <summary>
        /// 描述
        /// </summary>
        public string? CustomerDesc { get; set; } = string.Empty;
        /// <summary>
        /// 消费次数
        /// </summary>
        public int? ComsumerNumber { get; set; }  // 消费次数

        /// <summary>
        /// 状态
        /// </summary>
        public bool? Status { get; set; } = true;
        /// <summary>
        /// 消费描述
        /// </summary>
        public string? ConsumerDesc { get; set; } = string.Empty;
        /// <summary>
        ///  累计消费金额
        /// </summary>
        public decimal? Accumulativeconsumption { get; set; }
        /// <summary>
        /// 累计积分
        /// </summary>
        public decimal? Accumulativeintegral { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public string? CustomerLabel { get; set; } = string.Empty;
    }
}
