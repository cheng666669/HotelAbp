using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace HotelABP.Export
{
    /// <summary>
    /// 公用的导出服务实现。
    /// </summary>
    public class ExportAppService : ApplicationService, IExportAppService, ITransientDependency
    {
        [UnitOfWork(isTransactional: false)]
        public Task<IRemoteStreamContent> ExportToExcelAsync<T>(ExportDataDto<T> input) where T : class
        {
            // 声明临时文件名和路径
            string tempFileName = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());

            // 使用 try-finally 块确保临时文件被清理
            try
            {
                // 使用 using 块包裹 FileStream 和 Workbook
                using (var fileStream = new FileStream(tempFileName, FileMode.Create, FileAccess.Write))
                {
                    using (IWorkbook workbook = new XSSFWorkbook())
                    {
                        ISheet sheet = workbook.CreateSheet("Sheet1");
                        int currentRowIndex = 0; // 记录当前写入的行索引

                        // 检查输入数据是否为空
                        if (input.Items == null || !input.Items.Any())
                        {
                            workbook.Write(fileStream); // 写入到文件流
                            // 返回一个空的 RemoteStreamContent，注意这里需要读取文件
                            return Task.FromResult<IRemoteStreamContent>(new RemoteStreamContent(
                                new MemoryStream(File.ReadAllBytes(tempFileName)), // 从临时文件读取字节
                                fileName: $"{input.FileName}_{DateTime.Now:yyyyMMddHHmmss}.xlsx",
                                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                            ));
                        }

                        var list = input.Items.ToList();

                        // **关键修改：根据 ColumnMappings 获取要导出的属性**
                        // 获取T类型的所有公共可读属性
                        var allProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                    .Where(p => p.CanRead);

                        List<PropertyInfo> propertiesToExport;
                        // 如果提供了列映射，则只导出映射中包含的属性，并按映射键的顺序排序
                        if (input.ColumnMappings != null && input.ColumnMappings.Any())
                        {
                            propertiesToExport = input.ColumnMappings.Keys
                                .Select(key => allProperties.FirstOrDefault(p => p.Name == key))
                                .Where(p => p != null) // 过滤掉 ColumnMappings 中不存在于T类型的属性
                                .ToList();
                        }
                        else
                        {
                            // 如果没有提供列映射，则导出所有公共可读属性
                            propertiesToExport = allProperties.ToList();
                        }

                        // --- 1. 在最上面加一行居中标题 ---
                        // 1.1. 创建标题样式
                        ICellStyle titleStyle = workbook.CreateCellStyle();
                        titleStyle.Alignment = HorizontalAlignment.Center; // 水平居中
                        titleStyle.VerticalAlignment = VerticalAlignment.Center; // 垂直居中
                        IFont titleFont = workbook.CreateFont();
                        titleFont.IsBold = true; // 加粗
                        titleFont.FontHeightInPoints = 16; // 字体大小
                        titleStyle.SetFont(titleFont);

                        // 1.2. 创建标题行
                        IRow titleRow = sheet.CreateRow(currentRowIndex);
                        ICell titleCell = titleRow.CreateCell(0); // 在第一列创建单元格
                        titleCell.SetCellValue(input.FileName); // 将文件名作为标题
                        titleCell.CellStyle = titleStyle; // 应用样式

                        // 1.3. 合并单元格
                        // 合并从第0行第0列到第0行最后一列的单元格
                        int lastColumnIndex = propertiesToExport.Count - 1; // 这里的列数是根据要导出的属性数量来确定的
                        CellRangeAddress cellRange = new CellRangeAddress(0, 0, 0, lastColumnIndex);
                        if (propertiesToExport.Count > 0)
                        {
                            sheet.AddMergedRegion(cellRange);
                        }

                        currentRowIndex++; // 标题占了一行，行索引向下移动

                        // --- 2. 创建表头 ---
                        var headerRow = sheet.CreateRow(currentRowIndex);
                        for (int i = 0; i < propertiesToExport.Count; i++)
                        {
                            var prop = propertiesToExport[i];
                            string headerName = prop.Name;
                            // 使用 ColumnMappings 中的值作为列头，如果没有则使用属性名
                            if (input.ColumnMappings != null && input.ColumnMappings.ContainsKey(prop.Name))
                            {
                                headerName = input.ColumnMappings[prop.Name];
                            }
                            headerRow.CreateCell(i).SetCellValue(headerName);
                        }
                        currentRowIndex++; // 表头占了一行，行索引继续向下移动

                        // --- 3. 填充数据行 ---
                        for (int i = 0; i < list.Count; i++)
                        {
                            var item = list[i];
                            var dataRow = sheet.CreateRow(currentRowIndex + i); // 从当前行索引开始填充数据

                            for (int j = 0; j < propertiesToExport.Count; j++)
                            {
                                var prop = propertiesToExport[j];
                                var value = prop.GetValue(item);

                                if (value != null)
                                {
                                    if (value is bool boolValue) dataRow.CreateCell(j).SetCellValue(boolValue ? "是" : "否");
                                    else if (value is DateTime dateValue) dataRow.CreateCell(j).SetCellValue(dateValue.ToString("yyyy-MM-dd HH:mm:ss"));
                                    else if (value is IFormattable formattableValue) dataRow.CreateCell(j).SetCellValue(formattableValue.ToString());
                                    else dataRow.CreateCell(j).SetCellValue(value.ToString());
                                }
                                else
                                {
                                    dataRow.CreateCell(j).SetCellValue("");
                                }
                            }
                        }

                        // --- 4. 自动调整列宽 ---
                        for (int i = 0; i < propertiesToExport.Count; i++)
                        {
                            sheet.AutoSizeColumn(i);
                            // 添加代码，手动增加一些列宽，以确保内容完整显示
                            sheet.SetColumnWidth(i, sheet.GetColumnWidth(i) + 256 * 2);
                        }

                        // --- 5. 将工作簿写入文件流 ---
                        workbook.Write(fileStream); // 写入到文件流
                    } // using (workbook) 块结束
                } // using (fileStream) 块结束

                // --- 6. 从临时文件读取字节并返回 ---
                byte[] fileBytes = File.ReadAllBytes(tempFileName);
                MemoryStream memoryStream = new MemoryStream(fileBytes);

                // 返回 RemoteStreamContent
                return Task.FromResult<IRemoteStreamContent>(new RemoteStreamContent(
                    memoryStream,
                    fileName: $"{input.FileName}_{DateTime.Now:yyyyMMddHHmmss}.xlsx",
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                ));
            }
            finally
            {
                // 确保临时文件被删除
                if (File.Exists(tempFileName))
                {
                    File.Delete(tempFileName);
                }
            }
        }
    }
}
