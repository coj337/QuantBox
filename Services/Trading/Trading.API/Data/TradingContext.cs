using ExchangeManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Trading.API.Domain;
using Trading.API.Domain.BotTemplateAggregate;

namespace Trading.API.Data
{
    public class TradingContext : DbContext
    {
        public TradingContext(DbContextOptions<TradingContext> options) : base(options) { }

        public DbSet<ExchangeConfig> ExchangeCredentials { get; set; }
        public DbSet<Exchange> Exchanges { get; set; }
        public DbSet<TradeResults> ArbitrageResults { get; set; }
        public DbSet<BotTemplate> Templates { get; set; }
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
