using System.ComponentModel.DataAnnotations;

namespace PMS.DTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Tài khoản không được để trống")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; } = "";
    }
}