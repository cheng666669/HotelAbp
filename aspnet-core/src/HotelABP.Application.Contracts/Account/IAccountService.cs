using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HotelABP.Account
{
    public interface IAccountService:IApplicationService
    {
        Task<ApiResult> AddAccount(AccountRoleDto dto);
        Task<ApiResult<PageResult<RoleDto>>> GetRoleList(Seach seach);
        Task<ApiResult> DelAccount(Guid Id);
        Task<ApiResult> UpdateAccount(AccountRoleDto dto,Guid id);
        Task<ApiResult<PageResult<GetAccountResultDTO>>> GetAccountList(Seach seach,SearchAccountDTO dto);
    }
}
