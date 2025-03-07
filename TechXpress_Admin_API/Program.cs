using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TechXpress_application.Services;
using TechXpress_infrastructure.Data;
using TechXpress_domain.Interfaces.Services;
using TechXpress_domain.Interfaces.Repositories;
using TechXpress_infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using TechXpress_domain.Entities;
using Microsoft.OpenApi.Models;
using TechXpress_application.Mappings;
using System.Text.Json.Serialization;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);

// --------------------------
// Add services to the container.
// --------------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // This helps avoid circular reference issues during JSON serialization.
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
// Add controllers support.
builder.Services.AddControllers();

// Configure your DbContext using the "AdminConnection" connection string from configuration.
builder.Services.AddDbContext<TechXpress_context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AdminConnection")));

// Register Identity services with role support.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<TechXpress_context>()
    .AddDefaultTokenProviders();

// Register repository and service implementations.
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminService,AdminService>();
builder.Services.AddScoped<IReviewService,ReviewService>();

// Register an HttpClient for the IProductApiService implementation.
builder.Services.AddHttpClient<IProductApiService, ProductApiService>();

// Configure CORS to allow calls from your MVC client (adjust the origins as needed).
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMvcDomain", policy =>
    {
        policy.WithOrigins("http://localhost:5298", "https://localhost:7248")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
}); 
// --------------------------
// Configure JWT authentication.
// --------------------------
var jwtSecret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new InvalidOperationException("JWT Secret is missing in configuration.");
}
var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];
var key = Encoding.ASCII.GetBytes(jwtSecret);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])),
        RoleClaimType = ClaimTypes.Role
    };
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TechXpress_Admin_API", Version = "v1" });

    // Add JWT Bearer Security Definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

     c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
// --------------------------
// Register AutoMapper by scanning all assemblies that contain your mapping profiles.

// --------------------------
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// --------------------------
// Add an authorization policy for Admin users.
// --------------------------
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireRole("role", "Admin");
    });
});

// --------------------------
// Optionally add Swagger/OpenAPI support.
// --------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------------
// Build the app.
// --------------------------
var app = builder.Build();

// --------------------------
// Configure middleware.
// --------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS for the configured policy.
app.UseCors("AllowMvcDomain");

// Use HTTPS redirection.
app.UseHttpsRedirection();

app.UseRouting();

// Enable authentication and authorization.
app.UseAuthentication();
app.UseAuthorization();

// Map controller routes.
app.MapControllers();

// Run the application.
app.Run();
