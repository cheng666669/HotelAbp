using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace HotelABP.Export
{
    /// <summary>
    /// 通用的导出应用服务接口。
    /// </summary>
    public interface IExportAppService : IApplicationService
    {
        /// <summary>
        /// 将数据集合导出为Excel文件并返回文件流。
        /// </summary>
        /// <typeparam name="T">要导出的数据类型。</typeparam>
        /// <param name="input">包含数据集合和文件名等信息的DTO。</param>
        /// <returns>包含文件内容的流、文件名和内容类型。</returns>
        Task<IRemoteStreamContent> ExportToExcelAsync<T>(ExportDataDto<T> input) where T : class;
    }
}
