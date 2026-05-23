namespace Ecommerce525.API.DTOs.Response
{
    public class ProductsWithRelatedResponse
    {
        public IEnumerable<Product> Products { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
