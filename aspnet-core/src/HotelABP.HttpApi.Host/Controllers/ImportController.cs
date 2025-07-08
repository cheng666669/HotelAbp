using HotelABP.Import;
using HotelABP.RoomNummbers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HotelABP.Controllers
{
    public class ImportExcelDto
    {
        public IFormFile File { get; set; }
    }
    /// <summary>
    /// 导入Excel
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "import")]
    public class ImportController : ControllerBase
    {
        private readonly ProductRoomNumExcelDataHandler _excelHandler;
        private readonly ImportCustoimers _importCustoimers;

        public ImportController(ProductRoomNumExcelDataHandler excelHandler, ImportCustoimers importCustoimers)
        {
            _excelHandler = excelHandler;
            _importCustoimers = importCustoimers;
        }

        /// <summary>
        /// 导入房间号相关的Excel文件，解析并处理数据。
        /// </summary>
        /// <param name="input">包含上传文件的DTO对象，File属性为前端上传的Excel文件（IFormFile）。</param>
        /// <returns>
        /// 返回导入结果，格式为{"ImportedCount": 数量}，表示成功导入的数据条数。
        /// 如果未上传文件，则返回400 BadRequest。
        /// </returns>
        [HttpPost("excel")]
        [DisableRequestSizeLimit] // 可选，允许大文件上传
        public async Task<IActionResult> ImportExcel([FromForm] ImportExcelDto input)
        {
            // 从DTO中获取上传的文件对象
            var file = input.File;
            // 检查文件是否为空或长度为0
            if (file == null || file.Length == 0)
                // 返回400错误，提示未上传文件
                return BadRequest("No file uploaded.");

            // 打开文件的只读流，准备传递给处理器
            using (var stream = file.OpenReadStream())
            {
                // 调用房间号Excel处理器，异步处理文件流，返回导入的数据条数
                var count = await _excelHandler.HandleAsync(stream);
                // 返回200 OK，包含导入的数据条数
                return Ok(new { ImportedCount = count });
            }
        }

        /// <summary>
        /// 导入客户信息相关的Excel文件，解析并处理数据。
        /// </summary>
        /// <param name="input">包含上传文件的DTO对象，File属性为前端上传的Excel文件（IFormFile）。</param>
        /// <returns>
        /// 返回导入结果，格式为{"ImportedCount": 数量}，表示成功导入的数据条数。
        /// 如果未上传文件，则返回400 BadRequest。
        /// </returns>
        [HttpPost("customers")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> ImportCustomers([FromForm] ImportExcelDto input)
        {
            // 从DTO中获取上传的文件对象
            var file = input.File;
            // 检查文件是否为空或长度为0
            if (file == null || file.Length == 0)
                // 返回400错误，提示未上传文件
                return BadRequest("No file uploaded.");

            // 打开文件的只读流，准备传递给处理器
            using (var stream = file.OpenReadStream())
            {
                // 调用客户信息Excel处理器，异步处理文件流，返回导入的数据条数
                var count = await _importCustoimers.HandleAsync(stream);
                // 返回200 OK，包含导入的数据条数
                return Ok(new { ImportedCount = count });
            }
        }
    }
}
