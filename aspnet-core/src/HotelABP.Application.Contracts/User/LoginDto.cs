using System.ComponentModel.DataAnnotations;

namespace HotelABP.User
{
    public class LoginDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        public string CaptchaKey { get; set; }
        [Required]
        public string CaptchaCode { get; set; }
    }
}
