using System.Collections.Generic;

namespace Trading.API.Domain
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
        public string MarketSymbol { get; set; }
        public string BaseCurrency { get; set; }
        public string AltCurrency { get; set; }
    }

    public enum ArbitrageType
    {
        Triangle,
        Normal
    }
}
