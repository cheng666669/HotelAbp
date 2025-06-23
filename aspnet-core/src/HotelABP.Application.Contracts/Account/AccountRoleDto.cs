using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.Account
{
    public class AccountRoleDto
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; } = string.Empty;
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; } = string.Empty;
        /// <summary>
        /// 角色ID列表
        /// </summary>
        public List<Guid> RoleIds { get; set; } = new List<Guid>();
    }
}
