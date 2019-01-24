using System;

namespace BtcMarketsApiClient.Helpers
{
    public class ConversionHelper
    {
        public static string ReturnCurrentTimeStampInMilliseconds()
        {
            var unixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var secondsSinceEpoch = (long) (DateTime.UtcNow - unixTime).TotalMilliseconds;

            var nonce = secondsSinceEpoch.ToString();

            return nonce;
        }

        public static DateTime TimeStampInDateFormat(string milliseconds)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(Convert.ToDouble(milliseconds));
        }
    }
}