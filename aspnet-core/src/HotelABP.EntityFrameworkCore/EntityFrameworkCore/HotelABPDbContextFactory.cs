using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HotelABP.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class HotelABPDbContextFactory : IDesignTimeDbContextFactory<HotelABPDbContext>
{
    public HotelABPDbContext CreateDbContext(string[] args)
    {
        HotelABPEfCoreEntityExtensionMappings.Configure();

        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<HotelABPDbContext>()
            // .UseSqlServer(configuration.GetConnectionString("Default"));
            .UseMySql(configuration.GetConnectionString("Default"), ServerVersion.Parse("5.7.39-mysql"));

        return new HotelABPDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../HotelABP.HttpApi.Host/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
