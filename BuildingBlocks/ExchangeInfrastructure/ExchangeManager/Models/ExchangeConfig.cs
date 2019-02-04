using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeManager.Models
{
    public class ExchangeConfig
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Nickname { get; set; }

        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }

        public bool Simulated { get; set; }
    }
}
