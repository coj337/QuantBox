using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.ComponentModel;

namespace Sentiment.Domain
{
    public interface ISentimentListener
    {
        SentimentAnalysisResult GetSentiment(string[] keywords, int duration, bool translate = false);

        List<SentimentAsset> GetSupportedAssets();
    }
}
