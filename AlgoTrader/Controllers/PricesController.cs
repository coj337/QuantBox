using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sentiment.Infrastructure;

namespace MyVested.Pro.Controllers{
    [Route("[controller]")]
    public class PricesController : Controller {
        [HttpGet("[action]")]
        public JsonResult Tickers(){
            return Json(new List<Price>(){
                new Price(){
                    Symbol = "BTC/ETH",
                    Bid = 0.03318671m,
                    Ask = 0.03318684m
                },
                new Price(){
                    Symbol = "BTC/XRP",
                    Bid = 0.00007861m,
                    Ask = 0.00007872m
                },
                new Price(){
                    Symbol = "BTC/BCH",
                    Bid = 0.08290863m,
                    Ask = 0.08290867m
                }
            });
        }

        public class Price {
            public string Symbol { get; set; }
            public decimal Bid { get; set; }
            public decimal Ask { get; set; }
        }
    }
}
