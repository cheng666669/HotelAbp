using HotelABP.EntityFrameworkCore;
using HotelABP.MultiTenancy;
using HotelABP.RoomReserves;
using HotelABP.Services;
using HotelABP.EntityFrameworkCore;
using HotelABP.Import;
using HotelABP.MultiTenancy;
using HotelABP.MultiTenancy;
using HotelABP.Services;
using HotelABP.MultiTenancy;
using HotelABP.MultiTenancy;
using HotelABP.RoomReserves;
using Lazy.Captcha.Core.Generator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using SkiaSharp;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Volo.Abp;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

namespace HotelABP;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(HotelABPApplicationModule),
    typeof(HotelABPEntityFrameworkCoreModule),
    typeof(AbpAspNetCoreMvcUiLeptonXLiteThemeModule),
    typeof(AbpAccountWebOpenIddictModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpCachingStackExchangeRedisModule)
)]
public class HotelABPHttpApiHostModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("HotelABP");
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //343443434334
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        Configure<AbpAntiForgeryOptions>(options =>
        {
            options.AutoValidate = false; // ⛔ 禁用全局防伪验证
        });
        //context.Services.AddTransient(typeof(IExcelImporter<>), typeof(NpoiExcelImporter<>));
        // 绑定 Alipay 配置
        context.Services.Configure<AlipayOptions>(configuration.GetSection("Alipay"));

        ConfigureAuthentication(context);

        context.Services.Configure<AliyunOptions>(context.Services.GetConfiguration().GetSection("Aliyun"));
        context.Services.AddTransient<AliyunOssService>();
        ConfigureBundles();
        ConfigureUrls(configuration);
        ConfigureConventionalControllers();
        ConfigureVirtualFileSystem(context);
        ConfigureCors(context, configuration);
        ConfigureSwaggerServices(context, configuration);
        context.Services.AddCaptcha(context.Services.GetConfiguration(), option =>
        {
            option.CaptchaType = CaptchaType.WORD;
            option.CodeLength = 4;
            option.ExpirySeconds = 60;
            option.ImageOption.FontFamily = SKTypeface.FromFamilyName("DejaVu Sans");
            // 不设置 FontFamily，使用系统默认字体
        });
        // 注册全局异常过滤器
        context.Services.AddControllers(options =>
        {
            options.Filters.Add<GlobalExceptionFilter>();
        });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        context.Services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
                .AddJwtBearer(
                option =>
                {
                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        //是否验证发行人
                        ValidateIssuer = true,
                        ValidIssuer = configuration["JwtConfig:Bearer:Issuer"],//发行人

                        //是否验证受众人
                        ValidateAudience = true,
                        ValidAudience = configuration["JwtConfig:Bearer:Audience"],//受众人

                        //是否验证密钥
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfig:Bearer:SecurityKey"])),

                        ValidateLifetime = true, //验证生命周期

                        RequireExpirationTime = true, //过期时间

                        ClockSkew = TimeSpan.FromSeconds(30)   //平滑过期偏移时间
                    };
                }
            );
    }

    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                LeptonXLiteThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );
        });
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            options.RedirectAllowedUrls.AddRange(configuration["App:RedirectAllowedUrls"]?.Split(',') ?? Array.Empty<string>());

            //options.Applications["Angular"].RootUrl = configuration["App:ClientUrl"];
            //options.Applications["Angular"].Urls[AccountUrlNames.PasswordReset] = "account/reset-password";
        });
    }

    private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<HotelABPDomainSharedModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}HotelABP.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<HotelABPDomainModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}HotelABP.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<HotelABPApplicationContractsModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}HotelABP.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<HotelABPApplicationModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}HotelABP.Application"));
            });
        }
    }

    private void ConfigureConventionalControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(HotelABPApplicationModule).Assembly);
        });
    }

    private static void ConfigureSwaggerServices(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddSwaggerGen(options =>
        {
            // Correcting the misplaced code and ensuring proper syntax
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelABP API", Version = "v1" });
            options.SwaggerDoc("label", new OpenApiInfo { Title = "标签管理API", Version = "v1" });
            options.SwaggerDoc("customer", new OpenApiInfo { Title = "客户管理API", Version = "v1" });
            options.SwaggerDoc("account", new OpenApiInfo { Title = "用户管理API", Version = "v1" });
            options.SwaggerDoc("apipay", new OpenApiInfo { Title = "支付管理API", Version = "v1" });
            options.SwaggerDoc("fileimg", new OpenApiInfo { Title = "上传文件/视频管理API", Version = "v1" });
            options.SwaggerDoc("import", new OpenApiInfo { Title = "导入管理API", Version = "v1" });
            options.SwaggerDoc("menu", new OpenApiInfo { Title = "菜单管理API", Version = "v1" });
            options.SwaggerDoc("reserveroom", new OpenApiInfo { Title = "预订管理API", Version = "v1" });
            options.SwaggerDoc("role", new OpenApiInfo { Title = "角色管理API", Version = "v1" });
            options.SwaggerDoc("roomnum", new OpenApiInfo { Title = "房号管理API", Version = "v1" });
            options.SwaggerDoc("roomstate", new OpenApiInfo { Title = "房态管理API", Version = "v1" });
            options.SwaggerDoc("roomtype", new OpenApiInfo { Title = "房型管理API", Version = "v1" });
            options.SwaggerDoc("user", new OpenApiInfo { Title = "用户登录管理API", Version = "v1" });
            options.SwaggerDoc("store", new OpenApiInfo { Title = "门店管理API", Version = "v1" });
            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                if (apiDesc.GroupName == null) return docName == "v1"; // 没有分组的归到v1
                return apiDesc.GroupName == docName;
            });
            options.CustomSchemaIds(type => type.FullName);

            // Adding security definitions
            options.OperationFilter<AddResponseHeadersFilter>();
            options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
            options.OperationFilter<SecurityRequirementsOperationFilter>();

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "JWT授权(数据将在请求头中进行传递)直接在下面框中输入Bearer {token}(注意两者之间是一个空格) \"",
                Name = "Authorization", // jwt默认的参数名称
                In = ParameterLocation.Header, // jwt默认存放Authorization信息的位置(请求头中)
                Type = SecuritySchemeType.ApiKey
            });

            // Including XML comments for Swagger documentation
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var xmlPath = Path.Combine(basePath, "HotelABP.Application.xml"); // XML file name
            var xmlPaths = Path.Combine(basePath, "HotelABP.HttpApi.Host.xml"); // XML file name
            options.IncludeXmlComments(xmlPath, true); // Ensuring controller comments are included
            options.IncludeXmlComments(xmlPaths, true);
        });
    }


    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(configuration["App:CorsOrigins"]?
                        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(o => o.RemovePostFix("/"))
                        .ToArray() ?? Array.Empty<string>())
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
        }

        app.UseCorrelationId();
        app.MapAbpStaticAssets();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }
        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseSwaggerUI(c => {

            c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotelABP API");
            c.SwaggerEndpoint("/swagger/label/swagger.json", "标签管理API");
            c.SwaggerEndpoint("/swagger/customer/swagger.json", "客户管理API");
            c.SwaggerEndpoint("/swagger/account/swagger.json", "用户管理API");
            c.SwaggerEndpoint("/swagger/apipay/swagger.json", "支付管理API");
            c.SwaggerEndpoint("/swagger/fileimg/swagger.json", "上传文件/视频管理API");
            c.SwaggerEndpoint("/swagger/import/swagger.json", "导入管理API");
            c.SwaggerEndpoint("/swagger/menu/swagger.json", "菜单管理API");
            c.SwaggerEndpoint("/swagger/reserveroom/swagger.json", "预订管理API");
            c.SwaggerEndpoint("/swagger/role/swagger.json", "角色管理API");
            c.SwaggerEndpoint("/swagger/roomnum/swagger.json", "房号管理API");
            c.SwaggerEndpoint("/swagger/roomstate/swagger.json", "房态管理API");
            c.SwaggerEndpoint("/swagger/roomtype/swagger.json", "房型管理API");
            c.SwaggerEndpoint("/swagger/user/swagger.json", "用户登录管理API");
            c.SwaggerEndpoint("/swagger/store/swagger.json", "门店管理API");
            c.RoutePrefix = string.Empty;
            var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
            c.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            c.OAuthScopes("HotelABP");
        });

        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}
