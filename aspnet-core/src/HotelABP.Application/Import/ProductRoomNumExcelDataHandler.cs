using HotelABP.RoomNummbers;
using Microsoft.AspNetCore.Mvc;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace HotelABP.Import
{
  
    public class ProductRoomNumExcelDataHandler :  IRoomNumExcelDataHandler<RoomNummber>, ITransientDependency
    {
        
        private readonly IRepository<RoomNummber, Guid> _productRepository;

        public ProductRoomNumExcelDataHandler(IRepository<RoomNummber, Guid> productRepository)
        {
            _productRepository = productRepository;
        }
 
        /// <summary>
        /// 处理Excel流，批量导入房号数据（支持NPOI解析、DTO映射、批量插入）
        /// </summary>
        /// <param name="stream">Excel文件流</param>
        /// <returns>成功导入的数据条数</returns>
        /// <remarks>
        /// 1. 使用NPOI解析Excel流，获取第一个Sheet。
        /// 2. 跳过表头，从第二行开始遍历。
        /// 3. 逐行读取单元格，映射为RoomNummber实体。
        /// 4. 支持类型转换（int、bool等）。
        /// 5. 收集所有实体后批量插入数据库。
        /// 6. 返回导入的实体数量。
        /// </remarks>
        public async Task<int> HandleAsync(Stream stream)
        {
            var workbook = new XSSFWorkbook(stream); // 1. 解析Excel流
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

            await _productRepository.InsertManyAsync(entities, autoSave: true); // 5. 批量插入数据库
            return entities.Count; // 6. 返回导入数量
        }

        /// <summary>
        /// 获取单元格的int类型值（支持多种单元格类型）
        /// </summary>
        /// <param name="cell">Excel单元格</param>
        /// <returns>转换后的int值，无法转换时返回0</returns>
        /// <remarks>
        /// 1. 支持数值型、字符串型单元格。
        /// 2. 字符串型尝试int.TryParse。
        /// </remarks>
        private int GetIntCellValue(NPOI.SS.UserModel.ICell cell)
        {
            if (cell == null) return 0;
            if (cell.CellType == NPOI.SS.UserModel.CellType.Numeric) return (int)cell.NumericCellValue;
            if (cell.CellType == NPOI.SS.UserModel.CellType.String && int.TryParse(cell.StringCellValue, out int v)) return v;
            return 0;
        }

        /// <summary>
        /// 获取单元格的bool类型值（支持多种单元格类型）
        /// </summary>
        /// <param name="cell">Excel单元格</param>
        /// <returns>转换后的bool值，无法转换时返回false</returns>
        /// <remarks>
        /// 1. 支持布尔型、数值型、字符串型单元格。
        /// 2. 字符串型尝试bool.TryParse。
        /// </remarks>
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
