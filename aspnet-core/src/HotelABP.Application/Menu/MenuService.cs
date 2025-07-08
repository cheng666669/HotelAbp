using HotelABP.Users;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HotelABP.Menu
{
    /// <summary>
    /// 菜单管理
    /// </summary>
    [ApiExplorerSettings(GroupName = "menu")]
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
        /// 获取用户菜单树及操作权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>包含菜单树和操作权限的ApiResult</returns>
        /// <remarks>
        /// 此方法主要用于根据用户ID，获取该用户所拥有的菜单（树状结构）及其对应的操作权限。
        /// 详细步骤如下：
        /// 1. 通过UserRole表获取该用户所拥有的所有角色ID。
        /// 2. 通过RolePermission表获取这些角色所拥有的所有权限ID。
        /// 3. 通过Permission表筛选出所有菜单类型的权限（IsMenu=true）和操作类型的权限（IsMenu=false），且这些权限必须在权限ID集合中，并且是可见的（IsVisible=true）。
        /// 4. 递归调用BuildMenuTree方法，将菜单和操作权限组装成树状结构。
        /// </remarks>
        public async Task<ApiResult<List<MenuDto>>> GetMenuList(Guid userId)
        {
            // 1. 获取用户所有角色Id
            // 通过UserRole表查找该用户的所有角色（一个用户可能有多个角色）
            var userRoles = await LazyServiceProvider.LazyGetRequiredService<IRepository<UserRole>>().GetListAsync(x => x.UserId == userId);
            var roleIds = userRoles.Select(x => x.RoleId).ToList(); // 提取所有角色ID

            // 2. 获取这些角色拥有的所有权限Id
            // 通过RolePermission表查找这些角色所拥有的所有权限
            var rolePermissions = await roleperRep.GetListAsync(x => roleIds.Contains(x.RoleId));
            var permissionIds = rolePermissions.Select(x => x.PermissionId).Distinct().ToList(); // 提取所有权限ID，并去重

            // 3. 查询所有菜单和操作权限
            // 查询所有菜单权限（IsMenu=true），且在权限ID集合中，并且是可见的
            var allMenus = await perRep.GetListAsync(x => x.IsMenu && permissionIds.Contains(x.Id) && x.IsVisible);
            // 查询所有操作权限（IsMenu=false），且在权限ID集合中，并且是可见的
            var allActions = await perRep.GetListAsync(x => !x.IsMenu && permissionIds.Contains(x.Id) && x.IsVisible);

            // 4. 递归组装树结构
            // 调用BuildMenuTree方法，将菜单和操作权限组装成树状结构，parentId初始为Guid.Empty（根节点）
            var menuTree = BuildMenuTree(allMenus, allActions, Guid.Empty);
            // 返回封装好的ApiResult对象，包含菜单树和操作权限
            return ApiResult<List<MenuDto>>.Success(menuTree, ResultCode.Success);
        }

        /// <summary>
        /// 递归构建菜单树结构，并为每个菜单节点分配对应的操作权限
        /// </summary>
        /// <param name="allMenus">所有菜单权限集合（IsMenu=true）</param>
        /// <param name="allActions">所有操作权限集合（IsMenu=false）</param>
        /// <param name="parentId">父级菜单ID（递归入口为Guid.Empty）</param>
        /// <returns>组装好的菜单树（每个菜单节点包含子菜单和操作权限）</returns>
        /// <remarks>
        /// 此方法采用递归方式：
        /// 1. 先筛选出当前parentId下的所有菜单节点。
        /// 2. 对每个菜单节点：
        ///   a. 递归调用自身，查找其子菜单（Children）。
        ///   b. 查找其下所有操作权限（Actions），即ParentId等于当前菜单ID的所有操作权限。
        /// 3. 最终返回组装好的菜单树结构。
        /// </remarks>
        private List<MenuDto> BuildMenuTree(List<Permission> allMenus, List<Permission> allActions, Guid parentId)
        {
            // 1. 筛选出当前parentId下的所有菜单节点，并按Order排序
            return allMenus
                .Where(m => m.ParentId == parentId)
                .OrderBy(m => m.Order)
                .Select(m => new MenuDto
                {
                    Id = m.Id, // 菜单ID
                    Name = m.PermissionName, // 菜单名称
                    Path = m.Path, // 菜单路由路径
                    Icon = m.Icon, // 菜单图标
                    Order = m.Order, // 菜单排序
                    IsMenu = m.IsMenu, // 是否为菜单
                    IsVisible = m.IsVisible, // 是否可见
                    // 递归查找子菜单
                    Children = BuildMenuTree(allMenus, allActions, m.Id),
                    // 查找当前菜单下的所有操作权限
                    Actions = allActions
                        .Where(a => a.ParentId == m.Id)
                        .Select(a => new ActionDto { Id = a.Id, Name = a.PermissionName })
                        .ToList()
                }).ToList();
        }
    }
}
