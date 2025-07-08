using HotelABP.Customers;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.DependencyInjection;

namespace HotelABP.Import
{
    public class ImportCustoimers : ITransientDependency
    {
        private readonly IRepository<HotelABPCustoimerss, Guid> _customerRepository;

        public ImportCustoimers(IRepository<HotelABPCustoimerss, Guid> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<int> HandleAsync(Stream stream)
        {
            var workbook = new XSSFWorkbook(stream);
            var sheet = workbook.GetSheetAt(0);
            var entities = new List<HotelABPCustoimerss>();

            for (int i = 1; i <= sheet.LastRowNum; i++) // 假设第一行为表头
            {
                var row = sheet.GetRow(i);
                if (row == null) continue;

                var customerName = row.GetCell(3)?.ToString();
                if (!string.IsNullOrEmpty(customerName) && customerName.Length > 16)
                {
                    customerName = customerName.Substring(0, 16); // 截断到16字符
                }

                var entity = new HotelABPCustoimerss
                {
                    CustomerNickName = row.GetCell(1)?.ToString(),
                    CustomerType = TryParseGuid(row.GetCell(2)),
                    CustomerName = customerName,
                    PhoneNumber = row.GetCell(4)?.ToString(),
                    Gender = GetIntCellValue(row.GetCell(5)),
                    Birthday = GetDateCellValue(row.GetCell(6)),
                    City = row.GetCell(7)?.ToString(),
                    Address = row.GetCell(8)?.ToString(),
                    GrowthValue = GetDecimalCellValue(row.GetCell(9)),
                    AvailableBalance = GetDecimalCellValue(row.GetCell(10)),
                    AvailableGiftBalance = GetDecimalCellValue(row.GetCell(11)),
                    AvailablePoints = GetDecimalCellValue(row.GetCell(12)),
                    //  Rechargeamount = GetDecimalCellValue(row.GetCell(21)),
                    // Sumofconsumption = GetDecimalCellValue(row.GetCell(22)),
                    // CustomerDesc = row.GetCell(20)?.ToString(),
                    //ComsumerNumber = GetIntCellValue(row.GetCell(23)),
                    //Status = GetBoolCellValue(row.GetCell(24)),
                    // ConsumerDesc = row.GetCell(25)?.ToString(),
                    //  Accumulativeconsumption = GetDecimalCellValue(row.GetCell(26))
                };
                entities.Add(entity);
            }

            await _customerRepository.InsertManyAsync(entities, autoSave: true);
            return entities.Count;
        }

        // 工具方法
        private int? GetIntCellValue(ICell cell)
        {
            if (cell == null) return null;
            if (cell.CellType == CellType.Numeric) return (int)cell.NumericCellValue;
            if (cell.CellType == CellType.String && int.TryParse(cell.StringCellValue, out int v)) return v;
            return null;
        }

        private decimal GetDecimalCellValue(ICell cell)
        {
            if (cell == null) return 0;
            if (cell.CellType == CellType.Numeric) return (decimal)cell.NumericCellValue;
            if (cell.CellType == CellType.String && decimal.TryParse(cell.StringCellValue, out decimal v)) return v;
            return 0;
        }

        private bool? GetBoolCellValue(ICell cell)
        {
            if (cell == null) return null;
            if (cell.CellType == CellType.Boolean) return cell.BooleanCellValue;
            if (cell.CellType == CellType.Numeric) return cell.NumericCellValue != 0;
            if (cell.CellType == CellType.String && bool.TryParse(cell.StringCellValue, out bool v)) return v;
            return null;
        }

        private DateTime? GetDateCellValue(ICell cell)
        {
            if (cell == null) return null;
            if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell)) return cell.DateCellValue;
            if (cell.CellType == CellType.String && DateTime.TryParse(cell.StringCellValue, out DateTime v)) return v;
            return null;
        }

        private Guid? TryParseGuid(ICell cell)
        {
            if (cell == null) return null;
            var str = cell.ToString();
            if (Guid.TryParse(str, out Guid guid)) return guid;
            return null;
        }
    }
}
