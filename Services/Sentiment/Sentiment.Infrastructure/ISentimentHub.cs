using Microsoft.AspNetCore.SignalR;
using Sentiment.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sentiment.Infrastructure
{
    public interface ISentimentHub
    {
        Task SendSentiments(SentimentAnalysisResult[] sentimentResults);
        Task ReceiveSentiments(SentimentAnalysisResult[] sentimentResults);
    }
}
