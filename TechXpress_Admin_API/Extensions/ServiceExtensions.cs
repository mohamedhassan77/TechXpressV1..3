using Microsoft.EntityFrameworkCore;
using System;
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
            // Add other repositories
            // Services
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            // Add other services
            return services;
        }
    }
}
