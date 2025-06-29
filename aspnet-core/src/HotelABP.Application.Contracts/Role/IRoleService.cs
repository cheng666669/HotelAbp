using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HotelABP.Role
{
    public interface IRoleService:IApplicationService
    {
        Task<ApiResult> CreateRoleAsync(CreateUpdateRoleDto dto);
        Task<ApiResult<List<PermissionTreeDto>>> GetPermissionTree();
        Task<ApiResult> DelRoleAsync(Guid Id);
        Task<ApiResult> UpdateRoleAsync(Guid Id, CreateUpdateRoleDto dto);
        Task<ApiResult<PageResult<GetRoleResultDTO>>> GetRoleList(Seach seach, SearchRoleDTO dto);
        Task<ApiResult<List<PermissionTreeDto>>> GetPermByRole(Guid roleid);
    }
}
