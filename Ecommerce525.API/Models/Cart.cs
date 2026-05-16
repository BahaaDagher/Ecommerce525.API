using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce525.API.Models
{
    [PrimaryKey(nameof(ProductId) , nameof(ApplicationUserId))]
    public class Cart
    {
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; } 
        public int Count { get; set; }
        public decimal  Price { get; set; }

    }
}
