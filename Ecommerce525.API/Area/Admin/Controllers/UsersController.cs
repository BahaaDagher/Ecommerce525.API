using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce525.API.Area.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{CD.SUPER_ADMIN_ROLE} , {CD.ADMIN_ROLE}")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult GetList()
        {
            var users = _userManager.Users.AsQueryable();
            // filter 
            return Ok(new ApiResponse<IEnumerable<ApplicationUser>>()
            {
                IsSuccess = true,
                Message = "Users Returned Successfully",
                Data = users.AsEnumerable()
            }); 
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> LockUnLock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();
            bool IsSuperAdmin = await _userManager.IsInRoleAsync(user, CD.SUPER_ADMIN_ROLE);
            if (IsSuperAdmin)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Can't Lock Super Admin!!!"
                });
            }
            bool UnLockUser = true;  
            //unlock
            if (user.LockoutEnd == null || DateTime.UtcNow > user.LockoutEnd)
            {
                // lock 
                //user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddMinutes(15));
                UnLockUser = false; 
            }
            // lock
            else
            {
                // unlock 
                //user.LockoutEnd = null;
                await _userManager.SetLockoutEndDateAsync(user, null);
            }
            string message = "user Locked Successfully "; 
            if (UnLockUser)
            {
                message = "user UnLocked Successfully"; 
            }
            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = message
            });
        }
    }
}
