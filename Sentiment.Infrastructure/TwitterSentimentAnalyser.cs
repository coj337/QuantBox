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
using Tweetinvi.Models;
using Tweetinvi.Parameters;
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

        public List<SentimentAsset> GetSupportedAssets()
        {
            return new List<SentimentAsset>()
            {
                new SentimentAsset()
                {
                    symbol = "BTC",
                    name = "Bitcoin"
                },
                new SentimentAsset()
                {
                    symbol = "ETH",
                    name = "Ethereum"
                },
                new SentimentAsset()
                {
                    symbol = "XRP",
                    name = "Ripple"
                },
                new SentimentAsset()
                {
                    symbol = "XLM",
                    name = "Stellar"
                },
                new SentimentAsset()
                {
                    symbol = "BCH",
                    name = "Bitcoin Cash"
                },
                new SentimentAsset()
                {
                    symbol = "EOS",
                    name = "EOS"
                }
            };
        }

        //Get sentiment for given keywords over {duration} days
        public SentimentAnalysisResult GetSentiment(string[] keywords, int tweetCount = 100, bool translate = false)
        {
            List<ITweet> tweets = new List<ITweet>();

            //Get all the tweets for each keyword
            foreach (var keyword in keywords)
            {
                var searchParameter = new SearchTweetsParameters(keyword)
                {
                    Filters = TweetSearchFilters.Images,
                    //SinceId = (int)(DateTime.UtcNow.AddDays(duration * -1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds
                };

                if (!translate)
                {
                    searchParameter.Lang = LanguageFilter.English;
                }

                tweets.AddRange(Search.SearchTweets(searchParameter));
            }

            //Translate them to sentiment results
            SentimentIntensityAnalyzer analyzer = new SentimentIntensityAnalyzer();

            string sanitizedTweet = Sanitize(tweets[0].FullText);
            var results = analyzer.PolarityScores(sanitizedTweet);
            //First result needs no math
            SentimentAnalysisResult overallSentiment = new SentimentAnalysisResult()
            {
                Negative = results.Negative,
                Neutral = results.Neutral,
                Positive = results.Positive,
                Compound = results.Compound,
                ItemsChecked = 1
            };

            //Keep an average of the sentiment in the found tweets
            for(int i = 1; i < tweets.Count(); i++)
            {                
                sanitizedTweet = Sanitize(tweets[i].FullText);
                results = analyzer.PolarityScores(sanitizedTweet);

                overallSentiment.Negative = overallSentiment.Negative + (results.Negative - overallSentiment.Negative) / overallSentiment.ItemsChecked;
                overallSentiment.Neutral = overallSentiment.Neutral + (results.Neutral - overallSentiment.Neutral) / overallSentiment.ItemsChecked;
                overallSentiment.Positive = overallSentiment.Positive + (results.Positive - overallSentiment.Positive) / overallSentiment.ItemsChecked;
                overallSentiment.Compound = overallSentiment.Compound + (results.Compound - overallSentiment.Compound) / overallSentiment.ItemsChecked;
                overallSentiment.ItemsChecked++;
            }

            return overallSentiment;
        }

        //Start a listener to listen for tweets and classify them in real time
        //NOTE: Twitter only allows one stream at once
        public void StartRealTimeSentimentListener(List<string> keywords, bool translate = false)
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
