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
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;


namespace HotelABP.User
{
    //[ApiExplorerSettings(GroupName = "v1")]
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
        /// 生成验证码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Captcha(string id)
        {
            var info = captcha.Generate(id);
            return new FileContentResult(info.Bytes, "image/gif");
        }
        /// <summary>
        /// 演示时使用HttpGet传参方便，这里仅做返回处理
        /// </summary>
        [HttpGet("validate")]
        public bool Validate(string id, string code)
        {
            return captcha.Validate(id, code);
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResult<LoginResultDto>> LoginAsync(LoginDto dto)
        {
            try
            {
                // 验证码校验
                if (!captcha.Validate(dto.CaptchaKey, dto.CaptchaCode))
                {
                    return ApiResult<LoginResultDto>.Fail("验证码错误", ResultCode.Error);
                }
                var user = await userRep.FindAsync(x => x.UserName == dto.Username);
                if (user == null)
                {
                    return ApiResult<LoginResultDto>.Fail("用户不存在", ResultCode.NotFound);
                }
                else
                {
                    if(user.Password != dto.Password)
                    {
                        return ApiResult<LoginResultDto>.Fail("密码错误", ResultCode.ValidationError);
                    }
                    else
                    {
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
        /// 生成token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private LoginResultDto GenerateToken(SysUser user)
        {
            //声明
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("NickName", user.NickName),
            };

            //JWT密钥转换字节对称密钥
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfig:Bearer:SecurityKey"]));
            //算法
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //过期时间
            var expires = DateTime.UtcNow.AddHours(10);
            //payload负载
            var token = new JwtSecurityToken(
                configuration["JwtConfig:Bearer:Issuer"],
                configuration["JwtConfig:Bearer:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );
            
            var handler = new JwtSecurityTokenHandler();

            //生成token
            string jwt = handler.WriteToken(token);
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
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        //[HttpGet("/api/v1/user/GetUser")]
        public async Task<ApiResult<List<SysUser>>> GetUserAsync()
        {
            var user = await userRep.GetListAsync();
            return ApiResult<List<SysUser>>.Success(user,ResultCode.Success);
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <returns></returns>
        //[AllowAnonymous]
       // [HttpGet("/api/v1/user/Init")]
        public async Task<ApiResult<SysUser>> InitDataAsync()
        {
            try
            {
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
                await userRep.InsertAsync(sysUser);
                return ApiResult<SysUser>.Success(sysUser,ResultCode.Success);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
