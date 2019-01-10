using System;
using System.Collections.Generic;
using System.Text;

namespace Sentiment.Domain
{
    public class SentimentAsset
    {
        public string name { get; set; }

        public string symbol { get; set; }
        
        //TODO: Alias' (e.g. Ether for Ethereum/ETH or Lumens for Stellar/XLM
    }
}
