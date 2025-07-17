using Aliyun.OSS;
using Aliyun.OSS.Model;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using Volo.Abp.DependencyInjection;

namespace HotelABP.Services
{
    public class AliyunOssService: ITransientDependency
    {
        private readonly AliyunOptions _options;

        public AliyunOssService(IOptions<AliyunOptions> options)
        {
            _options = options.Value;
        }

        public string UploadVideo(Stream videoStream, string originalFileName,int expireMinutes = 30)
        {
            var client = new OssClient(_options.Endpoint, _options.AccessKeyId, _options.AccessKeySecret);

            // 构建唯一文件名

            // 过滤文件名中的非ASCII字符
            var safeFileName = System.Text.RegularExpressions.Regex.Replace(originalFileName, @"[^\u0000-\u007F]", "_");
            var fileExt = Path.GetExtension(safeFileName);
            var uniqueName = $"{Guid.NewGuid():N}{fileExt}";
            var objectKey = $"{_options.VideoFolder}{uniqueName}";
            // 上传
            client.PutObject(_options.BucketName, objectKey, videoStream);
            // 设置过期时间
            var expiration = DateTime.Now.AddMinutes(expireMinutes);

            // 生成签名 URL 请求
            var req = new GeneratePresignedUriRequest(_options.BucketName, objectKey, SignHttpMethod.Get)
            {
                Expiration = expiration
            };

            // 返回签名后的 URL
            var signedUrl = client.GeneratePresignedUri(req);
            return signedUrl.ToString();
           

        }
    }
}
