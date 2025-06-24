using HotelABP.Account;
using HotelABP.Users;
using Microsoft.AspNetCore.Authorization;
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

namespace HotelABP.User
{
    [Authorize]
    public class AccountService : ApplicationService, IAccountService
    {
        private readonly IRepository<SysUser> userRep;
        private readonly IRepository<Roles> roleRep;
        private readonly IRepository<UserRole> userRoleRep;
        private readonly ILogger<AccountService> logger;
        private readonly IDistributedCache<PageResult<RoleDto>> cache;

        public AccountService(IRepository<SysUser> userRep, IRepository<Roles> roleRep, IRepository<UserRole> userRoleRep,ILogger<AccountService> logger,IDistributedCache<PageResult<RoleDto>> cache)
        {
            this.userRep = userRep;
            this.roleRep = roleRep;
            this.userRoleRep = userRoleRep;
            this.logger = logger;
            this.cache = cache;
        }
        /// <summary>
        /// 添加用户角色
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ApiResult> AddAccount(AccountRoleDto dto)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var userexist = await userRep.FindAsync(x => x.UserName == dto.NickName);
                    if (userexist != null)
                    {
                        return ApiResult.Fail("用户已存在", ResultCode.ValidationError);
                    }
                    var data = ObjectMapper.Map<AccountRoleDto, SysUser>(dto);
                    var user = await userRep.InsertAsync(data);
                    var userid = user.Id;
                    foreach(var item in dto.RoleIds)
                    {
                        var userrole = new UserRole
                        {
                            UserId = userid,
                            RoleId = item
                        };
                        await userRoleRep.InsertAsync(userrole);
                    }
                    tran.Complete();
                    return ApiResult.Success(ResultCode.Success);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("添加用户角色有误"+ex.Message);
                throw;
            }
        }
        /// <summary>
        /// 显示角色列表
        /// </summary>
        /// <param name="seach"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResult<PageResult<RoleDto>>> GetRoleList(Seach seach)
        {
            try
            {
                //写键名
                var keys = "GetRole";
                //使用Redis缓存获取或添加数据
                var cacheResult = await cache.GetOrAddAsync(keys,async () =>
                {
                    var role = await roleRep.GetQueryableAsync();
                    var page = role.PageResult(seach.PageIndex, seach.PageSize);
                    var dto = ObjectMapper.Map<List<Roles>, List<RoleDto>>(page.Queryable.ToList());
                    PageResult<RoleDto> pageResult = new PageResult<RoleDto>
                    {
                        TotleCount = page.RowCount,
                        TotlePage = (int)Math.Ceiling(page.RowCount * 1.0 / seach.PageSize),
                        Data = dto
                    };
                    return pageResult;
                },()=>new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(5)//设置缓存过期时间为5分钟
                });

                
                return ApiResult<PageResult<RoleDto>>.Success(cacheResult, ResultCode.Success);
            }
            catch (Exception ex)
            {
                logger.LogError("显示角色列表有误" + ex.Message);
                throw;
            }
        }
    }
}
