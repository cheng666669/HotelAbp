using HotelABP.RoomPriceCalendar;
using HotelABP.RoomTypes;
using SqlSugar;
using System;
using Volo.Abp.DependencyInjection;

namespace HotelABP.Services
{
    public class DatabaseInitService: ITransientDependency
    {
        private readonly ISqlSugarClient _db;

        public DatabaseInitService(ISqlSugarClient db)
        {
            _db = db;
        }

        public void InitTables()
        {
            _db.CodeFirst.InitTables(
                typeof(RoomPrice),
                typeof(RoomPriceCalendars)
            );
        }
    }
}
