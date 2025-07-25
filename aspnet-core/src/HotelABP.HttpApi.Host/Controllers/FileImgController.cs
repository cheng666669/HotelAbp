﻿using Aliyun.OSS;
using HotelABP.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;

namespace HotelABP.Controllers
{
    /// <summary>
    /// 文件上传
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [IgnoreAntiforgeryToken]
    [ApiExplorerSettings(GroupName = "fileimg")]
    public class FileImgController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHost;
        private readonly AliyunOssService _aliyunOssService;
        private readonly AliyunOptions _options;

        public FileImgController(
            IWebHostEnvironment webHost,
            AliyunOssService aliyunOssService,
            IOptions<AliyunOptions> options)
        {
            _webHost = webHost;
            _aliyunOssService = aliyunOssService;
            _options = options.Value; // 正确方式
        }
        /// <summary>
        /// 异常测试接口
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("error")]
        public IActionResult ThrowError()
        {
            throw new Exception("测试异常");
        }
       
        [HttpPost]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            // 检查上传的文件列表是否为空或数量为0
            if (files == null || files.Count == 0)
            {
                // 返回400错误，提示上传文件为空
                return BadRequest("上传文件为空。");
            }

            // 用于存储所有上传后文件的相对路径
            var resultList = new List<string>();
            // 获取Web根目录（wwwroot），用于保存文件
            var webRootPath = _webHost.WebRootPath;
            // 如果WebRootPath为空（某些环境下可能为null），则手动拼接wwwroot路径
            if (string.IsNullOrEmpty(webRootPath))
            {
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }
            // 以当前日期（yyyy-MM-dd）作为文件夹，便于管理和查找
            var dateFolder = DateTime.Now.ToString("yyyy-MM-dd");
            // 拼接最终上传文件夹路径：如 wwwroot/uploads/2024-05-01
            var uploadFolder = Path.Combine(webRootPath, "uploads", dateFolder);

            // 如果上传目录不存在，则自动创建
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // 遍历每一个上传的文件
            foreach (var file in files)
            {
                // 获取文件扩展名（如 .jpg、.png）
                var fileExtension = Path.GetExtension(file.FileName);
                // 生成唯一的新文件名：时间戳+GUID，防止重名覆盖
                var newFileName = $"{DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid()}{fileExtension}";
                // 拼接完整的物理保存路径
                var filePath = Path.Combine(uploadFolder, newFileName);

                // 使用FileStream将文件内容写入到服务器本地磁盘
                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream); // 异步写入文件内容
                }

                // 生成文件的相对路径，便于前端访问
                var relativePath = $"/uploads/{dateFolder}/{newFileName}";
                // 添加到结果列表
                resultList.Add(relativePath);
            }

            // 返回200 OK，包含所有上传文件的相对路径
            return Ok(new { filePaths = resultList });
        }

        /// <summary>
        /// 上传单个视频文件到阿里云OSS，并返回上传结果（如URL或Key）。
        /// </summary>
        /// <param name="file">前端上传的视频文件，类型为IFormFile。</param>
        /// <returns>
        /// 返回阿里云OSS上传结果（通常为文件URL或唯一标识符）。
        /// 如果文件为空，则抛出UserFriendlyException异常。
        /// </returns>
        [HttpPost("UploadVideoAsync")]
        [DisableRequestSizeLimit] // 允许上传大文件（如视频），不限制请求体大小
        public async Task<string> UploadVideoAsync(IFormFile file)
        {
            // 检查文件是否为空或长度为0
            if (file == null || file.Length == 0)
                // 抛出友好异常，前端可直接显示该错误信息
                throw new UserFriendlyException("文件不能为空");

            // 打开文件的只读流，准备上传到OSS
            using var stream = file.OpenReadStream();
            // 调用阿里云OSS服务的上传方法，将文件流和文件名传递过去
            var result = _aliyunOssService.UploadVideo(stream, file.FileName);
            // 返回OSS上传结果（如URL或Key）
            return result;
        }

     

[HttpGet("GetVideoList")]
        public List<string> GetVideoList()
        {
            var client = new OssClient(_options.Endpoint, _options.AccessKeyId, _options.AccessKeySecret);
            var videoList = new List<string>();

            var listRequest = new ListObjectsRequest(_options.BucketName)
            {
                Prefix = _options.VideoFolder
            };

            var result = client.ListObjects(listRequest);

            foreach (var summary in result.ObjectSummaries)
            {
                // 生成带签名的 URL，有效期比如 1 年
                var expiration = DateTime.Now.AddYears(1);

                var signedUri = client.GeneratePresignedUri(_options.BucketName, summary.Key, expiration);

                // signedUri 是 Uri 类型，ToString() 就是完整带签名的 URL
                videoList.Add(signedUri.ToString());
            }

            return videoList;
        }


    }
}
