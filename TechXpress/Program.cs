using TechXpress_infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Repositories;
using TechXpress_domain.Interfaces.Services;
using TechXpress_application.Services;
using TechXpress_infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Google;
using TechXpress_services;
using TechXpress_data.Repositories;
using AutoMapper;
using TechXpress_application.Mappings;
using TechXpress.Models;

var builder = WebApplication.CreateBuilder(args);

// Enable console logging.
builder.Logging.AddConsole();

// Register AutoMapper by scanning assemblies that contain your profiles.
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Register Distributed Memory Cache for session state.
builder.Services.AddDistributedMemoryCache();

// Add MVC services.
builder.Services.AddControllersWithViews();

// Configure the database context using the "DefaultConnection" connection string.
builder.Services.AddDbContext<TechXpress_context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register ASP.NET Core Identity with role support.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<TechXpress_context>()
    .AddDefaultTokenProviders();

// Configure external authentication providers.

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
})
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
})
.AddFacebook(facebookOptions =>
{
    facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
    facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
});

// Register repository implementations.
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IShippingRepository, ShippingRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ISecurityRepository, SecurityRepository>();
builder.Services.AddScoped<ILoginHistoryRepository, LoginHistoryRepository>();

// Register application services.
builder.Services.AddScoped<ILoginHistoryService, LoginHistoryService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IShippingService, ShippingService>();
builder.Services.AddScoped<IOrderService, OrderService>();
//builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentService, FakePaymentService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<ISecurityService, SecurityService>();

// Register an HttpClient for the IProductApiService.
builder.Services.AddHttpClient<IProductApiService, ProductApiService>();
builder.Services.AddHttpClient<ICategoryApiService, CategoryApiService>();

// Configure session state.
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Optionally register in-memory caching.
builder.Services.AddMemoryCache();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddHttpClient<IProductApiService, ProductApiService>();
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Seed roles if they do not exist.
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// Configure the middleware pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

try
{
    app.Run();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Host terminated unexpectedly");
    throw;
}