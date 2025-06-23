using HotelABP.Users;
using Lazy.Captcha.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Tokens;
using Polly.Caching;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;


namespace HotelABP.User
{
    //[ApiExplorerSettings(GroupName = "v1")]
    [Authorize]
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
        /// <returns></returns>
        //[HttpGet("/api/v1/user/Captcha")]
        [AllowAnonymous]
        public ApiResult<CaptchaDto> GetCaptcha()
        {
            string guid = $"captcha:{Guid.NewGuid().ToString("N")}";
            var info = captcha.Generate(guid, 120);
            httpContextAccessor.HttpContext.Response.Cookies.Append("ValidateCode", guid);

            var stream = new System.IO.MemoryStream(info.Bytes);
            return ApiResult<CaptchaDto>.Success(new CaptchaDto
            {
                captchaKey = guid,
                captchaBase64 = $"data:image/gif;base64,{Convert.ToBase64String(stream.ToArray())}"
            },ResultCode.Success);
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
                //var Port = new Uri(httpContextAccessor.HttpContext.Request.Headers["Referer"]).Port;
                //if (!captcha.Validate(httpContextAccessor.HttpContext.Request.Cookies["ValidateCode"], dto.CaptchaCode) && Port == 3000)
                //{
                //    return ApiResult<LoginResultDto>.Fail("验证码错误", ResultCode.ValidationError);
                //}
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
                RefreshToken = Guid.NewGuid().ToString()
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
                    NickName = "管理员",
                    Gender = Gender.Male,
                    Password = "123456",
                    Mobile = "13888888888",
                    Status = Status.Enable,
                    Email = "admin@qq.com"
                };
                await userRep.InsertAsync(sysUser);
                return ApiResult<SysUser>.Success(sysUser,ResultCode.Success);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
