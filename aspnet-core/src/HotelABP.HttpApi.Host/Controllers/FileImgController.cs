using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

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
        [HttpGet("error")]
        public IActionResult ThrowError()
        {
            throw new Exception("测试异常");
        }
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("上传文件为空。");
            }

            var resultList = new List<string>();
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

            foreach (var file in files)
            {
                var fileExtension = Path.GetExtension(file.FileName);
                var newFileName = $"{DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadFolder, newFileName);

                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativePath = $"/uploads/{dateFolder}/{newFileName}";
                resultList.Add(relativePath);
            }

            return Ok(new { filePaths = resultList });
        }
    }
}
