using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features; // 需要加上这个命名空间
namespace HotelABP;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs.txt"))
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        try
        {
            Log.Information("Starting HotelABP.HttpApi.Host.");

          

          
            var builder = WebApplication.CreateBuilder(args);

            // 在 builder 创建后添加
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 1024 * 1024 * 1024; // 1GB，根据需要调整
            });
            // 加载默认 appsettings.json
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            // 加载本地机密配置
            builder.Configuration
                .AddJsonFile(@"C:\Users\Administrator\AppData\Roaming\Microsoft\UserSecrets\HotelABP-4681b4fd-151f-4221-84a4-929d86723e4c\secrets.json", optional: true, reloadOnChange: true);

            // 支持环境变量
            builder.Configuration.AddEnvironmentVariables();

            // 替换 ABP 使用的配置
            builder.Services.ReplaceConfiguration(builder.Configuration);

            // 注册CORS
            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowFrontend", policy =>
            //    {
            //        policy
            //            .WithOrigins("http://8.152.98.56:3030") // 允许前端地址
            //            .AllowAnyHeader()
            //            .AllowAnyMethod()
            //            .AllowCredentials(); // 如果前端有带cookie或认证信息
            //    });
            //});

            builder.Host.AddAppSettingsSecretsJson()
                .UseAutofac()
                .UseSerilog();
            await builder.AddApplicationAsync<HotelABPHttpApiHostModule>();
            var app = builder.Build();

            //// 启用CORS
            //app.UseCors("AllowFrontend");

            await app.InitializeApplicationAsync();
            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            if (ex is HostAbortedException)
            {
                throw;
            }

            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
