using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Sentiment.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Events;
using VaderSharp;

namespace Sentiment.Infrastructure
{
    public class TwitterSentimentAnalyser : ISentimentListener
    {
        public Dictionary<string, SentimentAnalysisResult> SentimentResults = new Dictionary<string, SentimentAnalysisResult>();

        private readonly IConfiguration _configuration;
        private readonly IHubContext<SentimentHub, ISentimentHub> _sentimentHub;

        public TwitterSentimentAnalyser(IConfiguration configuration, IHubContext<SentimentHub, ISentimentHub> sentimentHub)
        {
            _configuration = configuration;
            _sentimentHub = sentimentHub;

            string consumerKey = _configuration["Twitter:ApiKey"];
            string consumerSecret = _configuration["Twitter:ApiSecret"];
            string accessToken = _configuration["Twitter:AccessToken"];
            string accessTokenSecret = _configuration["Twitter:AccessTokenSecret"];

            Auth.SetUserCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret);
        }

        public void StartSentimentListener(List<string> keywords, bool translate = false)
        {
            // Enable Automatic RateLimit handling
            RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;
            var stream = Stream.CreateFilteredStream();

            //Add keywords to stream
            foreach (string keyword in keywords)
            {
                stream.AddTrack(keyword);
            }

            //Add languages to stream
            stream.AddTweetLanguageFilter("en");
            if (translate)
            {
                //TODO: Add more languages (or find out how to do all)
            }

            stream.MatchingTweetReceived += (sender, args) => OnMatchedTweet(sender, args, keywords.Last(), keywords.First());

            Task.Factory.StartNew(() =>
            {
                stream.StartStreamMatchingAllConditions();
            });
        }

        public List<SentimentAsset> GetSupportedAssets()
        {
            return new List<SentimentAsset>()
            {
                new SentimentAsset()
                {
                    symbol = "Bitcoin",
                    name = "BTC"
                },
                new SentimentAsset()
                {
                    symbol = "Ethereum",
                    name = "ETH"
                },
                new SentimentAsset()
                {
                    symbol = "Ripple",
                    name = "XRP"
                },
                new SentimentAsset()
                {
                    symbol = "Stellar",
                    name = "XLM"
                },
                new SentimentAsset()
                {
                    symbol = "Bitcoin Cash",
                    name = "BCH"
                },
                new SentimentAsset()
                {
                    symbol = "EOS",
                    name = "EOS"
                }
            };
        }

        private async void OnMatchedTweet(object sender, MatchedTweetReceivedEventArgs args, string symbol, string name)
        {
            SentimentIntensityAnalyzer analyzer = new SentimentIntensityAnalyzer();

            string sanitizedTweet = Sanitize(args.Tweet.FullText);
            var results = analyzer.PolarityScores(sanitizedTweet);

            if (SentimentResults.ContainsKey(symbol))
            {
                //Keep track of the running average
                SentimentResults[symbol].Negative = SentimentResults[symbol].Negative + (results.Negative - SentimentResults[symbol].Negative) / SentimentResults[symbol].ItemsChecked;
                SentimentResults[symbol].Neutral = SentimentResults[symbol].Neutral + (results.Neutral - SentimentResults[symbol].Neutral) / SentimentResults[symbol].ItemsChecked;
                SentimentResults[symbol].Positive = SentimentResults[symbol].Positive + (results.Positive - SentimentResults[symbol].Positive) / SentimentResults[symbol].ItemsChecked;
                SentimentResults[symbol].Compound = SentimentResults[symbol].Compound + (results.Compound - SentimentResults[symbol].Compound) / SentimentResults[symbol].ItemsChecked;
                SentimentResults[symbol].ItemsChecked++;
            }
            else
            {
                SentimentResults.Add(symbol, new SentimentAnalysisResult()
                {
                    Symbol = symbol,
                    Name = name,
                    Negative = results.Negative,
                    Neutral = results.Neutral,
                    Positive = results.Positive,
                    Compound = results.Compound,
                    ItemsChecked = 1
                });
            }

            await _sentimentHub.Clients.All.ReceiveSentiments(SentimentResults.Values.ToArray());
        }

        private string Sanitize(string raw)
        {
            return Regex.Replace(raw, @"(@[A-Za-z0-9]+)|([^0-9A-Za-z \t])|(\w+:\/\/\S+)", " ").ToString();
        }
    }
}
