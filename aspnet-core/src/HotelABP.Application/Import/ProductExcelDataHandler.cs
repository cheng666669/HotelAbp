using HotelABP.RoomNummbers;
using HotelABP.RoomTypes;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace HotelABP.Import
{
  
    public class ProductExcelDataHandler :  IExcelDataHandler<RoomNummber>, ITransientDependency
    {
        
        private readonly IRepository<RoomNummber, Guid> _productRepository;

        public ProductExcelDataHandler(IRepository<RoomNummber, Guid> productRepository)
        {
            _productRepository = productRepository;
        }
 
        public async Task<int> HandleAsync(Stream stream)
        {
            var workbook = new XSSFWorkbook(stream);
            var sheet = workbook.GetSheetAt(0);
            var entities = new List<RoomNummber>();

            for (int i = 1; i <= sheet.LastRowNum; i++) // 假设第一行为表头
            {
                var row = sheet.GetRow(i);
                if (row == null) continue;

                var entity = new RoomNummber
                {
                    // 假设第0列为房号，第1列为类型等，按实际字段映射
                    RoomTypeId = row.GetCell(1)?.ToString(),
                    RoomNum = row.GetCell(2)?.ToString(),
                    Order = GetIntCellValue(row.GetCell(3)),
                    Description = row.GetCell(4)?.ToString(),
                    State = GetBoolCellValue(row.GetCell(14)),
                    RoomState = GetIntCellValue(row.GetCell(15))
                };
                entities.Add(entity);
            }

            await _productRepository.InsertManyAsync(entities, autoSave: true);
            return entities.Count;
        }

        private int GetIntCellValue(NPOI.SS.UserModel.ICell cell)
        {
            if (cell == null) return 0;
            if (cell.CellType == NPOI.SS.UserModel.CellType.Numeric) return (int)cell.NumericCellValue;
            if (cell.CellType == NPOI.SS.UserModel.CellType.String && int.TryParse(cell.StringCellValue, out int v)) return v;
            return 0;
        }

        private bool GetBoolCellValue(NPOI.SS.UserModel.ICell cell)
        {
            if (cell == null) return false;
            if (cell.CellType == NPOI.SS.UserModel.CellType.Boolean) return cell.BooleanCellValue;
            if (cell.CellType == NPOI.SS.UserModel.CellType.Numeric) return cell.NumericCellValue != 0;
            if (cell.CellType == NPOI.SS.UserModel.CellType.String && bool.TryParse(cell.StringCellValue, out bool v)) return v;
            return false;
        }
    }
}
