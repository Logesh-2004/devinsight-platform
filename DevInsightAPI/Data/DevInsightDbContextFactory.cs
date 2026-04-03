using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DevInsightAPI.Data
{
    public class DevInsightDbContextFactory : IDesignTimeDbContextFactory<DevInsightDbContext>
    {
        public DevInsightDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection is missing from configuration.");

            var optionsBuilder = new DbContextOptionsBuilder<DevInsightDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new DevInsightDbContext(optionsBuilder.Options);
        }
    }
}
