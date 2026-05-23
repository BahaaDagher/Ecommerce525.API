using Ecommerce525.API.Repositories;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce525.API.Area.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = $" {CD.SUPER_ADMIN_ROLE},{CD.ADMIN_ROLE} ,{CD.EMPLOYEE_ROLE} ")]

    public class ProductsController : ControllerBase
    {

        //ApplicationDbContext _context = new ApplicationDbContext(); 
        IRepository<Product> _productRepository;// = new Repository<Product>();
        IRepository<Category> _categoryRepository;// = new Repository<Category>();
        IRepository<Brand> _brandRepository;// = new Repository<Brand>();
        IProductSubImageRepository _productSubImageRepository;// = new ProductSubImageRepository();
        IProductColorRepository _productColorRepository;// = new ProductColorRepository();

        public ProductsController(IRepository<Product> productRepository, IRepository<Category> categoryRepository, IRepository<Brand> brandRepository, IProductSubImageRepository productSubImageRepository, IProductColorRepository productColorRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
            _productSubImageRepository = productSubImageRepository;
            _productColorRepository = productColorRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] FilterProductRequest filterProductRequest)
        {
            //var products = _context.Products.AsQueryable();
            //products = products.Include(p => p.Category).Include(p => p.Brand);
            var products = await _productRepository.GetAsync(includes: [p => p.Category, p => p.Brand]);

            decimal discount = 50;
            if (filterProductRequest.ProductName is not null)
            {
                products = products.Where(p => p.Name.Contains(filterProductRequest.ProductName));
            }
            if (filterProductRequest.MinPrice > 0)
            {
                products = products.Where(p => (p.Price - (p.Price * p.Discount / 100)) >= filterProductRequest.MinPrice);
            }
            if (filterProductRequest.MaxPrice > 0)
            {
                products = products.Where(p => (p.Price - (p.Price * p.Discount / 100)) <= filterProductRequest.MaxPrice);
            }
            if (filterProductRequest.CategoryId > 0)
            {
                products = products.Where(p => p.CategoryId == filterProductRequest.CategoryId);
            }
            if (filterProductRequest.BrandId > 0)
            {
                products = products.Where(p => p.BrandId == filterProductRequest.BrandId);
            }
            if (filterProductRequest.IsLowQuantity)
            {
                products = products.OrderBy(p => p.Quantity);
            }

            //ViewData["Categories"] = categories;
            var totalPages = (int)Math.Ceiling(products.Count() / 8.0);
            var currentPage = filterProductRequest.Page;
            products = products.Skip((filterProductRequest.Page - 1) * 8).Take(8);

            return Ok(new ApiResponse<ProductsWithRelatedResponse>()
            {
                IsSuccess = true,
                Message = "Products Returned Successfully",
                Data = new ProductsWithRelatedResponse()
                {
                    Products = products,
                    CurrentPage = currentPage,
                    TotalPages = totalPages
                }
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var product = await _productRepository.GetOneAsync(p => p.Id == id);
            if (product is null)
            {
                return NotFound(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "invalid Product id",
                });
            }
            return Ok(new ApiResponse<Product>()
            {
                IsSuccess = true,
                Message = "Product Returned Successfully",
                Data = product
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateUpdateProductRequest createUpdateProductRequest)
        {
            var product = createUpdateProductRequest.Adapt<Product>();
            if (createUpdateProductRequest.ImgFile != null && createUpdateProductRequest.ImgFile.Length > 0)
            {
                //var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgFile.FileName);
                var fileName = Guid.NewGuid().ToString() + "-" + createUpdateProductRequest.ImgFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    createUpdateProductRequest.ImgFile.CopyTo(stream);
                }
                product.MainImg = fileName;
            }
            //var SavedProduct = _context.Products.Add(product);
            var SavedProduct = await _productRepository.CreateAsync(product);
            //_context.SaveChanges();
            await _productRepository.CommitAsync();
            if (createUpdateProductRequest.SubImgFiles != null && createUpdateProductRequest.SubImgFiles.Count > 0)
            {
                foreach (var image in createUpdateProductRequest.SubImgFiles)
                {
                    if (image != null && image.Length > 0)
                    {
                        //var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgFile.FileName);
                        var fileName = Guid.NewGuid().ToString() + "-" + image.FileName;
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images\\ProductSubImages", fileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            image.CopyTo(stream);
                        }
                        //_context.ProductSubImages.Add(new ProductSubImage()
                        await _productSubImageRepository.CreateAsync(new ProductSubImage()
                        {
                            ProductId = SavedProduct.Entity.Id,
                            Img = fileName,
                        });

                    }
                }
                await _productSubImageRepository.CommitAsync();
            }
            if (createUpdateProductRequest.Colors != null && createUpdateProductRequest.Colors.Count > 0)
            {
                foreach (var color in createUpdateProductRequest.Colors)
                {
                    //_context.ProductColors.Add(new ProductColor()
                    await _productColorRepository.CreateAsync(new ProductColor()
                    {
                        ProductId = SavedProduct.Entity.Id,
                        Color = color,
                    });
                }
            }
            await _productColorRepository.CommitAsync();

            return CreatedAtAction(nameof(GetOne), new { id = product.Id }, new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "Product Created Successfully"
            });
        }

        [Authorize(Roles = $" {CD.SUPER_ADMIN_ROLE},{CD.ADMIN_ROLE}")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id , [FromForm] CreateUpdateProductRequest createUpdateProductRequest)
        {
            //var productInDb = _context.Products.AsNoTracking().FirstOrDefault(b => b.Id == product.Id);
            var product = await _productRepository.GetOneAsync(filter: b => b.Id == id);
            product = createUpdateProductRequest.Adapt(product); 
            if (createUpdateProductRequest.ImgFile != null && createUpdateProductRequest.ImgFile.Length > 0)
            {
                //var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgFile.FileName);
                var fileName = Guid.NewGuid().ToString() + "-" + createUpdateProductRequest.ImgFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    createUpdateProductRequest.ImgFile.CopyTo(stream);
                }
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images", product.MainImg);

                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
                product.MainImg = fileName;
            }
            //_context.Products.Update(product);
            //_context.SaveChanges();
            _productRepository.Update(product);
            await _productRepository.CommitAsync();
            if (createUpdateProductRequest.SubImgFiles != null && createUpdateProductRequest.SubImgFiles.Count > 0)
            {
                // remove from DB
                //var oldProductSubImages = _context.ProductSubImages.Where(p => p.ProductId == product.Id); 
                var oldProductSubImages = await _productSubImageRepository.GetAsync(p => p.ProductId == product.Id);
                // remove Form wwwroot
                if (oldProductSubImages != null)
                {
                    //_context.ProductSubImages.RemoveRange(oldProductSubImages); 
                    _productSubImageRepository.RemoveRange(oldProductSubImages);
                    foreach (var item in oldProductSubImages)
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images\\ProductSubImages", item.Img);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                }
                foreach (var image in createUpdateProductRequest.SubImgFiles)
                {
                    if (image != null && image.Length > 0)
                    {
                        // insert in wwwroot
                        //var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgFile.FileName);
                        var fileName = Guid.NewGuid().ToString() + "-" + image.FileName;
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images\\ProductSubImages", fileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            image.CopyTo(stream);
                        }
                        // insert in DB
                        //_context.ProductSubImages.Add(new ProductSubImage()
                        await _productSubImageRepository.CreateAsync(new ProductSubImage()
                        {
                            ProductId = product.Id,
                            Img = fileName,
                        });

                    }
                }
                await _productSubImageRepository.CommitAsync();
            }
            if (createUpdateProductRequest.Colors != null && createUpdateProductRequest.Colors.Count > 0)
            {
                // remove from DB
                //var oldProductColors = _context.ProductColors.Where(p => p.ProductId == product.Id);
                var oldProductColors = await _productColorRepository.GetAsync(p => p.ProductId == product.Id);
                if (oldProductColors is not null)
                {
                    //_context.ProductColors.RemoveRange(oldProductColors); 
                    _productColorRepository.RemoveRange(oldProductColors);
                }
                foreach (var color in createUpdateProductRequest.Colors)
                {
                    //_context.ProductColors.Add(new ProductColor()
                    await _productColorRepository.CreateAsync(new ProductColor()
                    {
                        ProductId = product.Id,
                        Color = color,
                    });
                }
            }
            //_context.SaveChanges();
            await _productColorRepository.CommitAsync();

            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "Product Updated Successfully"
            }); 
        }
        [Authorize(Roles = $" {CD.SUPER_ADMIN_ROLE},{CD.ADMIN_ROLE}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            //var product = _context.Products.FirstOrDefault(c => c.Id == id);
            var product = await _productRepository.GetOneAsync(c => c.Id == id);
            if (product == null)
            {
                return NotFound(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "invalid Product id",
                });
            }
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images", product.MainImg);

            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            //var oldProductSubImages = _context.ProductSubImages.Where(p => p.ProductId == product.Id);
            var oldProductSubImages = await _productSubImageRepository.GetAsync(p => p.ProductId == product.Id);

            foreach (var item in oldProductSubImages)
            {
                var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images\\ProductSubImages", item.Img);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            //_context.Products.Remove(product); 
            //_context.SaveChanges();
            _productRepository.Delete(product);
            await _productRepository.CommitAsync();
            return NoContent();

        }
    }
}
