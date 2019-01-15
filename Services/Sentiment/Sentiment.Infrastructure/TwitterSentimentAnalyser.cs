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
    public class TwitterSentimentAnalyser : ISentimentAnalyser
    {
        public string Name { get => "Twitter"; }
        public Dictionary<string, SentimentAnalysisResult> SentimentResults = new Dictionary<string, SentimentAnalysisResult>();


        public TwitterSentimentAnalyser(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            Auth.SetUserCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret);
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

        private string Sanitize(string raw)
        {
            return Regex.Replace(raw, @"(@[A-Za-z0-9]+)|([^0-9A-Za-z \t])|(\w+:\/\/\S+)", " ").ToString();
        }
    }
}
