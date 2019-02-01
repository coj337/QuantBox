using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeManager.Helpers
{
    public static class OrderSizeExtensions
    {
        public static decimal ToDecimalPlaces(this decimal originalNumber, int decimalPlaces)
        {
            var power = Convert.ToDecimal(Math.Pow(10, decimalPlaces));
            return Math.Floor(originalNumber * power) / power;
        }
    }
}
