namespace Ecommerce525.API.Repositories
{
    public interface IProductSubImageRepository : IRepository<ProductSubImage>
    {
        void RemoveRange(IEnumerable<ProductSubImage> productSubImages);
    }
}
