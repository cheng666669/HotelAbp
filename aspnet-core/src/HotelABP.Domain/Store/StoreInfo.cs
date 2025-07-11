using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace HotelABP.Store
{
    public class StoreInfo:FullAuditedAggregateRoot<Guid>
    {
        [Required(ErrorMessage = "门店名称不能为空")]
        [MaxLength(50, ErrorMessage = "门店名称不能超过50个字符")]
        public string StoreName { get; set; } = string.Empty;
        [MaxLength(50, ErrorMessage = "分店名称不能超过50个字符")]
        [Required(ErrorMessage = "分店名称不能为空")]
        public string BranchName { get; set; } = string.Empty;
        [MaxLength(100, ErrorMessage = "商户地址不能超过100个字符")]
        [Required(ErrorMessage = "商户地址不能为空")]
        public string Address { get; set; } = string.Empty;
        /// <summary>
        /// 商户图片
        /// </summary>
        [MaxLength(200, ErrorMessage = "商户图片不能超过200个字符")]
        public string StoreImg { get; set; } = string.Empty;
        /// <summary>
        /// 客服电话
        /// </summary>
        [MaxLength(11, ErrorMessage = "客服电话不能超过11个字符")]
        [Required(ErrorMessage = "客服电话不能为空")]
        [RegularExpression(@"^((0\d{2,3}-\d{7,8})|(1[3456789]\d{9}))$", ErrorMessage = "请输入正确的手机号")]
        public string Mobile { get; set; } = string.Empty;
        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 门店介绍
        /// </summary>
        [MaxLength(200, ErrorMessage = "门店介绍不能超过200个字符")]
        [Required(ErrorMessage = "门店介绍不能为空")]
        public string Introduction { get; set; } = string.Empty;
        /// <summary>
        /// 入住需知
        /// </summary>
        [MaxLength(200, ErrorMessage = "入住需知不能超过200个字符")]
        [Required(ErrorMessage = "入住需知不能为空")]
        public string Note { get; set; } = string.Empty;
    }
}
