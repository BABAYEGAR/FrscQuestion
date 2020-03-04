using System;

namespace FrscQuestion.Models.Services
{
    public class TimeConverter
    {
        /// <summary>
        ///     This method converts a datetime to a unix timestamp
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public long ConvertDateTimeToUnix(DateTime date)
        {
            long unixTimestamp = (int) date.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            return unixTimestamp;
        }

        /// <summary>
        ///     This method converts a unit=x timestamp to a datetime
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public DateTime ConvertUnixToDateTime(long timestamp)
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(timestamp / 1000d)).ToLocalTime();
            return dt;
        }
    }
}