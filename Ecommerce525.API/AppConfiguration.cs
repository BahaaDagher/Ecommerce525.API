using Ecommerce525.API.Repositories;
using Ecommerce525.API.Utilities;
using Ecommerce525.API.Utilities.DBSeeding;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Ecommerce525.API
{
    public static class AppConfiguration
    {
        public static void RegisterConfig(this IServiceCollection services)
        {
            // Transient , Scoped , Singlton  
            services.AddScoped<IRepository<Models.Product>, Repository<Models.Product>>();
            services.AddScoped<IRepository<Category>, Repository<Category>>();
            services.AddScoped<IRepository<Brand>, Repository<Brand>>();
            services.AddScoped<IRepository<Cart>, Repository<Cart>>();
            services.AddScoped<IRepository<Promotion>, Repository<Promotion>>();
            services.AddScoped<IRepository<ApplicationUserOTP>, Repository<ApplicationUserOTP>>();
            services.AddScoped<IProductSubImageRepository, ProductSubImageRepository>();
            services.AddScoped<IProductColorRepository, ProductColorRepository>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IDbInitializer, DbInitializer>();
        }
    }
}
