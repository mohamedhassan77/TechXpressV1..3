using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using TechXpress_domain;

namespace TechXpress_infrastructure.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TechXpress_context>
    {
        public TechXpress_context CreateDbContext(string[] args)
        {
            // Build configuration
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).FullName) // Move up to the solution directory
                .AddJsonFile("TechXpress/appsettings.json") // Path to the appsettings.json file in the startup project
                .Build();

            // Configure DbContextOptions
            var builder = new DbContextOptionsBuilder<TechXpress_context>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connectionString);

            // Return a new instance of the DbContext
            return new TechXpress_context(builder.Options);
        }
    }
}