using System;
using System.Text;

namespace BtcMarketsApiClient.Helpers
{
    public class RequestHelper
    {
        public static string BuildOrderString(string currency, string instrument, int limit, string since)
        {
            var sb = new StringBuilder();
            // These need to be in this specific order for the API to work
            sb.Append("{\"currency\":\"");
            sb.Append(currency);
            sb.Append("\",\"instrument\":\"");
            sb.Append(instrument);
            sb.Append("\",\"limit\":");
            sb.Append(limit);
            if (since != null)
            {
                sb.Append(",\"since\":");
                sb.Append(since);
            }

            sb.Append("}");
            return sb.ToString();
        }

        public static string BuildNewOrderString(string currency, string instrument, long price, int volume,
            string orderSide, string orderType)
        {
            var sb = new StringBuilder();
            // These need to be in this specific order for the API to work
            sb.Append("{\"currency\":\"");
            sb.Append(currency);
            sb.Append("\",\"instrument\":\"");
            sb.Append(instrument);
            sb.Append("\",\"price\":");
            sb.Append(price);
            sb.Append(",\"volume\":");
            sb.Append(volume);
            sb.Append(",\"orderSide\":\"");
            sb.Append(orderSide);
            sb.Append("\",\"ordertype\":\"");
            sb.Append(orderType);
            sb.Append("\",\"clientRequestId\":\"");
            sb.Append(Guid.NewGuid());
            sb.Append("\"}");
            return sb.ToString();
        }

        public static string BuildDefaultOrderString()
        {
            return BuildOrderString("AUD", "BTC", 10, "1");
        }

        /// <summary>
        /// Builds up a string containing the Order ID's that you wish to query (currently only supports 1 ID but to be extended)
        /// </summary>
        /// <param name="orderID">The Unique Order ID of the BTC Markets orde you wish to query</param>
        /// <returns>{"orderIds":[6840125478]}</returns>
        public static string BuildOrderRequestFromID(string orderID)
        {
            var sb = new StringBuilder();
            sb.Append("{\"orderIds\":");
            sb.Append("[");
            sb.Append(orderID);
            sb.Append("]");
            sb.Append("}");
            return sb.ToString();

        }
    }


}