using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelABP.Export
{
    /// <summary>
    /// 导出操作的结果DTO，包含文件内容、文件名和内容类型。
    /// </summary>
    public class ExportResultDto
    {
        /// <summary>
        /// 文件内容的字节数组。
        /// </summary>
        public byte[] FileBytes { get; set; }

        /// <summary>
        /// 导出的文件名。
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件的MIME类型，例如 "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"。
        /// </summary>
        public string ContentType { get; set; }
    }
}
