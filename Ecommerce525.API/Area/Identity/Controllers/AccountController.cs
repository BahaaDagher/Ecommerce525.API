
using Ecommerce525.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace Ecommerce525.API.Area.Identity.Controllers
{
    [Area(CD.IDENTITY_AREA)]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        IEmailSender _emailSender;
        IRepository<ApplicationUserOTP> _applicationUserOTP;
        IJwtHandler _jwtHandler;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IRepository<ApplicationUserOTP> applicationUserOTP, IJwtHandler jwtHandler)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _applicationUserOTP = applicationUserOTP;
            _jwtHandler = jwtHandler;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequest registerVM)
        {
            ApplicationUser user = new ApplicationUser();
            user.Name = registerVM.Name;
            user.UserName = registerVM.UserName;
            user.Email = registerVM.Email;
            user.Address = registerVM.Address;

            var result = await _userManager.CreateAsync(user, registerVM.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "failed to create  user",
                    Errors = result.Errors.Select(e => e.Description)
                });  
            }
            await _userManager.AddToRoleAsync(user, CD.CUSTOMER_ROLE);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", userId = user.Id, token }, Request.Scheme);
            await _emailSender.SendEmailAsync(
                registerVM.Email,
                "Confirm Your Email from Ecommerce525",
                $"<h1> click <a href= {link} > here </a> to confirm your mail </h1>"
                );
            return CreatedAtAction("Login" , new ApiResponse<object>()
            {
                IsSuccess = true , 
                Message = "User Created Successfully" , 
            });
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.UserNameOrEmail) ??
                        await _userManager.FindByNameAsync(loginRequest.UserNameOrEmail);

            if (user == null)
            {
                return NotFound(new ApiResponse<object>()
                {
                    IsSuccess = false  , 
                    Message  = "invalid UserName Or Email Or Password"
                }); 
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginRequest.Password, loginRequest.RememberMe, true);
            if (!result.Succeeded)
            {
                List<string > errors = new List<string>();
                if (result.IsLockedOut)
                {
                    errors.Add( "to many attempts try again later");
                }
                if (result.IsNotAllowed)
                {
                    errors.Add( "please Confirm Your Email");
                }
                else
                {
                    errors.Add("invalid UserName Or Email Or Password");
                }
                return BadRequest(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Login Failed" , 
                    Errors = errors
                }); 
            }
            var token = await _jwtHandler.GenerateAccessTokenAsync(user); 
            return Ok(new AuthenticatedResponse()
            {
                AccessToken = token 
            });
        }
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string UserId, string token)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user is null)
            {
                return NotFound( new ApiResponse<object>()
                {
                    IsSuccess = false  , 
                    Message = "Invalid User" 
                }); 
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "error when  Confirm Email",
                    Errors = result.Errors.Select(e => e.Description)
                }); 
            }
            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "Email Confirmed successfully",
            });
        }
        [HttpPost("ResendEmailConfirmation")]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationRequest resendEmailConfirmationRequest)
        {
            var user = await _userManager.FindByEmailAsync(resendEmailConfirmationRequest.UserNameOrEmail) ??
                       await _userManager.FindByNameAsync(resendEmailConfirmationRequest.UserNameOrEmail);
            if (user == null)
            {
                return NotFound(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Invalid User"
                });
            }
            if (user.EmailConfirmed)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Your  Email is Already Confirmed"
                });
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", userId = user.Id, token }, Request.Scheme);
            await _emailSender.SendEmailAsync(
                user.Email,
                "Confirm Your Email from Ecommerce525",
                $"<h1> click <a href= {link} > here </a> to confirm your mail </h1>"
                );
            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "Resend Email Comfiramtion successfully"
            });
        }
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest forgetPasswordRequest)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordRequest.UserNameOrEmail) ??
                      await _userManager.FindByNameAsync(forgetPasswordRequest.UserNameOrEmail);
            if (user == null)
            {
                return NotFound(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Invalid User"
                });
            }
            var otp = new Random().Next(1000, 9999).ToString();
            var applicationUserOtp = new ApplicationUserOTP(otp, user.Id);

            await _applicationUserOTP.CreateAsync(applicationUserOtp);
            await _applicationUserOTP.CommitAsync();

            await _emailSender.SendEmailAsync(
                user.Email,
                "Reset your password",
                $"<h1> use this <span style= \"color: red\" >{otp}</span> to Reset your password </h1>"
                );
            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "Email Send with OTP successfully"
            });
            //return RedirectToAction(nameof(VerifyOTP), new { userId = user.Id });
        }
        [HttpPost("VerifyOTP")]
        public async Task<IActionResult> VerifyOTP(VerifyOTPRequest verifyOTPVM)
        {
            var user = await _userManager.FindByIdAsync(verifyOTPVM.UserId);
            if (user == null)
            {
                return NotFound(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Invalid User"
                });
            }
            var otps = await _applicationUserOTP.GetAsync(e =>
                e.ApplicationUserId == user.Id &&
                e.IsValid == true &&
                DateTime.UtcNow < e.ValidTo
                );
            //var otp = otps.OrderBy(e => e.CreatedAt).LastOrDefault();
            var otp = otps.OrderByDescending(e => e.CreatedAt).FirstOrDefault();
            if (otp == null || otp.OTP != verifyOTPVM.OTP)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "invalid / Expired OTP "
                });
            }
            otp.IsValid = false;
            await _applicationUserOTP.CommitAsync();
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "OTP Verfied Successfully"
            });
            //return RedirectToAction(nameof(ResetPassword), new { userId = user.Id });
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPasswordVM)
        {

            var user = await _userManager.FindByIdAsync(resetPasswordVM.UserId);
            if (user == null)
            {
                return NotFound(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Invalid User"
                });
            }
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordVM.Token, resetPasswordVM.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Error while ResetPassword " , 
                    Errors = result.Errors.Select(e=>e.Description) 
                });
            }
            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "Reset Password Successfully"
            });
        }
    }
}
