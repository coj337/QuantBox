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
        public ActionResult<SentimentAsset[]> GetSupportedAssets()
        {
            return new SentimentAsset[]
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

        [Route("TwitterSentiment")]
        [HttpGet]
        [ProducesResponseType(typeof(SentimentAnalysisResult), (int)HttpStatusCode.OK)]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public ActionResult<SentimentAnalysisResult> GetTwitterSentiment(string symbol, string name, int duration = 100, bool translate = false)
        {
            var sentimentResult = _sentimentService.GetSentiment("Twitter", symbol, name, duration, translate);
            return Ok(sentimentResult);
        }
    }
}
