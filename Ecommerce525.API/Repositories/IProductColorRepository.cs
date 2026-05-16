using Microsoft.EntityFrameworkCore;

namespace Ecommerce525.API.Repositories
{
    public interface IProductColorRepository : IRepository<ProductColor>
    {
        void RemoveRange(IEnumerable<ProductColor> productColors);
    }
}
