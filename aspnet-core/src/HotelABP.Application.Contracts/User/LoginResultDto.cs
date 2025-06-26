namespace HotelABP.User
{
    public class LoginResultDto
    {
        public string AccessToken { get; set; }
        public object Expires { get; set; }
        public object RefreshToken { get; set; }
        public string TokenType { get; set; }
    }
}
