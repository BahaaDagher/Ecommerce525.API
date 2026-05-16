using System.ComponentModel.DataAnnotations;

namespace Ecommerce525.API.DTOs.Request
{
    public class RegisterRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
