using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.ComponentModel;

namespace Sentiment.Domain
{
    public interface ISentimentListener
    {
        void StartSentimentListener(List<string> keywords, bool translate = false);

        List<SentimentAsset> GetSupportedAssets();
    }
}
