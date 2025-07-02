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

                // 房态
                var roomStatus = new Permission { PermissionName = "房态", Path = "/room-status", Icon = "icon-room", Order = 1, IsMenu = true, IsVisible = true };
                await perRep.InsertAsync(roomStatus);
                var roomStatusOps = new[]
                {
                    "设为维修","设为脏房","设为胜房","设为保留","办理预订","办理入住","取消保留","操作入住",
                    "取消预订","办理退房","办理换房","办理结算","批量设置保留","快速预订","住宿记录","切换房态"
                };
                int roomStatusOrder = 1;
                foreach (var name in roomStatusOps)
                {
                    await perRep.InsertAsync(new Permission { PermissionName = name, ParentId = roomStatus.Id, Path = string.Empty, Icon = string.Empty, Order = roomStatusOrder++, IsMenu = false, IsVisible = true });
                }

                // 订单
                var order = new Permission { PermissionName = "订单", Path = "/order", Icon = "icon-order", Order = 2, IsMenu = true, IsVisible = true };
                await perRep.InsertAsync(order);

                // 订单下的子权限及其操作权限
                // 住宿订单
                var stayOrder = new Permission { PermissionName = "住宿订单", ParentId = order.Id, Path = "/order/stay", Icon = "icon-stay", Order = 1, IsMenu = false, IsVisible = true };
                await perRep.InsertAsync(stayOrder);
                var stayOrderOps = new[] { "导出明细", "取消房间", "取消订单", "退房", "绿标", "取消订单", "办理入住" };
                int stayOrderOrder = 1;
                foreach (var name in stayOrderOps)
                {
                    await perRep.InsertAsync(new Permission { PermissionName = name, ParentId = stayOrder.Id, Path = string.Empty, Icon = string.Empty, Order = stayOrderOrder++, IsMenu = false, IsVisible = true });
                }

                // 订房订单
                var bookOrder = new Permission { PermissionName = "订房订单", ParentId = order.Id, Path = "/order/book", Icon = "icon-book", Order = 2, IsMenu = false, IsVisible = true };
                await perRep.InsertAsync(bookOrder);
                var bookOrderOps = new[] { "导出记录", "确认预订", "拒绝" };
                int bookOrderOrder = 1;
                foreach (var name in bookOrderOps)
                {
                    await perRep.InsertAsync(new Permission { PermissionName = name, ParentId = bookOrder.Id, Path = string.Empty, Icon = string.Empty, Order = bookOrderOrder++, IsMenu = false, IsVisible = true });
                }

                // 商城订单
                var mallOrder = new Permission { PermissionName = "商城订单", ParentId = order.Id, Path = "/order/mall", Icon = "icon-mall", Order = 3, IsMenu = false, IsVisible = true };
                await perRep.InsertAsync(mallOrder);
                var mallOrderOps = new[] { "通用", "结算" };
                int mallOrderOrder = 1;
                foreach (var name in mallOrderOps)
                {
                    await perRep.InsertAsync(new Permission { PermissionName = name, ParentId = mallOrder.Id, Path = string.Empty, Icon = string.Empty, Order = mallOrderOrder++, IsMenu = false, IsVisible = true });
                }

                // 售后管理
                var afterSale = new Permission { PermissionName = "售后管理", ParentId = order.Id, Path = "/order/aftersale", Icon = "icon-aftersale", Order = 4, IsMenu = false, IsVisible = true };
                await perRep.InsertAsync(afterSale);
                var afterSaleOps = new[] { "退货", "退款" };
                int afterSaleOrder = 1;
                foreach (var name in afterSaleOps)
                {
                    await perRep.InsertAsync(new Permission { PermissionName = name, ParentId = afterSale.Id, Path = string.Empty, Icon = string.Empty, Order = afterSaleOrder++, IsMenu = false, IsVisible = true });
                }

                // 业务
                var business = new Permission { PermissionName = "业务", Path = "/business", Icon = "icon-business", Order = 3, IsMenu = true, IsVisible = true };
                await perRep.InsertAsync(business);

                // 房价房态
                var priceStatus = new Permission { PermissionName = "房价房态", ParentId = business.Id, Path = "/business/price-status", Icon = "icon-price", Order = 1, IsMenu = false, IsVisible = true };
                await perRep.InsertAsync(priceStatus);
                var priceStatusOps = new[] { "排房", "礼包设置", "投放", "删除", "价格自有", "启用", "停用" };
                int priceStatusOrder = 1;
                foreach (var name in priceStatusOps)
                {
                    await perRep.InsertAsync(new Permission { PermissionName = name, ParentId = priceStatus.Id, Path = string.Empty, Icon = string.Empty, Order = priceStatusOrder++, IsMenu = false, IsVisible = true });
                }

                // 房型管理
                var roomTypeManage = new Permission { PermissionName = "房型管理", ParentId = business.Id, Path = "/business/room-type", Icon = "icon-roomtype", Order = 2, IsMenu = false, IsVisible = true };
                await perRep.InsertAsync(roomTypeManage);
                var roomTypeManageOps = new[] { "新增", "编辑", "删除", "批量删除", "设置房价", "编辑房型", "调整房型房量" };
                int roomTypeManageOrder = 1;
                foreach (var name in roomTypeManageOps)
                {
                    await perRep.InsertAsync(new Permission { PermissionName = name, ParentId = roomTypeManage.Id, Path = string.Empty, Icon = string.Empty, Order = roomTypeManageOrder++, IsMenu = false, IsVisible = true });
                }

                // 商品管理
                var productManage = new Permission { PermissionName = "商品管理", ParentId = business.Id, Path = "/business/product", Icon = "icon-product", Order = 3, IsMenu = false, IsVisible = true };
                await perRep.InsertAsync(productManage);
                var productManageOps = new[] { "新增", "编辑", "删除", "批量删除", "设置价格", "编辑商品", "调整商品数量" };
                int productManageOrder = 1;
                foreach (var name in productManageOps)
                {
                    await perRep.InsertAsync(new Permission { PermissionName = name, ParentId = productManage.Id, Path = string.Empty, Icon = string.Empty, Order = productManageOrder++, IsMenu = false, IsVisible = true });
                }

                // 餐饮管理
                var diningManage = new Permission { PermissionName = "餐饮管理", ParentId = business.Id, Path = "/business/dining", Icon = "icon-dining", Order = 4, IsMenu = false, IsVisible = true };
                await perRep.InsertAsync(diningManage);
                var diningManageOps = new[] { "新增", "编辑", "删除", "批量删除", "设置价格", "编辑菜品", "调整菜品数量" };
                int diningManageOrder = 1;
                foreach (var name in diningManageOps)
                {
                    await perRep.InsertAsync(new Permission { PermissionName = name, ParentId = diningManage.Id, Path = string.Empty, Icon = string.Empty, Order = diningManageOrder++, IsMenu = false, IsVisible = true });
                }

                // 收款码管理
                var paymentCodeManage = new Permission { PermissionName = "收款码管理", ParentId = business.Id, Path = "/business/payment-code", Icon = "icon-payment", Order = 5, IsMenu = false, IsVisible = true };
                await perRep.InsertAsync(paymentCodeManage);
                var paymentCodeManageOps = new[] { "新增", "编辑", "删除", "批量删除", "设置收款码", "编辑收款码", "调整收款码数量" };
                int paymentCodeManageOrder = 1;
                foreach (var name in paymentCodeManageOps)
                {
                    await perRep.InsertAsync(new Permission { PermissionName = name, ParentId = paymentCodeManage.Id, Path = string.Empty, Icon = string.Empty, Order = paymentCodeManageOrder++, IsMenu = false, IsVisible = true });
                }

                // 客户
                var customer = new Permission { PermissionName = "客户", Path = "/customer", Icon = "icon-customer", Order = 4, IsMenu = true, IsVisible = true };
                await perRep.InsertAsync(customer);

                // 客户管理
                var customerManage = new Permission { PermissionName = "客户管理", ParentId = customer.Id, Path = "/customer/manage", Icon = "icon-customer-manage", Order = 1, IsMenu = false, IsVisible = true };
                await perRep.InsertAsync(customerManage);
                var customerManageOps = new[] { "清除", "充值", "重置有效期", "送积分", "同步粉丝", "标签管理", "发送优惠券", "导入会员", "编辑", "冻结", "传给微信", "打标签" };
                int customerManageOrder = 1;
                foreach (var name in customerManageOps)
                {
                    await perRep.InsertAsync(new Permission { PermissionName = name, ParentId = customerManage.Id, Path = string.Empty, Icon = string.Empty, Order = customerManageOrder++, IsMenu = false, IsVisible = true });
                }

                // 会员卡
                var memberCard = new Permission { PermissionName = "会员卡", ParentId = customer.Id, Path = "/customer/member-card", Icon = "icon-member-card", Order = 2, IsMenu = false, IsVisible = true };
                await perRep.InsertAsync(memberCard);
                var memberCardOps = new[] { "启用", "停用", "编辑会员卡", "选择移交", "转出", "保存", "删除等级", "添加自定义权益", "删除" };
                int memberCardOrder = 1;
                foreach (var name in memberCardOps)
                {
                    await perRep.InsertAsync(new Permission { PermissionName = name, ParentId = memberCard.Id, Path = string.Empty, Icon = string.Empty, Order = memberCardOrder++, IsMenu = false, IsVisible = true });
                }

                // 设置
                var setting = new Permission { PermissionName = "设置", Path = "/setting", Icon = "icon-setting", Order = 5, IsMenu = true, IsVisible = true };
                await perRep.InsertAsync(setting);
                var settingChildren = new[]
                {
                    "基本设置","员工管理","权限管理","通知设置","店铺认证","业务入口","客户关系"
                };
                int settingOrder = 1;
                foreach (var name in settingChildren)
                {
                    await perRep.InsertAsync(new Permission { PermissionName = name, ParentId = setting.Id, Path = $"/setting/{settingOrder}", Icon = $"icon-setting-{settingOrder}", Order = settingOrder++, IsMenu = false, IsVisible = true });
                }

                // 权限管理下的子权限
                var permissionManage = await perRep.InsertAsync(new Permission { PermissionName = "权限管理", ParentId = setting.Id, Path = "/setting/permission-manage", Icon = "icon-permission-manage", Order = 3, IsMenu = false, IsVisible = true });
                var permissionManageChildren = new[]
                {
                    "账户管理","账户编辑","角色管理","角色编辑","操作日志"
                };
                int permissionManageOrder = 1;
                foreach (var name in permissionManageChildren)
                {
                    await perRep.InsertAsync(new Permission { PermissionName = name, ParentId = permissionManage.Id, Path = string.Empty, Icon = string.Empty, Order = permissionManageOrder++, IsMenu = false, IsVisible = true });
                }

                return ApiResult.Success(ResultCode.Success);
            }
            catch (Exception e)
            {
                logger.LogError("初始化权限数据有误" + e.Message);
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
