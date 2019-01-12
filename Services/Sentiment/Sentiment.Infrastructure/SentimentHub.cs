using Microsoft.AspNetCore.SignalR;
using Sentiment.Domain;
using System.Threading.Tasks;

namespace Sentiment.Infrastructure
{
    public class SentimentHub : Hub<ISentimentHub>
    {

    }
}
