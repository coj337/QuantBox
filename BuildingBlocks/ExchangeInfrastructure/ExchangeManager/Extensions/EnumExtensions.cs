using ExchangeManager.Models;
using ExchangeSharp;
using System;
using System.Collections.Generic;
using System.Text;
using OrderType = ExchangeManager.Models.OrderType;

namespace ExchangeManager.Helpers
{
    public static class EnumExtensions
    {
        public static ExchangeSharp.OrderType ToExSharpType(this OrderType type)
        {
            if (type == OrderType.Limit)
            {
                return ExchangeSharp.OrderType.Limit;
            }
            else if (type == OrderType.Market)
            {
                return ExchangeSharp.OrderType.Market;
            }
            else if (type == OrderType.Stop)
            {
                return ExchangeSharp.OrderType.Stop;
            }
            else
            {
                throw new Exception("OrderType has no matching type");
            }
        }

        public static OrderResult ToOrderResult(this ExchangeAPIOrderResult result)
        {
            switch (result) {
                case ExchangeAPIOrderResult.Canceled:
                    return OrderResult.Canceled;
                case ExchangeAPIOrderResult.Error:
                    return OrderResult.Error;
                case ExchangeAPIOrderResult.Filled:
                    return OrderResult.Filled;
                case ExchangeAPIOrderResult.FilledPartially:
                    return OrderResult.FilledPartially;
                case ExchangeAPIOrderResult.Pending:
                    return OrderResult.Pending;
                case ExchangeAPIOrderResult.PendingCancel:
                    return OrderResult.PendingCancel;
                default:
                    return OrderResult.Unknown;
            }
        }
    }
}
