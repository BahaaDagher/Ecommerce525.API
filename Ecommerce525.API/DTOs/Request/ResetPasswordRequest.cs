using System.ComponentModel.DataAnnotations;

namespace Ecommerce525.API.DTOs.Request
{
    public class ResetPasswordRequest
    {
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }

    }
}
