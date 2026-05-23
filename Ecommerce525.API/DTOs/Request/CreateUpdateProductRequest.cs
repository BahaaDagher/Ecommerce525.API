namespace Ecommerce525.API.DTOs.Request
{
    public class CreateUpdateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool Status { get; set; }
        public decimal Discount { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public IFormFile? ImgFile { get; set; }
        public List<IFormFile>? SubImgFiles { get; set; }
        public List<string>? Colors { get; set; }
    }
}
