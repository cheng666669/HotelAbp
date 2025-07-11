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
    /// <summary>
    /// 账号管理
    /// </summary>
    [IgnoreAntiforgeryToken]
    [ApiExplorerSettings(GroupName = "account")]
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
        /// 添加用户
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// TransactionScope本身支持自动回滚机制，
        /// 只要代码块内发生异常且没有调用 tran.Complete()，事务就会自动回滚，不需要手动写回滚代码。
        public async Task<ApiResult> AddAccountno(AccountDto dto)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        var userexist = await userRep.FindAsync(x => x.NickName == dto.NickName);
                        if (userexist != null)
                        {
                            return ApiResult.Fail("用户已存在", ResultCode.ValidationError);
                        }
                        dto.Password = dto.Mobile.ToString().Substring(7, 4);
                        var data = ObjectMapper.Map<AccountDto, SysUser>(dto);
                        var user = await userRep.InsertAsync(data);
                        tran.Complete();
                        return ApiResult.Success(ResultCode.Success);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("添加用户角色有误" + ex.Message);
                throw;
            }
        }
        /// <summary>
        /// 添加用户角色
        /// </summary>
        /// <param name="dto">用户及角色信息DTO</param>
        /// <returns>操作结果</returns>
        /// <remarks>
        /// 1. 开启事务，保证用户和角色关系同时写入。
        /// 2. 检查昵称是否已存在，存在则返回失败。
        /// 3. 自动生成初始密码（取手机号后6位）。
        /// 4. DTO映射为SysUser实体并插入用户表。
        /// 5. 遍历角色ID集合，插入用户-角色中间表。
        /// 6. 提交事务。
        /// 7. 捕获异常并记录日志。
        /// </remarks>
        public async Task<ApiResult> AddAccount([FromBody]AccountRoleDto dto)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // 检查昵称是否已存在
                    var userexist = await userRep.FindAsync(x => x.NickName == dto.NickName);
                    if (userexist != null)
                    {
                        return ApiResult.Fail("用户已存在", ResultCode.ValidationError);
                    }
                    // 自动生成初始密码（手机号后6位）
                    dto.Password = dto.Mobile.ToString().Substring(5,6);
                    // DTO映射为SysUser实体
                    var data = ObjectMapper.Map<AccountRoleDto, SysUser>(dto);
                    // 插入用户表
                    var user = await userRep.InsertAsync(data);
                    var userid = user.Id;
                    // 遍历角色ID集合，插入用户-角色中间表
                    foreach(var item in dto.RoleIds)
                    {
                        var userrole = new UserRole
                        {
                            UserId = userid,
                            RoleId = item
                        };
                        await userRoleRep.InsertAsync(userrole);
                    }
                    tran.Complete(); // 提交事务
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
        /// 显示角色列表（带缓存，提升性能）
        /// </summary>
        /// <param name="seach">分页参数</param>
        /// <returns>分页后的角色列表</returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <remarks>
        /// 1. 优先从Redis缓存获取角色分页数据。
        /// 2. 若缓存不存在则查询数据库并缓存结果。
        /// 3. 缓存有效期5分钟。
        /// </remarks>
        public async Task<ApiResult<PageResult<RoleDto>>> GetRoleList(Seach seach)
        {
            try
            {
                //写键名
                var keys = "GetRole";
                //使用Redis缓存获取或添加数据
                var cacheResult = await cache.GetOrAddAsync(keys,async () =>
                {
                    // 查询角色分页
                    var role = await roleRep.GetQueryableAsync();
                    var page = role.PageResult(seach.PageIndex, seach.PageSize);
                    // DTO映射
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
        /// 删除用户（根据ID）
        /// </summary>
        /// <param name="Id">用户ID</param>
        /// <returns>操作结果</returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <remarks>
        /// 1. 根据ID查找用户。
        /// 2. 若不存在返回失败。
        /// 3. 存在则删除。
        /// 4. 捕获异常并记录日志。
        /// </remarks>
        [HttpDelete]
        public async Task<ApiResult> DelAccount(Guid Id)
        {
            try
            {
                // 查找用户
                var res = await userRep.FirstOrDefaultAsync(x=>x.Id == Id);
                if(res == null)
                {
                    return ApiResult.Fail("用户不存在", ResultCode.NotFound);
                }
                // 删除用户
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
        /// 修改用户及分配角色（支持事务，防止部分写入）
        /// </summary>
        /// <param name="dto">用户及角色信息DTO</param>
        /// <param name="id">用户ID</param>
        /// <returns>操作结果</returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <remarks>
        /// 1. 开启事务。
        /// 2. 查找用户。
        /// 3. 删除原有用户-角色关系。
        /// 4. 更新用户信息。
        /// 5. 新增用户-角色关系。
        /// 6. 提交事务。
        /// 7. 捕获异常并记录日志。
        /// </remarks>
        public async Task<ApiResult> UpdateAccount(AccountRoleDto dto,Guid id)
        {
            try
            {
                using (var tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // 查找用户
                    var userexist = await userRep.FindAsync(x => x.NickName == dto.NickName);
                    // 查找原有用户-角色关系
                    var rolelist = await userRoleRep.GetListAsync(x => x.UserId == id);
                    // 删除用户角色中间表
                    foreach(var item in rolelist)
                    {
                        await userRoleRep.DeleteAsync(item);
                    }
                    // 自动生成初始密码（手机号后6位）
                    dto.Password = dto.Mobile.ToString().Substring(5, 6);
                    // DTO映射并更新用户信息
                    var user = ObjectMapper.Map(dto,userexist);
                    await userRep.UpdateAsync(user);
                    // 添加新用户-角色关系
                    foreach (var item in dto.RoleIds)
                    {
                        var userRole = new UserRole
                        {
                            UserId = id,
                            RoleId = item
                        };
                        await userRoleRep.InsertAsync(userRole);
                    }
                    tran.Complete(); // 提交事务
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
        /// 显示用户列表（支持多条件筛选和分页）
        /// </summary>
        /// <param name="seach">分页参数</param>
        /// <param name="dto">筛选条件</param>
        /// <returns>分页后的用户列表</returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <remarks>
        /// 1. 查询所有用户并分页。
        /// 2. DTO映射。
        /// 3. 查询每个用户的角色信息并拼接。
        /// 4. 支持手机号、昵称、角色ID多条件筛选。
        /// 5. 返回分页结果。
        /// </remarks>
        public async Task<ApiResult<PageResult<GetAccountResultDTO>>> GetAccountList(Seach seach, SearchAccountDTO dto)
        {
            try
            {
                // 查询所有用户并分页
                var res = await userRep.GetQueryableAsync();
                var list = res.PageResult(seach.PageIndex, seach.PageSize);
                // DTO映射
                var dtoList = ObjectMapper.Map<List<SysUser>, List<GetAccountResultDTO>>(list.Queryable.ToList());
                // 查询每个用户的角色信息
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
                // 多条件筛选
                dtoList = dtoList.WhereIf(!string.IsNullOrEmpty(dto.Mobile), x => x.Mobile == dto.Mobile)
                    .WhereIf(!string.IsNullOrEmpty(dto.NickName), x => x.NickName.Contains(dto.NickName))
                    .WhereIf(dto.RoleId != null, x => x.RoleId == dto.RoleId).ToList();
                // 组装分页结果
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
        /// 显示用户详情（根据ID）
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>用户详情DTO</returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <remarks>
        /// 1. 根据ID查找用户。
        /// 2. DTO映射。
        /// 3. 返回ApiResult包装的用户详情。
        /// </remarks>
        public async Task<ApiResult<AccountRoleDto>> GetAccount(Guid id)
        {
            try
            {
                // 查找用户
                var user = await userRep.GetAsync(x=>x.Id==id);
                // DTO映射
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
