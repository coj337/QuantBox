using Market.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Market.API.Infrastructure
{
    public class MarketContext : DbContext
    {
        public MarketContext(DbContextOptions<MarketContext> options) : base(options){}

        public DbSet<ExchangeConfig> ExchangeConfigs { get; set; }
    }

    public class CatalogContextDesignFactory : IDesignTimeDbContextFactory<MarketContext>
    {
        public MarketContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MarketContext>()
                .UseSqlServer("Server=.;Initial Catalog=MarketDb;Integrated Security=true");

            return new MarketContext(optionsBuilder.Options);
        }
    }
}
