using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sentiment.Domain;
using Sentiment.Infrastructure;

namespace Sentiment.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SentimentController : ControllerBase
    {
        private readonly TwitterSentimentAnalyser _twitterSentiment;

        public SentimentController(TwitterSentimentAnalyser twitterSentiment)
        {
            _twitterSentiment = twitterSentiment;
        }

        [Route("TwitterSentiment")]
        [HttpGet]
        [ProducesResponseType(typeof(SentimentAnalysisResult), (int)HttpStatusCode.OK)]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<SentimentAnalysisResult>> GetTwitterSentiment(string symbol, string name, int duration = 100, bool translate = false)
        {
            return Ok(_twitterSentiment.GetSentiment(new string[] { symbol, name }, duration, translate));
        }
    }
}
