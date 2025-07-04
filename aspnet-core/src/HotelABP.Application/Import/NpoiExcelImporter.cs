using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.DependencyInjection;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace HotelABP.Import
{
    //public class NpoiExcelImporter<TDto> : IExcelImporter<TDto>, ITransientDependency where TDto : new()
    //{
    //    private readonly IExcelDataHandler<TDto> _handler;

    //    public NpoiExcelImporter(IExcelDataHandler<TDto> handler)
    //    {
    //        _handler = handler;
    //    }

    //    public async Task ImportAsync(Stream stream)
    //    {
    //        var items = new List<TDto>();
    //        var errors = new List<string>();
    //        var workbook = new XSSFWorkbook(stream);
    //        var sheet = workbook.GetSheetAt(0);
    //        var props = typeof(TDto).GetProperties();

    //        for (int i = 1; i <= sheet.LastRowNum; i++)
    //        {
    //            var row = sheet.GetRow(i);
    //            if (row == null) continue;

    //            var item = new TDto();
    //            bool rowHasError = false;

    //            for (int j = 0; j < props.Length && j < row.LastCellNum; j++)
    //            {
    //                var cell = row.GetCell(j);
    //                if (cell == null) continue;

    //                var prop = props[j];
    //                object value = null;

    //                try
    //                {
    //                    switch (cell.CellType)
    //                    {
    //                        case CellType.String:
    //                            value = cell.StringCellValue?.Trim();
    //                            break;
    //                        case CellType.Numeric:
    //                            value = DateUtil.IsCellDateFormatted(cell) ? cell.DateCellValue : cell.NumericCellValue;
    //                            break;
    //                        case CellType.Boolean:
    //                            value = cell.BooleanCellValue;
    //                            break;
    //                    }

    //                    if (value != null)
    //                    {
    //                        var converted = Convert.ChangeType(value, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
    //                        prop.SetValue(item, converted);
    //                    }
    //                }
    //                catch (Exception ex)
    //                {
    //                    rowHasError = true;
    //                    errors.Add($"第{i + 1}行第{j + 1}列转换失败：{ex.Message}");
    //                }
    //            }

    //            if (!rowHasError)
    //            {
    //                items.Add(item);
    //            }
    //        }

    //        var memoryStream = new MemoryStream();
    //        using (var writer = new StreamWriter(memoryStream, leaveOpen: true))
    //        {
    //            foreach (var item in items)
    //            {
    //                var values = props.Select(p => p.GetValue(item)?.ToString() ?? string.Empty);
    //                writer.WriteLine(string.Join(",", values));
    //            }
    //            writer.Flush();
    //            memoryStream.Position = 0;
    //        }
    //        await _handler.HandleAsync(items);
    //    }
    //}

    //[ApiController]
    //[Route("api/[controller]")]
    //public class ExcelImportController<TDto> : ControllerBase where TDto : new()
    //{
    //    private readonly IExcelImporter<TDto> _excelImporter;

    //    public ExcelImportController(IExcelImporter<TDto> excelImporter)
    //    {
    //        _excelImporter = excelImporter;
    //    }

    //    [HttpPost("import")]
    //    public async Task<IActionResult> Import(IFormFile file)
    //    {
    //        if (file == null || file.Length == 0)
    //            return BadRequest("Empty file.");

    //        using (var stream = file.OpenReadStream())
    //        {
    //            await _excelImporter.ImportAsync(stream);
    //        }

    //        return Ok("Import successful");
    //    }
    //}
}
