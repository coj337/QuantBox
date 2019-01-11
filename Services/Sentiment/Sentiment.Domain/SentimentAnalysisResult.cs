using System;
using System.Collections.Generic;
using System.Text;

namespace Sentiment.Domain
{
    public class SentimentAnalysisResult
    {
        public string Symbol { get; set; }
        
        public string Name { get; set; }

        public double Negative { get; set; }

        public double Neutral { get; set; }

        public double Positive { get; set; }

        public double Compound { get; set; }

        public int ItemsChecked { get; set; }
    }
}
