using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce525.API.Area.Identity.Controllers
{
    [Area(CD.IDENTITY_AREA)]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetInfo()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Invalid User"
                });
            }
            //var applicationUserVM = new ApplicationUserVM() ;
            //applicationUserVM.Name = user.Name; 
            //applicationUserVM.Email = user.Email; 
            //applicationUserVM.PhoneNumber = user.PhoneNumber; 
            //applicationUserVM.Address = user.Address;
            //

            var applicationUserResponse = user.Adapt<ApplicationUserResponse>();
            return Ok(new ApiResponse<ApplicationUserResponse>()
            {
                IsSuccess = true,
                Message = "data Returned successfully" , 
                Data = applicationUserResponse
            }); 
        }
        [HttpPost("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(ApplicationUserRequest applicationUserRequest)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Invalid User"
                });
            }
            user.Name = applicationUserRequest.Name;
            user.PhoneNumber = applicationUserRequest.PhoneNumber;
            user.Address = applicationUserRequest.Address;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Invalid data" ,
                    Errors = result.Errors.Select(e=>e.Description) 
                });
            }
            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "updated user Successfully", 
            });
        }
        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(ApplicationUserRequest applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Invalid User"
                });
            }
            var result = await _userManager.ChangePasswordAsync(user, applicationUserVM.CurrentPassword, applicationUserVM.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Invalid data",
                    Errors = result.Errors.Select(e => e.Description)
                });
            }
            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "change password Successfully",
            });
        }
    }
}
