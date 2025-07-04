namespace HotelABP
{
    public class AliyunOptions
    {
        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
        public string Endpoint { get; set; }
        public string BucketName { get; set; }
        // 必须添加这一行！
        public string VideoFolder { get; set; }
    }
}
