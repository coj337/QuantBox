using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Sentiment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SentimentController : ControllerBase
    {
        // GET api/sentiment
        // Asset: Asset to monitor
        // Duration: Time to monitor the asset for
        // Translate: Translate from non-english languages?
        [HttpGet]
        public ActionResult<IEnumerable<string>> TwitterSentiment(List<string> keywords, string duration, bool translate = false)
        {
            throw new NotImplementedException();
        }
    }
}
