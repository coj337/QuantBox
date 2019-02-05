using ExchangeManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trading.API.Domain;

namespace Trading.API.Data
{
    public class TradingContext : DbContext
    {
        public TradingContext(DbContextOptions<TradingContext> options) : base(options) { }

        public DbSet<ExchangeConfig> ExchangeCredentials { get; set; }
        public DbSet<Exchange> Exchanges { get; set; }
        public DbSet<ArbitrageTradeResults> ArbitrageResults { get; set; }
        public DbSet<Bot> Bots { get; set; }

        public class TradingContextDesignFactory : IDesignTimeDbContextFactory<TradingContext>
        {
            public TradingContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<TradingContext>().UseSqlServer("Server=.;Initial Catalog=TradingDb;Integrated Security=true");

                return new TradingContext(optionsBuilder.Options);
            }
        }
    }
}
