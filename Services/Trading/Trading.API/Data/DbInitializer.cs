using ExchangeManager.Models;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trading.API.Domain;
using Trading.API.Domain.Actions;
using Trading.API.Domain.BotTemplateAggregate;
using Trading.API.Domain.Indicators;
using Trading.API.Domain.Insurances;
using Trading.API.Domain.Safeties;
using Trading.API.Domain.TradeSettingsAggregate;
using OrderType = ExchangeManager.Models.OrderType;

namespace Trading.API.Data
{
    public static class DbInitializer
    {
        public static void Seed(TradingContext context)
        {
            if (!context.Exchanges.Any())
            {
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

            if (!context.Templates.Any())
            {
                context.Templates.AddRange(new List<BotTemplate>()
                {
                    new BotTemplate("Blank"),
                    new BotTemplate("Triangle Arbitrage")
                    {
                        //TODO: Complete templates
                    },
                    new BotTemplate("Normal Arbitrage")
                    {
                        Indicators = new List<IIndicator>()
                        {
                            new PriceCondition()
                            {
                                StartExchange = "Binance",
                                StartSide = OrderSide.Buy,
                                StartPair = "ETH/BTC",
                                EndExchange = "KuCoin",
                                EndSide = OrderSide.Sell,
                                EndPair = "ETH/BTC",
                                ConditionType = PriceConditionType.Higher
                            }
                        },
                        Actions = new List<IAction>()
                        {
                            new CreateOrder()
                            {
                                Exchange = "Binance",
                                OrderType = OrderType.Market,
                                Pair = new Pair()
                                {
                                    MarketSymbol = "ETH/BTC",
                                    BaseCurrency = "BTC",
                                    AltCurrency = "ETH"
                                },
                                ExistingCurrency = "BTC"
                            },
                            new CreateOrder()
                            {
                                Exchange = "KuCoin",
                                OrderType = OrderType.Market,
                                Pair = new Pair()
                                {
                                    MarketSymbol = "ETH/BTC",
                                    BaseCurrency = "BTC",
                                    AltCurrency = "ETH"
                                },
                                ExistingCurrency = "ETH"
                            }
                        }
                    }
                });
                context.SaveChanges();
            }
        }
    }
}
