using Ecommerce525.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce525.API.Area.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = $" {CD.SUPER_ADMIN_ROLE},{CD.ADMIN_ROLE} ,{CD.EMPLOYEE_ROLE} ")]
    public class CategoriesController : ControllerBase
    {
        IRepository<Category> _categoryRepository;

        public CategoriesController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            //var categories = _context.Categories.AsQueryable();
            var categories = await _categoryRepository.GetAsync();
            return Ok(new ApiResponse<IEnumerable<Category>>()
            {
                IsSuccess = true  , 
                Message = "Categories Returend Seccessfully" , 
                Data = categories
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id )
        {
            //var categories = _context.Categories.AsQueryable();
            var category = await _categoryRepository.GetOneAsync(c=>c.Id == id);
            if(category is null )
            {
                return NotFound(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Invalid category Id"
                });
            }
            return Ok(new ApiResponse<Category>()
            {
                IsSuccess = true,
                Message = "Category Returend Seccessfully",
                Data = category
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            //_context.Categories.Add(category);
            //_context.SaveChanges();
            await _categoryRepository.CreateAsync(category);
            await _categoryRepository.CommitAsync();

            //Response.Cookies.Append("Success-Notification", "category Created Successfully"); 

            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "category Created Successfully",
            });
        }
        [HttpPut("{id}")]
        [Authorize(Roles = $" {CD.SUPER_ADMIN_ROLE},{CD.ADMIN_ROLE}")]
        public async Task<IActionResult> Update(int id , Category category)
        {
            var categoryInDB = await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (categoryInDB is null)
            {
                return NotFound(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Invalid category Id"
                });
            }
            categoryInDB.Name = category.Name;
            categoryInDB.Description = category.Description;
            categoryInDB.Status = category.Status;
            //_context.Categories.Update(category);
            //_context.SaveChanges();
            _categoryRepository.Update(categoryInDB);
            await _categoryRepository.CommitAsync();
            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "category updated Successfully"
            });
        }
        [Authorize(Roles = $" {CD.SUPER_ADMIN_ROLE},{CD.ADMIN_ROLE}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            //var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Invalid category Id"
                });
            }
            //_context.Categories.Remove(category); 
            //_context.SaveChanges();
            _categoryRepository.Delete(category);
            await _categoryRepository.CommitAsync(); ;
            return NoContent() ;

        }
    }
}
