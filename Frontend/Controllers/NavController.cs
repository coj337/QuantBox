using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyVested.Pro.Controllers{
    [Route("[controller]")]
    public class NavController : Controller{
        [HttpGet("[action]")]
        public JsonResult Accounts(){
            return Json(new List<NavAccount>(){
                new NavAccount(){ name = "Account 1" },
                new NavAccount(){ name = "Account 2" }
            });
        }

        [HttpGet("[action]")]
        public JsonResult Indicators(){
            return Json(new List<NavIndicator>(){
                new NavIndicator(){ name = "Indicator 1" },
                new NavIndicator(){ name = "Indicator 2" }
            });
        }

        [HttpGet("[action]")]
        public JsonResult Bots(){
            return Json(new List<NavBot>(){
                new NavBot(){ name = "Bot 1" },
                new NavBot(){ name = "Bot 2" }
            });
        }

        [HttpGet("[action]")]
        public JsonResult Scripts(){
            return Json(new List<NavScript>(){
                new NavScript(){ name = "Script 1" },
                new NavScript(){ name = "Script 2" },
            });
        }

        public class NavAccount{
            public string name { get; set; }
        }

        public class NavIndicator{
            public string name { get; set; }
        }

        public class NavBot{
            public string name { get; set; }
        }

        public class NavScript{
            public string name { get; set; }
        }
    }
}
