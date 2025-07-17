using HotelABP.Users;
using Lazy.Captcha.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;


namespace HotelABP.User
{
    /// <summary>
    /// 用户管理
    /// </summary>
    [ApiExplorerSettings(GroupName = "user")]
    //[Authorize]
    [IgnoreAntiforgeryToken]
    public class UserService:ApplicationService
    {
        private readonly IRepository<SysUser> userRep;
        private readonly IConfiguration configuration;
        private readonly ICaptcha captcha;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserService(IRepository<SysUser> userRep,IConfiguration configuration,ICaptcha captcha, IHttpContextAccessor httpContextAccessor)
        {
            this.userRep = userRep;
            this.configuration = configuration;
            this.captcha = captcha;
            this.httpContextAccessor = httpContextAccessor;
        }
        /// <summary>
        /// 生成验证码图片并返回给前端，前端可通过id标识本次验证码会话
        /// </summary>
        /// <param name="id">验证码会话唯一标识</param>
        /// <returns>验证码图片（GIF格式）</returns>
        /// <remarks>
        /// 1. 调用ICaptcha服务生成验证码图片和字节流。
        /// 2. 返回FileContentResult，前端可直接渲染为图片。
        /// </remarks>
        [HttpGet]
        public IActionResult Captcha(string id)
        {

                var info = captcha.Generate(id); // 生成验证码图片和字节流
                return new FileContentResult(info.Bytes, "image/gif"); // 返回图片内容
            

        }
        /// <summary>
        /// 演示时使用HttpGet传参方便，这里仅做返回处理
        /// </summary>
        /// <param name="id">验证码会话唯一标识</param>
        /// <param name="code">用户输入的验证码</param>
        /// <returns>校验结果，true为正确，false为错误</returns>
        /// <remarks>
        /// 1. 调用ICaptcha服务校验验证码。
        /// 2. 返回校验结果。
        /// </remarks>
        [HttpGet("validate")]
        public bool Validate(string id, string code)
        {
            return captcha.Validate(id, code); // 校验验证码
        }
        /// <summary>
        /// 用户登录，校验验证码、用户名和密码，成功后返回JWT Token
        /// </summary>
        /// <param name="dto">登录参数（用户名、密码、验证码等）</param>
        /// <returns>登录结果，包含Token等信息</returns>
        /// <remarks>
        /// 1. 校验验证码是否正确。
        /// 2. 查询用户是否存在。
        /// 3. 校验密码是否正确。
        /// 4. 生成并返回JWT Token。
        /// </remarks>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResult<LoginResultDto>> LoginAsync(LoginDto dto)
        {
            try
            {
                // 1. 校验验证码
                if (!captcha.Validate(dto.CaptchaKey, dto.CaptchaCode))
                {
                    return ApiResult<LoginResultDto>.Fail("验证码错误", ResultCode.Error);
                }
                // 2. 查询用户
                var user = await userRep.FindAsync(x => x.UserName == dto.Username);
                if (user == null)
                {
                    return ApiResult<LoginResultDto>.Fail("用户不存在", ResultCode.NotFound);
                }
                else
                {
                    // 3. 校验密码
                    if (user.Password != dto.Password)
                    {
                        return ApiResult<LoginResultDto>.Fail("密码错误", ResultCode.ValidationError);
                    }
                    else
                    {
                        // 4. 生成Token并返回
                        return ApiResult<LoginResultDto>.Success(GenerateToken(user), ResultCode.Success);
                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResult<LoginResultDto>> Login(string Username,string Password)
        {
            try
            {
                
                var user = await userRep.FindAsync(x => x.UserName == Username);
                if (user == null)
                {
                    return ApiResult<LoginResultDto>.Fail("用户不存在", ResultCode.NotFound);
                }
                else
                {
                    // 3. 校验密码
                    if (user.Password != Password)
                    {
                        return ApiResult<LoginResultDto>.Fail("密码错误", ResultCode.ValidationError);
                    }
                    else
                    {
                        // 4. 生成Token并返回
                        return ApiResult<LoginResultDto>.Success(GenerateToken(user), ResultCode.Success);
                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 生成JWT Token，包含用户基本信息和过期时间
        /// </summary>
        /// <param name="user">用户实体</param>
        /// <returns>登录结果DTO，包含Token、过期时间、用户信息等</returns>
        /// <remarks>
        /// 1. 构造JWT声明（用户名、ID、昵称等）。
        /// 2. 读取配置文件中的密钥，生成对称加密密钥。
        /// 3. 选择HmacSha256算法。
        /// 4. 设置Token过期时间（10小时）。
        /// 5. 构造JwtSecurityToken对象。
        /// 6. 生成Token字符串。
        /// 7. 返回LoginResultDto。
        /// </remarks>
        private LoginResultDto GenerateToken(SysUser user)
        {
            // 1. 构造JWT声明
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("NickName", user.NickName),
            };

            // 2. JWT密钥转换字节对称密钥
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfig:Bearer:SecurityKey"]));
            // 3. 算法
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 4. 过期时间
            var expires = DateTime.UtcNow.AddHours(10);
            // 5. payload负载
            var token = new JwtSecurityToken(
                configuration["JwtConfig:Bearer:Issuer"],
                configuration["JwtConfig:Bearer:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );
            
            var handler = new JwtSecurityTokenHandler();

            // 6. 生成token
            string jwt = handler.WriteToken(token);
            // 7. 返回结果
            return new LoginResultDto
            {
                AccessToken = jwt,
                Expires = expires,
                TokenType = "Bearer",
                RefreshToken = Guid.NewGuid().ToString(),
                UserName = user.UserName,
                NickName = user.NickName,
                Id = user.Id
            };
        }
        /// <summary>
        /// 获取所有用户信息列表
        /// </summary>
        /// <returns>用户实体列表</returns>
        /// <remarks>
        /// 1. 查询所有用户。
        /// 2. 返回ApiResult包装的用户列表。
        /// </remarks>
        //[HttpGet("/api/v1/user/GetUser")]
        public async Task<ApiResult<List<SysUser>>> GetUserAsync()
        {
            var user = await userRep.GetListAsync(); // 查询所有用户
            return ApiResult<List<SysUser>>.Success(user,ResultCode.Success); // 返回结果
        }
        /// <summary>
        /// 初始化用户数据，插入一个默认管理员账号
        /// </summary>
        /// <returns>插入的用户实体</returns>
        /// <remarks>
        /// 1. 构造默认管理员用户对象。
        /// 2. 插入数据库。
        /// 3. 返回插入结果。
        /// </remarks>
        //[AllowAnonymous]
       // [HttpGet("/api/v1/user/Init")]
        public async Task<ApiResult<SysUser>> InitDataAsync()
        {
            try
            {
                // 构造默认管理员用户
                SysUser sysUser = new SysUser
                {
                    UserName = "admin",
                    NickName = "张三",
                    Gender = Gender.Male,
                    Password = "123456",
                    Mobile = "18541202154",
                    Status = Status.Enable,
                    Email = "admin@qq.com"
                };
                await userRep.InsertAsync(sysUser); // 插入数据库
                return ApiResult<SysUser>.Success(sysUser,ResultCode.Success); // 返回结果
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
