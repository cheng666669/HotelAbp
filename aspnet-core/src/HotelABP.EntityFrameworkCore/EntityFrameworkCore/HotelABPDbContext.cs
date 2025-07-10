using HotelABP.Customers;
using HotelABP.Grades;
using HotelABP.Labels;
﻿using HotelABP.RoomReserves;
using HotelABP.RoomNummbers;
using HotelABP.RoomTypes;
using Microsoft.EntityFrameworkCore;
using HotelABP.Users;
using Microsoft.EntityFrameworkCore;
using HotelABP.Users;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using HotelABP.Store;


namespace HotelABP.EntityFrameworkCore;


[ConnectionStringName("Default")]
public class HotelABPDbContext :
    AbpDbContext<HotelABPDbContext>
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<Roles> Roles { get; set; }
    public DbSet<SysUser> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    #endregion

    public HotelABPDbContext(DbContextOptions<HotelABPDbContext> options)
        : base(options)
    {

    }

    public DbSet<ReserveRoom> ReserveRooms { get; set; }
    /// <summary>
    /// 房型表
    /// </summary>
    public DbSet<RoomType> RoomTypes { get; set; }
    /// <summary>
    /// 房号表
    /// </summary>
    public DbSet<RoomNummber> RoomNummbers { get; set; }


    public DbSet<HotelABPCustoimerss> HotelABPCustoimerss { get; set; }
    public DbSet<HotelABPLabelss> HotelABPLabelss { get; set; }
    public DbSet<HotelAbpGrades> HotelAbpGrades { get; set; }
    public DbSet<HotelABPCustoimerTypeName> HotelABPCustoimerTypeName { get; set; }
    
    /// <summary>
    /// 门店表
    /// </summary>
    public DbSet<StoreInfo> StoreInfo { get; set; }


    public DbSet<MoneyDetail> MoneyDetails { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */


        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();




        //builder.Entity<Usertinfo>(b =>
        //{
        //    b.ToTable(HotelABPConsts.DbTablePrefix + "Usertinfo", HotelABPConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(HotelABPConsts.DbTablePrefix + "YourEntities", HotelABPConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
    }
}
