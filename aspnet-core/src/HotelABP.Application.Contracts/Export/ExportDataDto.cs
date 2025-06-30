using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.Export
{
    /// <summary>
    /// 导出数据请求DTO。
    /// </summary>
    /// <typeparam name="T">要导出的数据类型。</typeparam>
    public class ExportDataDto<T>
    {
        /// <summary>
        /// 要导出的数据集合。
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// 导出文件的名称（不含扩展名）。
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 自定义列头映射（可选）。
        /// Key: 属性名（Property Name），Value: 显示名称（Display Name）。
        /// 如果不设置，则使用属性名作为列头。
        /// </summary>
        public Dictionary<string, string> ColumnMappings { get; set; }
    }
}
