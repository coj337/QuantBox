using System;
using System.Collections.Generic;
using System.Text;

namespace Market.Domain
{
    public class CurrencyData
    {
        public string Name { get; set; }

        public string Symbol { get; set; }

        public bool TradingEnabled { get; set; }

        public bool WithdrawalEnabled { get; set; }

        public bool DepositEnabled { get; set; }

        public decimal WithdrawalFee { get; set; }

        public decimal MinWithdrawal { get; set; }

        public int MinConfirmations { get; set; }
    }
}
