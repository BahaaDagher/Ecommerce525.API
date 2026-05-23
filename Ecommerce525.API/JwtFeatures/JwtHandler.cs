using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecommerce525.API.JwtFeatures
{
    public class JwtHandler : IJwtHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection jwtSettings;
        private readonly UserManager<ApplicationUser> _userManger ;

        public JwtHandler(IConfiguration configuration, UserManager<ApplicationUser> userManger)
        {
            _configuration = configuration;
            jwtSettings = _configuration.GetSection("JwtSettings");
            _userManger = userManger;
        }


        public async Task<string> GenerateAccessTokenAsync(ApplicationUser user )
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["securityKey"]));
            var SigningCredentials = new SigningCredentials(key , SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new Claim( ClaimTypes.Name , user.Name) ,
                new Claim( ClaimTypes.Email , user.Email) ,
                new Claim( ClaimTypes.NameIdentifier , user.Id) ,
            };
            var roles = await _userManger.GetRolesAsync(user); 
            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));  
            }

            var jwtSecurityToken  = new JwtSecurityToken(
                issuer: jwtSettings["validIssuer"] , 
                audience: jwtSettings["validAudience"] , 
                claims : claims , 
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["validTime"])) , 
                signingCredentials: SigningCredentials
            ); 
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken) ;  
        }
    }
}
