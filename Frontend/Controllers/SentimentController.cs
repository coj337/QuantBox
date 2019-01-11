using Microsoft.AspNetCore.Mvc;
using Sentiment.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Controllers
{
    public class SentimentController : Controller
    {
        private readonly TwitterSentimentAnalyser _twitterSentiment;

        public SentimentController(TwitterSentimentAnalyser twitterSentiment)
        {
            _twitterSentiment = twitterSentiment;
        }

        [HttpGet]
        public JsonResult GetSupportedAssets()
        {
            return Json(_twitterSentiment.GetSupportedAssets());
        }

        [HttpGet]
        public JsonResult GetTwitterSentiment(string symbol, string name, int duration = 100, bool translate = false)
        {
            return Json(_twitterSentiment.GetSentiment(new string[] { symbol, name }, duration, translate));
        }
    }
}
