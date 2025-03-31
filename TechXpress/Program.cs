using TechXpress_infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Repositories;
using TechXpress_domain.Interfaces.Services;
using TechXpress_application.Services;
using TechXpress_infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Google;
using TechXpress_application.Mappings;
using TechXpress.Models;
using AutoMapper;
using TechXpress_data.Repositories;
using TechXpress_services;

var builder = WebApplication.CreateBuilder(args);

// Enable console logging
builder.Logging.AddConsole();

// Register AutoMapper with specific profile.
// The first AddAutoMapper call registers types from the assembly that contains WebMappingProfile,
// and the second call ensures any additional profiles in that assembly are picked up.
builder.Services.AddAutoMapper(typeof(WebMappingProfile));
builder.Services.AddAutoMapper(typeof(WebMappingProfile).Assembly);

// Register Distributed Memory Cache for session state
builder.Services.AddDistributedMemoryCache();

// Add MVC services
builder.Services.AddControllersWithViews();

// Configure the database context using the connection string "DefaultConnection" from configuration.
builder.Services.AddDbContext<TechXpress_context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<TechXpress_context>()
    .AddDefaultTokenProviders();

// Configure external authentication (Google and Facebook)
builder.Services.AddAuthentication()
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

// Register repositories (for dependency injection)
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<IShippingRepository, ShippingRepository>();
builder.Services.AddScoped<ISecurityRepository, SecurityRepository>();
builder.Services.AddScoped<ILoginHistoryRepository, LoginHistoryRepository>();

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IShippingService, ShippingService>();
builder.Services.AddScoped<ISecurityService, SecurityService>();
builder.Services.AddScoped<ILoginHistoryService, LoginHistoryService>();
builder.Services.AddScoped<IPaymentService, FakePaymentService>(); // Use FakePaymentService instead of PaymentService
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<IOrderAdminService, OrderAdminService>();

// Register API services (for calling external APIs)
builder.Services.AddHttpClient<IProductApiService, ProductApiService>();
builder.Services.AddHttpClient<ICategoryApiService, CategoryApiService>();

// Configure session state
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

// Register a named HttpClient for admin API calls, using the BaseUrl from configuration.
builder.Services.AddHttpClient("AdminApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]); // Ensure this value is set correctly in appsettings.json (e.g., "https://localhost:7276/")
});

// Add HTTP context accessor (required for session access in controllers)
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Seed roles for Identity (e.g., Admin, User)
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var roles = new[] { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
            logger.LogInformation("Created role: {Role}", role);
        }
    }
}

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Default routing
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
