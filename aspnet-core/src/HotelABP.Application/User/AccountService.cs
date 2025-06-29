using HotelABP.Account;
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

namespace HotelABP.User
{
    //[Authorize]
    [IgnoreAntiforgeryToken]
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
        public async Task<ApiResult> AddAccount([FromBody]AccountRoleDto dto)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var userexist = await userRep.FindAsync(x => x.NickName == dto.NickName);
                    if (userexist != null)
                    {
                        return ApiResult.Fail("用户已存在", ResultCode.ValidationError);
                    }
                    dto.Password = dto.Mobile.ToString().Substring(5,6);
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
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpDelete]
        public async Task<ApiResult> DelAccount(Guid Id)
        {
            try
            {
                var res = await userRep.FirstOrDefaultAsync(x=>x.Id == Id);
                if(res == null)
                {
                    return ApiResult.Fail("用户不存在", ResultCode.NotFound);
                }
                await userRep.DeleteAsync(res);
                return ApiResult.Success(ResultCode.Success);
            }
            catch (Exception ex)
            {
                logger.LogError("删除用户有误" + ex.Message);
                throw;
            }
        }
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResult> UpdateAccount(AccountRoleDto dto,Guid id)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var userexist = await userRep.FindAsync(x => x.NickName == dto.NickName);
                    
                    var rolelist = await userRoleRep.GetListAsync(x => x.UserId == id);
                    //删除用户角色中间表
                    foreach(var item in rolelist)
                    {
                        await userRoleRep.DeleteAsync(x=>x.RoleId==item.RoleId);
                    }
                    var user = ObjectMapper.Map(dto,userexist);
                    await userRep.UpdateAsync(user);
                    //添加用户角色
                    foreach (var item in dto.RoleIds)
                    {
                        var userRole = new UserRole
                        {
                            UserId = id,
                            RoleId = item
                        };
                        await userRoleRep.InsertAsync(userRole);
                    }
                    tran.Complete();
                    return ApiResult.Success(ResultCode.Success);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("修改用户有误" + ex.Message);
                throw;
            }
        }
        /// <summary>
        /// 显示用户列表
        /// </summary>
        /// <param name="seach"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResult<PageResult<GetAccountResultDTO>>> GetAccountList(Seach seach, SearchAccountDTO dto)
        {
            try
            {
                var res = await userRep.GetQueryableAsync();
                //res = res.WhereIf(!string.IsNullOrEmpty(dto.Mobile), x => x.Mobile == dto.Mobile)
                //    .WhereIf(!string.IsNullOrEmpty(dto.NickName), x => x.NickName.Contains(dto.NickName));
                var list = res.PageResult(seach.PageIndex, seach.PageSize);
                var dtoList = ObjectMapper.Map<List<SysUser>, List<GetAccountResultDTO>>(list.Queryable.ToList());
                foreach (var item in dtoList)
                {
                    var role = await userRoleRep.GetListAsync(x => x.UserId == item.Id);
                    foreach (var item1 in role)
                    {
                        var roleinfo = await roleRep.FindAsync(x => x.Id == item1.RoleId);
                        item.RoleName += roleinfo.RoleName+",";
                        item.RoleId = item1.RoleId;
                    }
                    // 去除最后一个逗号
                    if (!string.IsNullOrEmpty(item.RoleName))
                    {
                        item.RoleName = item.RoleName.TrimEnd(',');
                    }
                }
                dtoList = dtoList.WhereIf(!string.IsNullOrEmpty(dto.Mobile), x => x.Mobile == dto.Mobile)
                    .WhereIf(!string.IsNullOrEmpty(dto.NickName), x => x.NickName.Contains(dto.NickName))
                    .WhereIf(dto.RoleId != null, x => x.RoleId == dto.RoleId).ToList();
                var pageresult = new PageResult<GetAccountResultDTO>
                {
                    Data = dtoList,
                    TotleCount = list.RowCount,
                    TotlePage = (int)Math.Ceiling(list.RowCount * 1.0 / seach.PageSize)
                };
                return ApiResult<PageResult<GetAccountResultDTO>>.Success(pageresult, ResultCode.Success);
            }
            catch ( Exception ex)
            {

                throw;
            }
        }
        /// <summary>
        /// 显示用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResult<AccountRoleDto>> GetAccount(Guid id)
        {
            try
            {
                var user = await userRep.GetAsync(x=>x.Id==id);
                var dto = ObjectMapper.Map<SysUser, AccountRoleDto>(user);
                return ApiResult<AccountRoleDto>.Success(dto, ResultCode.Success);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
