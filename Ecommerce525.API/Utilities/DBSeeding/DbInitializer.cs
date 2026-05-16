using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace Ecommerce525.API.Utilities.DBSeeding
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DbInitializer> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DbInitializer(ApplicationDbContext context, ILogger<DbInitializer> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InitializeAsync()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }
                if(!_roleManager.Roles.Any())
                {
                    await _roleManager.CreateAsync(new IdentityRole(CD.SUPER_ADMIN_ROLE));  
                    await _roleManager.CreateAsync(new IdentityRole(CD.ADMIN_ROLE));  
                    await _roleManager.CreateAsync(new IdentityRole(CD.EMPLOYEE_ROLE));  
                    await _roleManager.CreateAsync(new IdentityRole(CD.CUSTOMER_ROLE));

                    await _userManager.CreateAsync(new ApplicationUser()
                    {
                        Name = "SuperAdmin",
                        UserName = "SuperAdmin",
                        Email = "superadmin@eraasoft.com",
                        EmailConfirmed = true
                    } , "SuperAdmin@123");

                    var user = await _userManager.FindByEmailAsync("superadmin@eraasoft.com"); 

                    await  _userManager.AddToRoleAsync(user, CD.SUPER_ADMIN_ROLE);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message); 
            }
            
        }
    }
}
