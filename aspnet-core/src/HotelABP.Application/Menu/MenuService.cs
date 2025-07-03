using HotelABP.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HotelABP.Menu
{
    public class MenuService : ApplicationService, IMenuService
    {
        private readonly IRepository<Permission> perRep;
        private readonly IRepository<RolePermission> roleperRep;

        public MenuService(IRepository<Permission> perRep, IRepository<RolePermission> roleperRep)
        {
            this.perRep = perRep;
            this.roleperRep = roleperRep;
        }
        /// <summary>
        /// 获取用户菜单树
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ApiResult<List<MenuDto>>> GetMenuList(Guid userId)
        {
            // 1. 获取用户所有角色Id
            var userRoles = await LazyServiceProvider.LazyGetRequiredService<IRepository<UserRole>>().GetListAsync(x => x.UserId == userId);
            var roleIds = userRoles.Select(x => x.RoleId).ToList();

            // 2. 获取这些角色拥有的所有权限Id
            var rolePermissions = await roleperRep.GetListAsync(x => roleIds.Contains(x.RoleId));
            var permissionIds = rolePermissions.Select(x => x.PermissionId).Distinct().ToList();

            // 3. 查询Permission表中IsMenu==true且在权限Id集合中的所有菜单
            var allMenus = await perRep.GetListAsync(x =>   permissionIds.Contains(x.Id) && x.IsVisible);

            // 4. 递归组装树结构
            var menuTree = BuildMenuTree(allMenus, Guid.Empty);
            return ApiResult<List<MenuDto>>.Success(menuTree,ResultCode.Success);
        }

        private List<MenuDto> BuildMenuTree(List<Permission> allMenus, Guid parentId)
        {
            return allMenus
                .Where(m => m.ParentId == parentId)
                .OrderBy(m => m.Order)
                .Select(m => new MenuDto
                {
                    Id = m.Id,
                    Name = m.PermissionName,
                    Path = m.Path,
                    Icon = m.Icon,
                    Order = m.Order,
                    IsMenu = m.IsMenu,
                    IsVisible = m.IsVisible,
                    Children = BuildMenuTree(allMenus, m.Id)
                }).ToList();
        }
    }
}
