using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.ComponentModel;

namespace Sentiment.Domain
{
    public interface ISentimentAnalyser
    {
        string Name { get; }

        SentimentAnalysisResult GetSentiment(string[] keywords, int duration, bool translate = false);
    }
}
