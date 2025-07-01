using HotelABP.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Transactions;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;

namespace HotelABP.Role
{
    [IgnoreAntiforgeryToken]
    public class RoleService : ApplicationService, IRoleService
    {
        private readonly IRepository<Roles> roleRep;
        private readonly IRepository<Permission> perRep;
        private readonly IDistributedCache<List<PermissionTreeDto>> cache;
        private readonly IRepository<RolePermission> roleperRep;
        private readonly ILogger<RoleService> logger;

        public RoleService(IRepository<Roles> roleRep, IRepository<Permission> perRep, IDistributedCache<List<PermissionTreeDto>> cache, IRepository<RolePermission> roleperRep, ILogger<RoleService> logger)
        {
            this.roleRep = roleRep;
            this.perRep = perRep;
            this.cache = cache;
            this.roleperRep = roleperRep;
            this.logger = logger;
        }
        /// <summary>
        /// 添加角色权限
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResult> CreateRoleAsync([FromBody]CreateUpdateRoleDto dto)
        {
            try
            {
                var roleExist = await roleRep.FindAsync(x => x.RoleName == dto.RoleName);
                if (roleExist != null)
                {
                    return ApiResult.Fail("角色已存在",ResultCode.Error);
                }
                using (var tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var roledto = ObjectMapper.Map<CreateUpdateRoleDto, Roles>(dto);
                    var role = await roleRep.InsertAsync(roledto);
                    var roleid = role.Id;
                    foreach (var item in dto.PermissionIds)
                    {
                        var roleper = new RolePermission();
                        roleper.RoleId = roleid;
                        roleper.PermissionId = item;
                        await roleperRep.InsertAsync(roleper);
                    }
                    tran.Complete();
                    return ApiResult.Success(ResultCode.Success);
                }
            }
            catch (Exception ex) 
            {
                logger.LogError("添加角色权限有误" + ex.Message);
                throw;
            }
        }
        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResult<List<PermissionTreeDto>>> GetPermissionTree()
        {
            try
            {
                //设置键名
                var keys = "GetPermissionTree";

                var cacheResult = await cache.GetOrAddAsync(keys,async () =>
                {
                    var list = await GetPermission(Guid.Empty);
                    return list;
                }, () => new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(10)//设置缓存过期时间为5分钟
                });
                return ApiResult<List<PermissionTreeDto>>.Success(cacheResult,ResultCode.Success);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        private async Task<List<PermissionTreeDto>> GetPermission(Guid parentid)
        {
            try
            {
                var permissions = await perRep.GetListAsync(x => x.ParentId == parentid);
                var permissionDtos = new List<PermissionTreeDto>();
                foreach (var item in permissions)
                {
                    var permissionDto = new PermissionTreeDto();
                    permissionDto.Id = item.Id;
                    permissionDto.PermissionName = item.PermissionName;
                    permissionDto.Children = await GetPermission(item.Id);
                    permissionDtos.Add(permissionDto);
                }
                return permissionDtos;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        /// <summary>
        /// 初始化权限数据
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult> InitData()
        {
            try
            {
                // 1. 一级权限
                var permissions = new List<Permission>();

                var overview = new Permission { PermissionName = "概况" };
                var roomStatus = new Permission { PermissionName = "房态" };
                var futureRoomStatus = new Permission { PermissionName = "远期房态" };
                var order = new Permission { PermissionName = "订单" };
                var business = new Permission { PermissionName = "业务" };
                var customer = new Permission { PermissionName = "客户" };
                var marketing = new Permission { PermissionName = "营销" };
                var handleBooking = new Permission { PermissionName = "办理预订" };

                // 2. 插入一级权限，获取Id
                await perRep.InsertAsync(overview);
                await perRep.InsertAsync(roomStatus);
                await perRep.InsertAsync(futureRoomStatus);
                await perRep.InsertAsync(order);
                await perRep.InsertAsync(business);
                await perRep.InsertAsync(customer);
                await perRep.InsertAsync(marketing);
                await perRep.InsertAsync(handleBooking);

                // 3. 二级及以下权限
                // 房态操作权限
                var roomStatusOps = new[]
                {
                    "设为维修","设为脏房","设为胜房","设为保留","办理预订","办理入住","取消保留","操作入住",
                    "取消预订","办理退房","办理换房","办理结算","批量设置保留","快速预订","住宿记录","切换房态"
                };
                foreach (var name in roomStatusOps)
                {
                    await perRep.InsertAsync(new Permission
                    {
                        PermissionName = name,
                        ParentId = roomStatus.Id
                    });
                }

                // 远期房态操作权限
                var futureRoomStatusOps = new[]
                {
                    "换房","退房","入账","结算","办理预订","办理入住","取消预订","设为维修","设为脏房","设为胜房",
                    "设为保留","批量设置保留","快速预订","住宿记录","切换房态"
                };
                foreach (var name in futureRoomStatusOps)
                {
                    await perRep.InsertAsync(new Permission
                    {
                        PermissionName = name,
                        ParentId = futureRoomStatus.Id
                    });
                }

                // 订单子权限
                var orderChildren = new[]
                {
                    "住宿订单","商城订单","餐饮订单","售后管理","会员开卡订单","充值订单","收款码订单","扫码核销","其他订单"
                };
                foreach (var name in orderChildren)
                {
                    await perRep.InsertAsync(new Permission
                    {
                        PermissionName = name,
                        ParentId = order.Id
                    });
                }

                // 业务子权限
                var businessChildren = new[]
                {
                    "房价房态","房型管理","商品管理","餐饮管理","收款码管理"
                };
                foreach (var name in businessChildren)
                {
                    await perRep.InsertAsync(new Permission
                    {
                        PermissionName = name,
                        ParentId = business.Id
                    });
                }

                // 客户子权限
                var customerChildren = new[]
                {
                    "客户管理","会员卡","资产管理","资产记录"
                };
                foreach (var name in customerChildren)
                {
                    await perRep.InsertAsync(new Permission
                    {
                        PermissionName = name,
                        ParentId = customer.Id
                    });
                }

                // 营销子权限
                await perRep.InsertAsync(new Permission
                {
                    PermissionName = "营销应用",
                    ParentId = marketing.Id
                });

                return ApiResult.Success(ResultCode.Success);
            }
            catch (Exception e)
            {
                Logger.LogError("初始化权限数据有误" + e.Message);
                throw;
            }
        }
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpDelete]
        public async Task<ApiResult> DelRoleAsync(Guid Id)
        {
            try
            {
                var role = await roleRep.GetAsync(x => x.Id == Id);
                if (role == null)
                {
                    return ApiResult.Fail("角色不存在", ResultCode.NotFound);
                }
                await roleRep.DeleteAsync(role);
                return ApiResult.Success(ResultCode.Success);
            }
            catch ( Exception ex)
            {

                throw;
            }
        }
        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPut]
        public async Task<ApiResult> UpdateRoleAsync(Guid Id, CreateUpdateRoleDto dto)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var role = await roleRep.GetAsync(x => x.Id == Id);
                    if (role == null)
                    {
                        return ApiResult.Fail("角色不存在", ResultCode.NotFound);
                    }
             
                    //删除中间表
                    var roleperlist = await roleperRep.GetListAsync(x => x.RoleId == Id);
                    foreach (var item in roleperlist)
                    {
                        await roleperRep.DeleteAsync(item);
                    }
                    var roltlist = ObjectMapper.Map(dto,role);
                    var a = await roleRep.UpdateAsync(roltlist);
                    //添加中间表
                    foreach (var item in dto.PermissionIds)
                    {
                        var roleper = new RolePermission
                        {
                            RoleId = Id,
                            PermissionId = item
                        };
                        await roleperRep.InsertAsync(roleper);
                    }
                    //提交事务
                    tran.Complete();
                    return ApiResult.Success(ResultCode.Success);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("修改角色有误" + ex.Message);
                throw;
            }
        }
        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="seach"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResult<PageResult<GetRoleResultDTO>>> GetRoleList(Seach seach, SearchRoleDTO dto)
        {
            try
            {
                var role = await roleRep.GetQueryableAsync();
                role = (System.Linq.IQueryable<Roles>)role.WhereIf(!string.IsNullOrEmpty(dto.RoleName), x => x.RoleName.Contains(dto.RoleName));
                var list = role.PageResult(seach.PageIndex, seach.PageSize);
                var dtoList = ObjectMapper.Map<List<Roles>, List<GetRoleResultDTO>>(list.Queryable.ToList());
                var pageresult = new PageResult<GetRoleResultDTO>
                {
                    TotleCount = list.RowCount,
                    TotlePage = (int)Math.Ceiling(list.RowCount * 1.0 / seach.PageSize),
                    Data = dtoList
                };
                return ApiResult<PageResult<GetRoleResultDTO>>.Success(pageresult, ResultCode.Success);
            }
            catch (Exception ex)
            {
                logger.LogError("获取角色列表有误"+ex.Message);
                throw;
            }
        }
        /// <summary>
        /// 根据角色id获取权限树
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResult<List<PermissionTreeDto>>> GetPermByRole(Guid roleid)
        {
            try
            {
                // 1. 获取角色拥有的权限ID列表
                var rolePermissions = await roleperRep.GetListAsync(x => x.RoleId == roleid); // 从角色权限表中查询指定角色的所有权限记录
                var rolePermissionIds = rolePermissions.Select(x => x.PermissionId).ToList(); // 提取出权限ID列表

                // 2. 如果没有权限，返回空列表
                if (!rolePermissionIds.Any()) // 检查权限ID列表是否为空
                {
                    return ApiResult<List<PermissionTreeDto>>.Success(new List<PermissionTreeDto>(), ResultCode.Success); // 如果为空，返回空的成功结果
                }

                // 3. 获取角色拥有的权限详情
                var permissions = await perRep.GetListAsync(x => rolePermissionIds.Contains(x.Id)); // 根据权限ID列表获取完整的权限信息
                
                // 4. 构建权限树
                var result = new List<PermissionTreeDto>(); // 创建结果列表，用于存储最终的权限树
                
                // 5. 先找到所有一级权限（ParentId为空的）
                var rootPermissions = permissions.Where(x => x.ParentId == Guid.Empty).ToList(); // 筛选出所有没有父级的权限（一级权限）
                
                // 6. 为每个一级权限构建树
                foreach (var rootPermission in rootPermissions) // 遍历每个一级权限
                {
                    var treeNode = BuildTreeNode(rootPermission, permissions); // 为当前一级权限构建完整的树结构
                    result.Add(treeNode); // 将构建好的树节点添加到结果列表中
                }
                
                return ApiResult<List<PermissionTreeDto>>.Success(result, ResultCode.Success); // 返回成功结果
            }
            catch (Exception ex) // 捕获异常
            {
                Logger.LogError($"根据角色ID获取权限树失败: {ex.Message}"); // 记录错误日志
                throw; // 重新抛出异常
            }
        }

        /// <summary>
        /// 构建权限树节点
        /// </summary>
        /// <param name="permission">当前权限</param>
        /// <param name="allPermissions">所有权限列表</param>
        /// <returns></returns>
        private PermissionTreeDto BuildTreeNode(Permission permission, List<Permission> allPermissions)
        {
            try
            {
                // 创建权限树节点对象
                var node = new PermissionTreeDto
                {
                    Id = permission.Id, // 设置权限ID
                    PermissionName = permission.PermissionName, // 设置权限名称
                    IsSelected = true, // 设置选中状态为true（因为所有显示的权限都是角色拥有的）
                    Children = new List<PermissionTreeDto>() // 初始化子权限列表
                };

                // 找到当前权限的所有子权限
                var children = allPermissions.Where(x => x.ParentId == permission.Id).ToList(); // 筛选出所有父级ID等于当前权限ID的权限

                // 为每个子权限递归构建树
                foreach (var child in children) // 遍历每个子权限
                {
                    var childNode = BuildTreeNode(child, allPermissions); // 递归调用，为子权限构建树节点
                    node.Children.Add(childNode); // 将子节点添加到当前节点的子权限列表中
                }

                return node; // 返回构建好的树节点
            }
            catch (Exception ex)
            {
                logger.LogError("构建权限树节点有误" + ex.Message);
                throw;
            }
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResult> DeleteRange(List<Guid> guids)
        {
            try
            {
                foreach (var item in guids)
                {
                    var res = await roleRep.FindAsync(x=>x.Id==item);
                    await roleRep.DeleteAsync(res);
                }
                return ApiResult.Success(ResultCode.Success);
            }
            catch (Exception ex)
            {
                logger.LogError("批量删除角色有误" + ex.Message);
                throw;
            }
        }
    }
}
