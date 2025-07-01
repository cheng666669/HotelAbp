using System;
using Volo.Abp.Application.Dtos;

namespace HotelABP.User
{
    public class LoginResultDto: EntityDto<Guid>
    {
        public string AccessToken { get; set; }
        public object Expires { get; set; }
        public object RefreshToken { get; set; }
        public string TokenType { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; } = string.Empty;
    }
}
