using ExchangeManager.Models;
using ExchangeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExchangeManager.Helpers
{
    public static class PriceCalculator
    {
        //Note: Amount should be the amount in the alt, not the amount in the base currency
        public static decimal GetPriceQuote(List<OrderbookOrder> orders, decimal amount)
        {
            try
            {
                decimal price = 0;

                decimal amountLeft = amount;
                int i = 0;

                //Loop until our order's filled or we run out of orderbook
                while (amountLeft > 0 && i < orders.Count())
                {
                    decimal altAmount = orders[i].Amount;
                    //if (pair.EndsWith("USDT"))
                    //{
                    //    altAmount *= orders[i].Price;
                    //}
                    decimal amountBought = altAmount;
                    if (altAmount > amountLeft) //Make sure we only partial fill the last of the order
                    {
                        amountBought = amountLeft;
                    }

                    //Keep track of how much is left
                    amountLeft -= amountBought;

                    //Track average price payed for the amount filled
                    price += (orders[i].Price / (amount / amountBought));

                    i++;
                }

                if (i > orders.Count())
                {
                    throw new Exception("Orderbook too thin, couldn't calculate price, requested " + amount + " when only " + orders.Sum(x => x.Amount) + " was available");
                }

                return price;
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong in GetPriceQuote (" + e.Message + ")");
            }
        }

        public static decimal GetPriceQuote(List<ExchangeOrderPrice> orders, decimal amount)
        {
            try
            {
                decimal price = 0;

                decimal amountLeft = amount;
                int i = 0;

                //Loop until our order's filled or we run out of orderbook
                while (amountLeft > 0 && i < orders.Count())
                {
                    decimal altAmount = orders[i].Amount;

                    decimal amountBought = altAmount;
                    if (altAmount > amountLeft) //Make sure we only partial fill the last of the order
                    {
                        amountBought = amountLeft;
                    }

                    //Keep track of how much is left
                    amountLeft -= amountBought;

                    //Track average price payed for the amount filled
                    price += (orders[i].Price / (amount / amountBought));

                    i++;
                }

                if (i > orders.Count())
                {
                    throw new Exception("Orderbook too thin, couldn't calculate price, requested " + amount + " when only " + orders.Sum(x => x.Amount) + " was available");
                }

                return price;
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong in GetPriceQuote (" + e.Message + ")");
            }
        }

        //Converts a base currency to an alt currency at the market rate
        public static decimal ConvertBaseToAlt(decimal price, decimal baseCurrencyAmount)
        {
            var altCurrencyAmount = baseCurrencyAmount / price;

            return altCurrencyAmount;
        }
    }
}
