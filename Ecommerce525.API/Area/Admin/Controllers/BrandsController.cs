using Ecommerce525.API.Repositories;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce525.API.Area.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = $" {CD.SUPER_ADMIN_ROLE},{CD.ADMIN_ROLE} ,{CD.EMPLOYEE_ROLE} ")]
    public class BrandsController : ControllerBase
    {
        IRepository<Brand> _brandRepository;    

        public BrandsController(IRepository<Brand> brandRepository)
        {
            _brandRepository = brandRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            //var brands = _context.Brands.AsQueryable();
            var brands = await _brandRepository.GetAsync();
            return Ok(new ApiResponse<IEnumerable<Brand>>()
            {
                IsSuccess = true  , 
                Message = "Brands Returned Successfully" ,
                Data = brands
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id )
        {
            var brand = await _brandRepository.GetOneAsync(b=>b.Id == id); 
            if (brand is null)
            {
                return NotFound(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "invalid Brand",
                });
            }
            return Ok(new ApiResponse<Brand>()
            {
                IsSuccess = true,
                Message = "Brand Returned Successfully",
                Data = brand
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateUpdateCategoryRequest createUpdateCategoryRequest )  
        {
            //var brand = new Brand();
            //brand.Name = createUpdateCategoryRequest.Name; 
            //brand.Description= createUpdateCategoryRequest.Description; 
            //brand.Status = createUpdateCategoryRequest.Status;

            var brand = createUpdateCategoryRequest.Adapt<Brand>(); 

            if (createUpdateCategoryRequest.ImgFile != null && createUpdateCategoryRequest.ImgFile.Length > 0)
            {
                //var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgFile.FileName);
                var fileName = Guid.NewGuid().ToString() + "-" + createUpdateCategoryRequest.ImgFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\brand_images", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    createUpdateCategoryRequest.ImgFile.CopyTo(stream);
                }
                brand.Img = fileName;
            }
            //_context.Brands.Add(brand);
            //_context.SaveChanges(); 
            await _brandRepository.CreateAsync(brand);
            await _brandRepository.CommitAsync();
            return CreatedAtAction( nameof(GetOne) , new { id = brand.Id }, new ApiResponse<object>()
            {
                IsSuccess = true , 
                Message = "Brand Created Successfully"                 
            });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id  , [FromForm] CreateUpdateCategoryRequest createUpdateCategoryRequest)
        {
            //var brandInDb = _context.Brands.AsNoTracking().FirstOrDefault(b => b.Id == brand.Id);
            var brand = await _brandRepository.GetOneAsync(filter: b => b.Id == id);
            //brand.Name = createUpdateCategoryRequest.Name;
            //brand.Description = createUpdateCategoryRequest.Description;
            //brand.Status = createUpdateCategoryRequest.Status;

            brand = createUpdateCategoryRequest.Adapt(brand);
            if (createUpdateCategoryRequest.ImgFile != null && createUpdateCategoryRequest.ImgFile.Length > 0)
            {
                //var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgFile.FileName);
                var fileName = Guid.NewGuid().ToString() + "-" + createUpdateCategoryRequest.ImgFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\brand_images", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    createUpdateCategoryRequest.ImgFile.CopyTo(stream);
                }
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\brand_images", brand.Img);

                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
                brand.Img = fileName;
            }
            //_context.Brands.Update(brand);
            //_context.SaveChanges();
            _brandRepository.Update(brand);
            await _brandRepository.CommitAsync();
            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "Brand Updated Successfully",
            });
        }
        [Authorize(Roles = $" {CD.SUPER_ADMIN_ROLE},{CD.ADMIN_ROLE}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            //var brand = _context.Brands.FirstOrDefault(c => c.Id == id);
            var brand = await _brandRepository.GetOneAsync(c => c.Id == id);
            if (brand == null)
            {
                return NotFound(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "invalid brand Id"
                }); 
            }
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\brand_images", brand.Img);

            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            //_context.Brands.Remove(brand); 
            //_context.SaveChanges();
            _brandRepository.Delete(brand);
            await _brandRepository.CommitAsync();
            return NoContent(); 

        }
    }
}
