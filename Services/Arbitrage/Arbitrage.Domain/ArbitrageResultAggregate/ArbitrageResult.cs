using System;
using System.Collections.Generic;

namespace Arbitrage.Domain
{
    public class ArbitrageResult
    {
        public List<string> Exchanges { get; set; } //Exchanges involved in this transaction

        public List<Pair> Pairs { get; set; } //Pairs involved in this transaction

        public string InitialCurrency { get; set; } //Symbol of the initial currency (e.g. ETH if we do ETH->BTC->ETH)

        public decimal InitialLiquidity { get; set; } //How much of the initial currency we want to trade with

        public decimal TransactionFee { get; set; } // Transaction fee incurred to get the asset from first exchange to second

        public decimal Profit { get; set; } // % spread between the initial and final bid/ask

        public ArbitrageType Type { get; set; } //Type of arbitrage (i.e. Triangle or Normal)
    }

    public class Pair
    {
        public string BaseCurrency { get; set; }
        public string AltCurrency { get; set; }

        public Pair(string baseCurrency, string altCurrency)
        {
            BaseCurrency = baseCurrency;
            AltCurrency = altCurrency;
        }
    }

    // Custom comparer for the Product class
    public class PairComparer : IEqualityComparer<Pair>
    {
        // Products are equal if their names and product numbers are equal.
        public bool Equals(Pair x, Pair y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (x is null || y is null)
                return false;

            //Check whether the products' properties are equal.
            return x.BaseCurrency == y.BaseCurrency && x.AltCurrency == y.AltCurrency;
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.
        public int GetHashCode(Pair pair)
        {
            //Check whether the object is null
            if (pair is null) return 0;
            
            //Get hash code for the BaseCurrency field if it is not null.
            int hashPairBase = string.IsNullOrEmpty(pair.BaseCurrency) ? 0 : pair.BaseCurrency.GetHashCode();

            //Get hash code for the AltCurrency field.
            int hashPairAlt = string.IsNullOrEmpty(pair.AltCurrency) ? 0 : pair.AltCurrency.GetHashCode();

            //Calculate the hash code for the product.
            return hashPairBase ^ hashPairAlt;
        }
    }

    public enum ArbitrageType
    {
        Triangle,
        Normal
    }
}
