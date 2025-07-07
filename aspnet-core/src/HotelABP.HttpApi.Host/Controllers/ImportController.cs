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
    [Route("api/[controller]")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private readonly ProductRoomNumExcelDataHandler _excelHandler;
        private readonly ImportCustoimers _importCustoimers;

        public ImportController(ProductRoomNumExcelDataHandler excelHandler, ImportCustoimers importCustoimers)
        {
            _excelHandler = excelHandler;
            _importCustoimers = importCustoimers;
        }

        [HttpPost("excel")]
        [DisableRequestSizeLimit] // 可选，允许大文件上传
        public async Task<IActionResult> ImportExcel([FromForm] ImportExcelDto input)
        {
            var file = input.File;
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            using (var stream = file.OpenReadStream())
            {
                var count = await _excelHandler.HandleAsync(stream);
                return Ok(new { ImportedCount = count });
            }
        }

        [HttpPost("customers")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> ImportCustomers([FromForm] ImportExcelDto input)
        {
            var file = input.File;
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            using (var stream = file.OpenReadStream())
            {
                var count = await _importCustoimers.HandleAsync(stream);
                return Ok(new { ImportedCount = count });
            }
        }
    }
}
