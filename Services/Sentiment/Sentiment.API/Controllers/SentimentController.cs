using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Sentiment.Domain;
using Sentiment.Infrastructure;

namespace Sentiment.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SentimentController : ControllerBase
    {
        private readonly SentimentService _sentimentService;

        public SentimentController(SentimentService sentimentService)
        {
            _sentimentService = sentimentService;
        }

        [HttpGet]
        [Route("SupportedAssets")]
        public ActionResult<IEnumerable<SentimentAsset>> GetSupportedAssets(int limit = 20)
        {
            return Ok(_sentimentService.GetAssets().Take(20));
        }

        [HttpGet]
        [Route("TwitterSentiment")]
        [ProducesResponseType(typeof(SentimentAnalysisResult), (int)HttpStatusCode.OK)]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public ActionResult<SentimentAnalysisResult> GetTwitterSentiment(string symbol, string name, int duration = 100, bool translate = false)
        {
            var sentimentResult = _sentimentService.GetSentiment("Twitter", symbol, name, duration, translate);
            return Ok(sentimentResult);
        }
    }
}
