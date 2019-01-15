using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Sentiment.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sentiment.Infrastructure
{
    public class SentimentService : IHostedService
    {
        private readonly List<ISentimentAnalyser> _supportedAnalysers;

        public SentimentService(IConfiguration configuration)
        {
            //Twitter creds
            string consumerKey = configuration["Twitter:ApiKey"];
            string consumerSecret = configuration["Twitter:ApiSecret"];
            string accessToken = configuration["Twitter:AccessToken"];
            string accessTokenSecret = configuration["Twitter:AccessTokenSecret"];

            _supportedAnalysers = new List<ISentimentAnalyser>()
            {
                new TwitterSentimentAnalyser(consumerKey, consumerSecret, accessToken, accessTokenSecret)
            };
        }

        public SentimentAnalysisResult GetSentiment(string source, string symbol, string name, int duration = 100, bool translate = false)
        {
            var sourceAnalyser = _supportedAnalysers.FirstOrDefault(x => x.Name == source);
            if(sourceAnalyser == null)
            {
                throw new Exception("Analyser " + source + " not found!");
            }

            return sourceAnalyser.GetSentiment(new string[] { symbol, name }, duration, translate);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
