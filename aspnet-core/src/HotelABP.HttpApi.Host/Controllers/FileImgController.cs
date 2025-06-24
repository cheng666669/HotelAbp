using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HotelABP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileImgController : ControllerBase
    {
        IWebHostEnvironment webHost;
        
        public FileImgController(IWebHostEnvironment webHost)
        {
            this.webHost = webHost;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("上传文件为空。");
            }

            try
            {
                var webRootPath = webHost.WebRootPath;
                if (string.IsNullOrEmpty(webRootPath))
                {
                    webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }

                var dateFolder = DateTime.Now.ToString("yyyy-MM-dd");
                var uploadFolder = Path.Combine(webRootPath, "uploads", dateFolder);

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var fileExtension = Path.GetExtension(file.FileName);
                var newFileName = $"{DateTime.Now:yyyyMMddHHmmssfff}{fileExtension}";
                var filePath = Path.Combine(uploadFolder, newFileName);

                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativePath = $"/uploads/{dateFolder}/{newFileName}";
                return Ok(new { filePath = relativePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器内部错误: {ex.Message}");
            }
        }
    }
}
