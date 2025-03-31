using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechXpress_domain.Interfaces.Repositories;
using TechXpress_domain.Interfaces.Services;
using TechXpress_infrastructure.Repositories;
using TechXpress_infrastructure.Data;
using TechXpress_application.Services;

namespace TechXpress_Admin_API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddAdminServices(this IServiceCollection services, IConfiguration config)
        {
            // Database Context
            services.AddDbContext<TechXpress_context>(options =>
                options.UseSqlServer(
                    config.GetConnectionString("AdminConnection"),
                    b => b.MigrationsAssembly("TechXpress_Infrastructure")));

            // Repositories
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<ICartRepository, CartRepository>();

            // Services
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            // Register admin-specific order service
            services.AddScoped<IOrderAdminService, OrderAdminService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<ICategoryApiService, CategoryApiService>();

            return services;
        }
    }
}
