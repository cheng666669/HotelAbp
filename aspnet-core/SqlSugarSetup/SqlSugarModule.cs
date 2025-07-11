
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using Volo.Abp.Modularity;

namespace SqlSugarSetup
{
    public class SqlSugarModule: AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            context.Services.AddScoped<ISqlSugarClient>(sp =>
            {
                var config = new ConnectionConfig()
                {
                    ConnectionString = configuration["ConnectionStrings:Default"], // 来自 appsettings.json
                    DbType = DbType.MySql, // 你用的数据库类型
                    IsAutoCloseConnection = true,
                    InitKeyType = InitKeyType.Attribute,
                };

                return new SqlSugarClient(config);
            });
        }
    }
}
