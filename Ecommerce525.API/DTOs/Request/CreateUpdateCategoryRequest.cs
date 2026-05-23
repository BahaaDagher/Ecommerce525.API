namespace Ecommerce525.API.DTOs.Request
{
    public class CreateUpdateCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Status { get; set; }
        public IFormFile? ImgFile { get; set; }
    }
}
