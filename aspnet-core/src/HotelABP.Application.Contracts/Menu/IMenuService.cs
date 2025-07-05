using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HotelABP.Menu
{
    public interface IMenuService:IApplicationService
    {
        Task<ApiResult<List<MenuDto>>> GetMenuList(Guid userId);
    }
}
