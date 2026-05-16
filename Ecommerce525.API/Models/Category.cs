using Ecommerce525.API.Validations;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce525.API.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        //[MinLength(3)]
        //[MaxLength(30)]
        [MinMaxCustome(3,30)]
        public string Name { get; set; } = string.Empty; 
        public string? Description   { get; set; }
        public bool Status{ get; set; }
    }
}
