using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trading.API.Domain;

namespace Trading.API.Data
{
    public static class DbInitializer
    {
        public static void Seed(TradingContext context)
        {
            if (context.Exchanges.Any())
            {
                return; //Already seeded
            }

            context.Exchanges.AddRange(new List<Exchange>()
            {
                new Exchange(){
                    Name = "Binance",
                    TradingEnabled = true
                },
                new Exchange(){
                    Name = "BtcMarkets",
                    TradingEnabled = true
                },
                new Exchange(){
                    Name = "KuCoin",
                    TradingEnabled = true
                },
                new Exchange(){
                    Name = "Coinjar",
                    TradingEnabled = true
                },
            });
            context.SaveChanges();
        }
    }
}
