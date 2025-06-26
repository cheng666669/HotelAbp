using AutoMapper.Internal.Mappers;
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

        public RoleService(IRepository<Roles> roleRep,IRepository<Permission> perRep,IDistributedCache<List<PermissionTreeDto>> cache,IRepository<RolePermission> roleperRep)
        {
            this.roleRep = roleRep;
            this.perRep = perRep;
            this.cache = cache;
            this.roleperRep = roleperRep;
        }
        /// <summary>
        /// 添加角色权限
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResult> CreateRoleAsync(CreateUpdateRoleDto dto)
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

                    //var rolelist = ObjectMapper.Map<CreateUpdateRoleDto, Roles>(dto);
                    //rolelist.Id = Id;
                    //await roleRep.UpdateAsync(rolelist);

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
            catch (Exception)
            {

                throw;
            }
        }
    }
}
